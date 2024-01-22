using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_app.core.Models.Data;

public class Todo : BaseModel
{
    [MaxLength(100)]
    public string? Title { get; set; }
    public bool IsDone { get; set; }
    public ICollection<TodoEntry> Entries { get; set; } = [];

    public ICollection<LabelTodo> LabelData { get; set; } = [];

    [NotMapped]
    public ICollection<Label> Labels => LabelData.Select(ln => ln.Label).ToList();
}
