using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todo_app.api.Controllers;
using todo_app.core;
using todo_app.core.Models.ResponseModels.General;
using todo_app.core.Repositories;

namespace todo_app.api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LabelController(IUnitOfWork unitOfWork) : SharedController
{
    [HttpGet]
    public ActionResult<GenericResponse<IEnumerable<Label>>> GetUserLabels()
    {
        GenericResponse<IEnumerable<Label>> res = new();
        var userId = GetUserId();
        var labels = unitOfWork.Labels.GetAllByUser(userId, null, l => l.Name);

        res.IsSuccess = true;
        res.Data = labels;
        res.Message = "user labels fetched successfully";

        return res;
    }

    [HttpPost]
    public ActionResult<GenericResponse<Label>> CreateLabel([FromBody] LabelDTO newLabel)
    {
        GenericResponse<Label> res = new();
        var userId = GetUserId();

        var userLabels = unitOfWork.Labels.GetAllByUser(userId);
        if (userLabels.Any(l => l.Name == newLabel.Name))
        {
            res.Error = "label name already exists";
            return UnprocessableEntity(res);
        }
        Label label = new() { Name = newLabel.Name, UserId = userId };

        unitOfWork.Labels.Create(label);
        unitOfWork.SaveChanges();

        res.Data = label;
        res.IsSuccess = true;
        res.Message = "label created successfully";

        return CreatedAtAction(nameof(GetUserLabels), new { id = label.Id }, res);
    }

    [HttpPut("{id}")]
    public ActionResult<GenericResponse<Label>> UpdateLabel(int id, [FromBody] LabelDTO newLabel)
    {
        GenericResponse<Label> res = new();
        var userId = GetUserId();

        var label = unitOfWork.Labels.GetOneById(id);

        if (label is null)
        {
            res.Message = "no label was found with this id";
            return NotFound(res);
        }

        if (label.UserId != userId)
        {
            return Forbid();
        }

        var userLabels = unitOfWork.Labels.GetAllByUser(userId);
        if (userLabels.Any(l => l.Name == newLabel.Name))
        {
            res.Error = "label name already exists";
            return UnprocessableEntity(res);
        }

        label.Name = newLabel.Name;

        unitOfWork.Labels.Update(label);
        unitOfWork.SaveChanges();

        res.Data = label;
        res.Message = "label updated successfully";
        res.IsSuccess = true;

        return Ok(res);
    }

    [HttpDelete("{id}")]
    public ActionResult<GenericResponse<Label>> DeleteLabel(int id)
    {
        GenericResponse<Label> res = new();
        var userId = GetUserId();

        var label = unitOfWork.Labels.GetOneById(id);

        if (label is null)
        {
            res.Message = "no label was found with this id";
            return NotFound(res);
        }

        if (label.UserId != userId)
        {
            return Forbid();
        }

        var labelNotesWithThisLabel = unitOfWork.LabelNote.GetAll(ln => ln.LabelId == label.Id);

        unitOfWork.LabelNote.DeleteRange(labelNotesWithThisLabel);
        unitOfWork.SaveChanges();

        var labelsTodosWithThisLabel = unitOfWork.LabelTodo.GetAll(lt => lt.LabelId == label.Id);

        unitOfWork.LabelTodo.DeleteRange(labelsTodosWithThisLabel);
        unitOfWork.SaveChanges();

        unitOfWork.Labels.Delete(label);
        unitOfWork.SaveChanges();

        res.Message = "label deleted successfully";
        res.IsSuccess = true;

        return res;
    }
}
