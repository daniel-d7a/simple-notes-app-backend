using todo_app.core.Models.Data;

namespace todo_app.core;

public class LabelNote
{
    public int LabelId { get; set; }
    public int NoteId { get; set; }

    public Label Label { get; set; } = null!;
    public Note Note { get; set; } = null!;
}
