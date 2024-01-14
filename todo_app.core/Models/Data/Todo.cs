using System.ComponentModel.DataAnnotations;

namespace todo_app.core.Models.Data;

public class Todo : BaseModel
{
    [MaxLength(100)]
    public string? Title { get; set; }
    public bool IsDone { get; set; }
    public ICollection<TodoEntry> Entries { get; set; }
}
