using Application.UseCases.Todos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly CreateTodoHandler _createTodoHandler;
    private readonly GetAllTodosHandler _getAllTodosHandler;

    public TodoController(
        CreateTodoHandler createTodoHandler,
        GetAllTodosHandler getAllTodosHandler)
    {
        _createTodoHandler = createTodoHandler;
        _getAllTodosHandler = getAllTodosHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoCommand command)
    {
        var result = await _createTodoHandler.HandleAsync(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getAllTodosHandler.HandleAsync(new GetAllTodosQuery());
        return Ok(result);
    }
}
