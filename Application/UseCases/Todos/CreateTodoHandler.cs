using Application.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases.Todos;

public class CreateTodoHandler
{
    private readonly IGenericRepository<TodoItem> _todoRepository;

    public CreateTodoHandler(IGenericRepository<TodoItem> todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItemDto> HandleAsync(CreateTodoCommand command)
    {
        var todo = new TodoItem
        {
            Title = command.Title,
            IsCompleted = false
        };

        var created = await _todoRepository.AddAsync(todo);

        return new TodoItemDto
        {
            Id = created.Id,
            Title = created.Title,
            IsCompleted = created.IsCompleted
        };
    }
}
