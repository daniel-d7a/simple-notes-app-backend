using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todo_app.core;
using todo_app.core.DTOs;
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;
using todo_app.core.Models.Data;
using todo_app.core.Models.ResponseModels.General;
using todo_app.core.Repositories;

namespace todo_app.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodosController(IUnitOfWork unitOfWork) : SharedController
{
    [HttpGet]
    public ActionResult<GenericResponse<PaginatedResult<Todo>>> GetAllTodos(
        [FromQuery] BaseQueryParams queryParams,
        string? type,
        string? label
    )
    {
        var res = new GenericResponse<PaginatedResult<Todo>>();
        var userId = GetUserId();
        PaginatedResult<Todo> todos;
        if (string.IsNullOrEmpty(type) && string.IsNullOrEmpty(label))
        {
            todos = unitOfWork.Todos.GetAllByUserWithLabelsPaginated(
                userId,
                queryParams,
                [t => t.Entries]
            );
        }
        else if (!string.IsNullOrEmpty(type))
        {
            switch (type)
            {
                case Types.Favourite:
                    todos = unitOfWork.Todos.GetAllByUserWithLabelsPaginated(
                        userId,
                        queryParams,
                        [t => t.Entries],
                        t => t.IsFavourite
                    );
                    break;
                default:
                    res.Error = "Unknown type";
                    return UnprocessableEntity(res);
            }
        }
        else if (!string.IsNullOrEmpty(label))
        {
            var userLabel = unitOfWork
                .Labels.GetAllByUser(userId)
                .FirstOrDefault(l => l.Name == label);
            if (userLabel is null)
            {
                res.Message = "label not found";
                return NotFound(res);
            }

            todos = unitOfWork.Todos.GetAllByUserWithLabelsPaginated(
                userId,
                queryParams,
                [t => t.Entries],
                t => t.LabelData.Any(ld => ld.LabelId == userLabel.Id)
            );
        }
        else
        {
            res.Error = "Unknown type";
            return UnprocessableEntity(res);
        }

        res.IsSuccess = true;
        res.Message = "user todos fetched successfully";
        res.Data = todos;
        return Ok(res);
    }

    [HttpGet("{id}")]
    public ActionResult<GenericResponse<Todo>> GetOneTodo(int id)
    {
        var res = new GenericResponse<Todo>();

        var userId = GetUserId();

        var todo = unitOfWork.Todos.GetWithLabels(id, t => t.Entries);
        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        if (userId != todo.UserId)
        {
            return Forbid();
        }

        res.IsSuccess = true;
        res.Message = "user todo fetched successfully";
        res.Data = todo;
        return Ok(res);
    }

    [HttpPost("{id}/label")]
    public ActionResult<GenericResponse<Todo>> AddLabelToTodo(int id, [FromBody] LabelDTO label)
    {
        var res = new GenericResponse<Todo>();
        var userId = GetUserId();

        var todo = unitOfWork.Todos.GetWithLabels(id);

        if (label.Id is null)
        {
            res.Error = "label id is null";
            return UnprocessableEntity(res);
        }

        var labelToAdd = unitOfWork.Labels.GetOneById((int)label.Id!);

        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        if (labelToAdd is null)
        {
            res.Error = "label not found";
            return UnprocessableEntity(res);
        }

        if (todo.UserId != userId || labelToAdd.UserId != userId)
        {
            return Forbid();
        }

        if (!todo.Labels.Any(l => l.Id == labelToAdd.Id))
        {
            unitOfWork.LabelTodo.Create(new() { LabelId = labelToAdd.Id, TodoId = todo.Id });
            unitOfWork.SaveChanges();
        }
        res.IsSuccess = true;
        res.Data = todo;
        res.Message = "label added to todo successfully";

        return res;
    }

    [HttpDelete("{id}/label/{labelId}")]
    public ActionResult<GenericResponse<Todo>> RemoveLabelFromTodo(int id, int labelId)
    {
        var res = new GenericResponse<Todo>();
        var userId = GetUserId();

        var todo = unitOfWork.Todos.GetWithLabels(id, t => t.Entries);
        var labelToRemove = unitOfWork.Labels.GetOneById(labelId);

        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        if (labelToRemove is null)
        {
            res.Error = "label not found";
            return UnprocessableEntity(res);
        }

        if (todo.UserId != userId || labelToRemove.UserId != userId)
        {
            return Forbid();
        }

        var labelTodoToRemove = unitOfWork.LabelTodo.GetByCompoundKey(labelId, id);

        if (todo.Labels.Any(l => l.Id == labelToRemove.Id))
        {
            unitOfWork.LabelTodo.Delete(labelTodoToRemove);
            unitOfWork.SaveChanges();
        }
        res.IsSuccess = true;
        res.Data = todo;
        res.Message = "label removed from todo successfully";

        return res;
    }

    [HttpPost]
    public ActionResult<GenericResponse<Todo>> CreateTodo([FromBody] TodoDTO todo)
    {
        var res = new GenericResponse<Todo>();

        var userId = GetUserId();
        var newTodo = new Todo
        {
            Title = todo.Title,
            IsFavourite = todo.IsFavourite,
            IsDone = todo.Entries.All(e => e.IsDone),
            UserId = userId,
            Entries = todo.Entries.Select(
                e =>
                    new TodoEntry()
                    {
                        IsDone = e.IsDone,
                        Position = e.Position,
                        Priority = e.Priority,
                        Text = e.Text,
                    }
            )
                .ToList()
        };

        unitOfWork.Todos.Create(newTodo);
        unitOfWork.SaveChanges();

        res.Message = "todo created successfully";
        res.Data = newTodo;
        res.IsSuccess = true;

        return CreatedAtAction(nameof(GetOneTodo), new { id = newTodo.Id }, res);
    }

    [HttpDelete("{id}")]
    public ActionResult<GenericResponse<Todo>> DeleteTodo(int id)
    {
        var res = new GenericResponse<Todo>();
        var userId = GetUserId();

        var todo = unitOfWork.Todos.GetOneById(id, t => t.Entries);
        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        if (userId != todo.UserId)
        {
            return Forbid();
        }

        var labelTodosWithThisLabel = unitOfWork.LabelTodo.GetAll(lt => lt.TodoId == todo.Id);

        unitOfWork.LabelTodo.DeleteRange(labelTodosWithThisLabel);
        unitOfWork.SaveChanges();

        unitOfWork.Todos.Delete(todo);
        unitOfWork.SaveChanges();

        res.Message = "todo deleted successfully";
        res.IsSuccess = true;
        return Ok(res);
    }

    [HttpPut("{id}")]
    public ActionResult<GenericResponse<Todo>> UpdateTodo(int id, [FromBody] TodoDTO newTodo)
    {
        //updates only the title (for now)
        // to update the entries we have separate methods below
        var res = new GenericResponse<Todo>();

        var todo = unitOfWork.Todos.GetOneById(id, t => t.Entries);
        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        var userId = GetUserId();
        if (userId != todo.UserId)
        {
            return Forbid();
        }

        // update todo
        todo.Title = newTodo.Title;
        todo.IsFavourite = newTodo.IsFavourite;

        unitOfWork.Todos.Update(todo);
        unitOfWork.SaveChanges();

        res.IsSuccess = true;
        res.Message = "todo updated successfully";
        res.Data = todo;
        return Ok(res);
    }

    [HttpPost("{id}/Entry")]
    public ActionResult<GenericResponse<TodoEntry>> AddTodoEntry(int id, TodoEntryDTO newEntry)
    {
        var res = new GenericResponse<TodoEntry>();

        var todo = unitOfWork.Todos.GetOneById(id, t => t.Entries);
        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }
        var userId = GetUserId();
        if (userId != todo.UserId)
        {
            return Forbid();
        }

        var entry = new TodoEntry()
        {
            IsDone = newEntry.IsDone,
            Position = newEntry.Position,
            Priority = newEntry.Priority,
            Text = newEntry.Text,
            TodoId = todo.Id
        };

        unitOfWork.TodoEntries.Create(entry);

        var allDone = todo.Entries.All(e => e.IsDone);
        todo.IsDone = allDone;
        unitOfWork.Todos.Update(todo);

        unitOfWork.SaveChanges();

        res.Message = "entry created successfully";
        res.IsSuccess = true;
        res.Data = entry;

        return CreatedAtAction(nameof(GetOneTodo), new { id = entry.Id }, res);
    }

    [HttpPut("{id}/Entry/{entryId}")]
    public ActionResult<GenericResponse<TodoEntry>> UpdateTodoEntry(
        int id,
        int entryId,
        [FromBody] TodoEntryDTO entry
    )
    {
        var res = new GenericResponse<TodoEntry>();

        var todo = unitOfWork.Todos.GetOneById(id, t => t.Entries);
        if (todo is null)
        {
            res.Message = "no todo found with this id";
            return NotFound(res);
        }

        var userId = GetUserId();
        if (userId != todo.UserId)
        {
            return Forbid();
        }

        if (entry.Id is null)
        {
            res.Message = "entry id cannot be null";
            return BadRequest(res);
        }
        var updatedEntry = unitOfWork.TodoEntries.GetOneById(entryId);
        if (todo.Id != updatedEntry.TodoId || entryId != entry.Id)
        {
            res.Message = "this entry does not belong to this todo";
            return BadRequest(res);
        }

        updatedEntry.IsDone = entry.IsDone;
        updatedEntry.Text = entry.Text;
        updatedEntry.Position = entry.Position;
        updatedEntry.Priority = entry.Priority;

        unitOfWork.TodoEntries.Update(updatedEntry);

        var allDone = todo.Entries.All(e => e.IsDone);
        todo.IsDone = allDone;
        unitOfWork.Todos.Update(todo);

        unitOfWork.SaveChanges();

        res.IsSuccess = true;
        res.Message = "entry updated successfully";
        res.Data = updatedEntry;
        return Ok(res);
    }

    [HttpDelete("Entry/{id}")]
    public ActionResult<GenericResponse<Todo>> DeleteTodoEntry(int id)
    {
        var res = new GenericResponse<Todo>();

        var entry = unitOfWork.TodoEntries.GetOneById(id);
        if (entry is null)
        {
            res.Message = "no entry found with this id";
            return NotFound(res);
        }

        var todo = unitOfWork.Todos.GetOneById(entry.TodoId, t => t.Entries);

        if (todo is null)
        {
            res.Message = "no todo found with this enrty";
            return NotFound(res);
        }
        var userId = GetUserId();
        if (userId != todo.UserId)
        {
            return Forbid();
        }

        unitOfWork.TodoEntries.Delete(entry);
        unitOfWork.SaveChanges();

        var allDone = todo.Entries.All(e => e.IsDone);
        todo.IsDone = allDone;
        unitOfWork.Todos.Update(todo);
        unitOfWork.SaveChanges();

        res.Message = "entry deleted successfully";
        res.IsSuccess = true;
        res.Data = todo;
        return Ok(res);
    }
}
