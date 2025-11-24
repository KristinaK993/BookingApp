using Application.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases.Todos;

public class GetAllTodosHandler
{
    private readonly IGenericRepository<TodoItem> _todoRepository;

    public GetAllTodosHandler(IGenericRepository<TodoItem> todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IReadOnlyList<TodoItemDto>> HandleAsync(GetAllTodosQuery query)
    {
        var todos = await _todoRepository.ListAsync();

        return todos.Select(t => new TodoItemDto
        {
            Id = t.Id,
            Title = t.Title,
            IsCompleted = t.IsCompleted
        }).ToList();
    }
}
