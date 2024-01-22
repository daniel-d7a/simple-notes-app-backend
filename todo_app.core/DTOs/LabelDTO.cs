using System.ComponentModel.DataAnnotations;

namespace todo_app.core;

public class LabelDTO
{
    public int? Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = "";
}
