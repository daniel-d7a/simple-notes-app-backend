namespace todo_app.core.Models.ResponseModels.General;

public class GenericResponse<T>
    where T : class
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }
}
