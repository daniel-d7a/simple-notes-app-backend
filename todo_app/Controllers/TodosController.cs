using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        [FromQuery] BaseQueryParams queryParams
    )
    {
        var res = new GenericResponse<PaginatedResult<Todo>>();
        var userId = GetUserId();
        PaginatedResult<Todo> todos;

        if (string.IsNullOrEmpty(queryParams.Type))
        {
            todos = unitOfWork.Todos.GetAllByUserPaginated(userId, queryParams);
        }
        else if (queryParams.Type != Types.Favourite)
        {
            res.Error = "Unknown type";
            return NotFound(res);
        }
        else
        {
            todos = unitOfWork.Todos.GetAllByUserPaginated(userId, queryParams, n => n.IsFavourite);
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

        res.IsSuccess = true;
        res.Message = "user todo fetched successfully";
        res.Data = todo;
        return Ok(res);
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

        return CreatedAtAction(nameof(CreateTodo), new { id = newTodo.Id }, res);
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
