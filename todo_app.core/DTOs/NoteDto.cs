using System.ComponentModel.DataAnnotations;

namespace todo_app.core.DTOs;

public class NoteDTO
{
    [MaxLength(100)]
    public string? Title { get; set; }

    public string? Body { get; set; }

    public bool IsFavourite { get; set; }

    [Required]
    public string UserId { get; set; }
}
