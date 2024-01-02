using Microsoft.AspNetCore.Mvc;
using todo_app.core.Repositories;
using todo_app.core.DTOs;
using todo_app.core.Models.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using todo_app.core.Models.ResponseModels.General;
using todo_app.core.Helpers.QueryParams;
using todo_app.core.Helpers.Pagination;
using System.Text.Json;

namespace todo_app.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController(IUnitOfWork _unitOfWork
        , ILogger<NotesController> _logger
        ) : ControllerBase 
    {

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetOneNote(int id)
        {
            var res = new GenericResponse<Note>();

            var userId = getUserId();
            if(userId is null)
            {
                res.Error = "user id is null";
                return Unauthorized(res);
            }

            var note = await NoteExisits(id);
            if (note is null)
            {
                res.Message = "no notes found with this id";
                return NotFound(res);
            }

            if(userId != note.UserId)
            {
                return Forbid();
            }

            res.IsSuccess = true;
            res.Data = note;
            res.Message = "note fetched successfully";
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Note>>> GetAllUserNotesPaginated([FromQuery] BaseQueryParams queryParams)
        {
            var res = new GenericResponse<PaginatedResult<Note>>();

            var userId = getUserId();

            if(userId is null)
            {
                res.Error = "user id is null";
                return Unauthorized(res);
            }

            var notes = await _unitOfWork.Notes.GetAllByUserPaginatedAsync(userId, queryParams);
            Console.WriteLine(JsonSerializer.Serialize(notes));
            res.IsSuccess = true;
            res.Message = "user notes fetched successfully";
            res.Data = notes;

            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] NoteDTO noteDto)
        {
            var res = new GenericResponse<Note>();
            var userId = getUserId();
            Note newNote = new()
            {
                Title = noteDto.Title,
                Body = noteDto.Body,
                UserId = userId,
            };

            _unitOfWork.Notes.Create(newNote);
            await _unitOfWork.SaveChangesAsync();

            res.IsSuccess = true;
            res.Data = newNote;
            res.Message = "Note created successfully";

            return CreatedAtAction(nameof(GetOneNote), new { id = newNote.Id}, res);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> UpdateNote(int id, [FromBody]NoteDTO newNote)
        {
            var res = new GenericResponse<Note>();

            var userId = getUserId();
            if(userId is null)
            {
                res.Error = "user id is null";
                return Unauthorized(res);
            }

            var note = await NoteExisits(id);
            if (note is null)
            {
                res.Message = "no note found with this id";
                return NotFound(res);
            }

            if(userId != note.UserId)
            {
                return Forbid();
            }

            note.Title = newNote.Title;
            note.Body = newNote.Body;

            _unitOfWork.Notes.Update(note);
            await _unitOfWork.SaveChangesAsync();

            res.IsSuccess = true;
            res.Message = "note updates successfully";
            res.Data = note;

            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Note>> DeleteNote(int id)
        {
            var res = new GenericResponse<Note>();

            var userId = getUserId();
            if(userId is null)
            {
                res.Error = "user id is null";
                return Unauthorized(res);
            }

            var note = await NoteExisits(id);
            if(note is null)
            {
                res.Message = "no note found with this id";
                return NotFound(res);
            }

            if(userId != note.UserId)
            {
                return Forbid();
            }
            _unitOfWork.Notes.Delete(note);
            await _unitOfWork.SaveChangesAsync();

            res.IsSuccess = true;
            res.Message = "note deleted successfully";

            return Ok(res);
        }

        private async Task<Note?> NoteExisits(int id)
        {
            return await _unitOfWork.Notes.GetOneByIdAsync(id);
        }

        private string getUserId()
        {

            var userId = User.FindFirstValue("uid");
            //Console.WriteLine($"user id => {userId}");
            return userId;
        }

        //private async Task<GenericResponse<Note>> AuthorizeNoteAccess(int id)
        //{
        //    var res = new GenericResponse<Note>();

        //    var userId = getUserId();
        //    if(userId is null)
        //    {
        //        res.Error = "user id is null";
        //        return res;
        //    }

        //    var note = await NoteExisits(id);
        //    if(note is null)
        //    {
        //        res.Message = "no note found with this id";
        //        return res;
        //    }

        //    if(userId != note.UserId)
        //    {
        //        return res;
        //    }

        //    res.IsSuccess = true;
        //    return res;
        //}
    }
}
