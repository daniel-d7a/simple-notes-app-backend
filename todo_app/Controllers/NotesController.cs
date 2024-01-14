using FluentValidation;
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

namespace todo_app.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController(IUnitOfWork unitOfWork, IValidator<NoteDTO> validator)
        : SharedController
    {
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
            unitOfWork.Notes.Delete(note);
            unitOfWork.SaveChanges();

            res.IsSuccess = true;
            res.Message = "note deleted successfully";

            return Ok(res);
        }

        [HttpGet]
        public ActionResult<GenericResponse<PaginatedResult<Note>>> GetAllUserNotesPaginated(
            [FromQuery] BaseQueryParams queryParams
        )
        {
            var res = new GenericResponse<PaginatedResult<Note>>();

            var userId = GetUserId();
            PaginatedResult<Note> notes;
            if (string.IsNullOrEmpty(queryParams.Type))
            {
                notes = unitOfWork.Notes.GetAllByUserPaginated(userId, queryParams);
            }
            else if (queryParams.Type != Types.Favourite)
            {
                res.Error = "Unknown type";
                return NotFound(res);
            }
            else
            {
                notes = unitOfWork.Notes.GetAllByUserPaginated(
                    userId,
                    queryParams,
                    n => n.IsFavourite
                );
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

            var note = unitOfWork.Notes.GetOneById(id);
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
    }
}
