using System.ComponentModel.DataAnnotations;

namespace todo_app.core.Models.Data;

public class TodoEntry
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Text { get; set; }
    public bool IsDone { get; set; }

    [Range(0, 5)]
    public int Priority { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int Position { get; set; } = 0;
    public int TodoId { get; set; }
    public Todo Todo { get; set; }
}
