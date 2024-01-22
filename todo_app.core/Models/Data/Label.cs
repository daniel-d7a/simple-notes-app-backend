using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using todo_app.core.Models.Auth;
using todo_app.core.Models.Data;

namespace todo_app.core;

public class Label
{
    public int Id { get; set; }

    [MaxLength(100)]
    public ICollection<LabelNote> LabelNotes { get; set; } = [];
    public ICollection<LabelTodo> LabelTodos { get; set; } = [];
    public string Name { get; set; } = "";
    public string UserId { get; set; } = "";
    public UserModel User { get; set; } = null!;
}
