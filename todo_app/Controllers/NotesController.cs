using Microsoft.AspNetCore.Mvc;
using todo_app.core.Models;
using todo_app.core.Repositories;
using todo_app.core.DTOs;

namespace todo_app.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase 
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetOneNote(int id)
        {
            var note = await NoteExisits(id);
            if (note is null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            //return BadRequest();
            return Ok(await _unitOfWork.Notes.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] NoteDto noteDto)
        {
            Note newNote = new()
            {
                Title = noteDto.Title,
                Body = noteDto.Body,
            };

            _unitOfWork.Notes.Create(newNote);
            await _unitOfWork.SaveChanges();

            return CreatedAtAction(nameof(GetOneNote), new { id = newNote.Id} ,newNote);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> UpdateNote(int id, [FromBody]Note newNote)
        {
            var note = await NoteExisits(id);
            if (note is null)
            {
                return NotFound();
            }
            note.Title = newNote.Title;
            note.Body = newNote.Body;
            _unitOfWork.Notes.Update(note); 
            await _unitOfWork.SaveChanges();

            return Ok(note);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Note>> DeleteNote(int id)
        {
            var note = await NoteExisits(id);
            if(note is null)
            {
                return NotFound();
            }
            _unitOfWork.Notes.Delete(note);
            await _unitOfWork.SaveChanges();

            return Ok(note);
        }

        private async Task<Note?> NoteExisits(int id)
        {
            return await _unitOfWork.Notes.GetOneByIdAsync(id);
        }
    }
}
