using todo_app.core.Models.Data;

namespace todo_app.core;

public class LabelTodo
{
    public int LabelId { get; set; }
    public int TodoId { get; set; }

    public Label Label { get; set; } = null!;
    public Todo Todo { get; set; } = null!;
}
