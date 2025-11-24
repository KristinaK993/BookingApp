namespace Application.Dtos;

public class TodoItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
}
