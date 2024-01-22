using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_app.core.Models.Data;

public class Note : BaseModel
{
    [MaxLength(100)]
    public string? Title { get; set; }
    public string? Body { get; set; }
    public ICollection<LabelNote> LabelData { get; set; } = [];

    [NotMapped]
    public ICollection<Label> Labels => LabelData.Select(ln => ln.Label).ToList();
}
