using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TasksController : ControllerBase
{
    private readonly TasksRepository _repo;

    public TasksController(TasksRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var tasks = _repo.GetAllTasks();
        return Ok(tasks);
    }
}