using Domain.Common;

namespace Domain.Entities;

public class TodoItem : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
}
