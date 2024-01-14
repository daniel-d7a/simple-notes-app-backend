using System.ComponentModel.DataAnnotations;

namespace todo_app.core.Models.Data;

public class Note : BaseModel
{
    [MaxLength(100)]
    public string? Title { get; set; }
    public string? Body { get; set; }
}
