using FluentValidation;
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
public class NotesController(IUnitOfWork unitOfWork, IValidator<NoteDTO> validator)
    : SharedController
{
    [HttpGet]
    public ActionResult<GenericResponse<PaginatedResult<Note>>> GetAllUserNotesPaginated(
        [FromQuery] BaseQueryParams queryParams,
        string? type,
        string? label
    )
    {
        var res = new GenericResponse<PaginatedResult<Note>>();

        var userId = GetUserId();
        PaginatedResult<Note> notes;
        if (string.IsNullOrEmpty(type) && string.IsNullOrEmpty(label))
        {
            notes = unitOfWork.Notes.GetAllByUserWithLabelsPaginated(userId, queryParams);
        }
        else if (!string.IsNullOrEmpty(type))
        {
            switch (type)
            {
                case Types.Favourite:
                    notes = unitOfWork.Notes.GetAllByUserWithLabelsPaginated(
                        userId,
                        queryParams,
                        null,
                        n => n.IsFavourite
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

            notes = unitOfWork.Notes.GetAllByUserWithLabelsPaginated(
                userId,
                queryParams,
                null,
                n => n.LabelData.Any(ld => ld.LabelId == userLabel.Id)
            );
        }
        else
        {
            res.Error = "Unknown type";
            return UnprocessableEntity(res);
        }

        res.IsSuccess = true;
        res.Message = "user notes fetched successfully";
        res.Data = notes;

        return Ok(res);
    }

    [HttpGet("{id}")]
    public ActionResult<GenericResponse<Note>> GetOneNote(int id)
    {
        var res = new GenericResponse<Note>();

        var userId = GetUserId();

        var note = unitOfWork.Notes.GetWithLabels(id);
        if (note is null)
        {
            res.Message = "no notes found with this id";
            return NotFound(res);
        }

        if (userId != note.UserId)
        {
            return Forbid();
        }

        res.IsSuccess = true;
        res.Data = note;
        res.Message = "note fetched successfully";
        return Ok(res);
    }

    [HttpPost]
    public ActionResult<GenericResponse<Note>> CreateNote([FromBody] NoteDTO noteDto)
    {
        var res = new GenericResponse<Note>();
        var result = validator.Validate(noteDto);
        if (!result.IsValid)
        {
            res.Error = String.Join(
                ", ",
                result.Errors.Select(e => e.ErrorMessage).Distinct().ToArray()
            );
            return BadRequest(res);
        }

        var userId = GetUserId();
        Note newNote =
            new()
            {
                IsFavourite = noteDto.IsFavourite,
                Title = noteDto.Title,
                Body = noteDto.Body,
                UserId = userId,
            };

        unitOfWork.Notes.Create(newNote);
        unitOfWork.SaveChanges();

        res.IsSuccess = true;
        res.Data = newNote;
        res.Message = "Note created successfully";

        return CreatedAtAction(nameof(GetOneNote), new { id = newNote.Id }, res);
    }

    [HttpDelete("{id}")]
    public ActionResult<GenericResponse<Note>> DeleteNote(int id)
    {
        var res = new GenericResponse<Note>();
        var userId = GetUserId();

        var note = unitOfWork.Notes.GetOneById(id);
        if (note is null)
        {
            res.Message = "no note found with this id";
            return NotFound(res);
        }

        if (userId != note.UserId)
        {
            return Forbid();
        }

        var labelNotesWithThisNote = unitOfWork.LabelNote.GetAll(ln => ln.NoteId == note.Id);

        unitOfWork.LabelNote.DeleteRange(labelNotesWithThisNote);
        unitOfWork.SaveChanges();

        unitOfWork.Notes.Delete(note);
        unitOfWork.SaveChanges();

        res.IsSuccess = true;
        res.Message = "note deleted successfully";

        return Ok(res);
    }

    [HttpPut("{id}")]
    public ActionResult<GenericResponse<Note>> UpdateNote(int id, [FromBody] NoteDTO newNote)
    {
        var res = new GenericResponse<Note>();
        var result = validator.Validate(newNote);
        if (!result.IsValid)
        {
            res.Error = String.Join(
                ", ",
                result.Errors.Select(e => e.ErrorMessage).Distinct().ToArray()
            );
            return BadRequest(res);
        }

        var userId = GetUserId();

        var note = unitOfWork.Notes.GetOneById(id);
        if (note is null)
        {
            res.Message = "no note found with this id";
            return NotFound(res);
        }

        if (userId != note.UserId)
        {
            return Forbid();
        }

        note.Title = newNote.Title;
        note.Body = newNote.Body;
        note.IsFavourite = newNote.IsFavourite;

        unitOfWork.Notes.Update(note);
        unitOfWork.SaveChanges();

        res.IsSuccess = true;
        res.Message = "note updates successfully";
        res.Data = note;

        return Ok(res);
    }

    [HttpPost("{id}/label")]
    public ActionResult<GenericResponse<Note>> AddLabelToNote(int id, LabelDTO label)
    {
        var res = new GenericResponse<Note>();
        var userId = GetUserId();

        var note = unitOfWork.Notes.GetWithLabels(id);

        if (label.Id is null)
        {
            res.Error = "label id is null";
            return UnprocessableEntity(res);
        }
        var labelToAdd = unitOfWork.Labels.GetOneById((int)label.Id!);

        if (note is null)
        {
            res.Message = "no note found with this id";
            return NotFound(res);
        }

        if (labelToAdd is null)
        {
            res.Error = "label not found";
            return UnprocessableEntity(res);
        }

        if (note.UserId != userId || labelToAdd.UserId != userId)
        {
            return Forbid();
        }

        if (!note.Labels.Any(l => l.Id == labelToAdd.Id))
        {
            unitOfWork.LabelNote.Create(new() { NoteId = id, LabelId = labelToAdd.Id });
            unitOfWork.SaveChanges();
        }
        res.IsSuccess = true;
        res.Data = note;
        res.Message = "label added to note successfully";

        return res;
    }

    [HttpDelete("{id}/label/{labelId}")]
    public ActionResult<GenericResponse<Note>> RemoveLabelFromNote(int id, int labelId)
    {
        var res = new GenericResponse<Note>();
        var userId = GetUserId();

        var note = unitOfWork.Notes.GetWithLabels(id);
        var labelToRemove = unitOfWork.Labels.GetOneById(labelId);

        if (note is null)
        {
            res.Message = "no note found with this id";
            return NotFound(res);
        }

        if (labelToRemove is null)
        {
            res.Error = "label not found";
            return UnprocessableEntity(res);
        }

        if (note.UserId != userId || labelToRemove.UserId != userId)
        {
            return Forbid();
        }

        var labelNoteToRemove = unitOfWork.LabelNote.GetByCompoundKey(labelId, id);

        if (note.Labels.Any(l => l.Id == labelToRemove.Id))
        {
            unitOfWork.LabelNote.Delete(labelNoteToRemove);
            unitOfWork.SaveChanges();
        }
        res.IsSuccess = true;
        res.Data = note;
        res.Message = "label removed from note successfully";

        return res;
    }
}
