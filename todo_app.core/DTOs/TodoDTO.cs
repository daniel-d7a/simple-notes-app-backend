using System.ComponentModel.DataAnnotations;

namespace todo_app.core.DTOs;

public class TodoDTO
{
    [MaxLength(100)]
    public string Title { get; set; }
    public bool IsFavourite { get; set; }
    public ICollection<TodoEntryDTO> Entries { get; set; }
    public string UserId { get; set; }
    public bool IsDone { get; set; }
}
