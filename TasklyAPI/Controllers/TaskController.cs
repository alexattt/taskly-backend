using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TasklyAPI.Models;
using TasklyAPI.Models.DTOs;
using TasklyAPI.Services;

namespace TasklyAPI.Controllers
{
	[ApiController]
	[Route("api/task")]
	[Authorize]
	public class TaskController : ControllerBase
	{
		private readonly TaskService _taskService;

		public TaskController(TaskService taskService)
		{
			_taskService = taskService;
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}

		/// <summary>
		/// Retrieves tasks of user
		/// </summary>
		/// <returns>List of tasks</returns>
		[SwaggerResponse(200, "List of tasks retrieved successfully", typeof(IEnumerable<TaskDTO>))]
		[SwaggerResponse(400, "Error retrieving list of tasks")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasks()
		{
			try
			{
				var userId = GetUserId();
				var tasks = await _taskService.GetUserTasksAsync(userId);

				return Ok(tasks);
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}

		/// <summary>
		/// Retrieves task by its id
		/// </summary>
		/// <param name="id">Task ID</param>
		/// <returns>Single task</returns>
		[SwaggerResponse(200, "List of tasks retrieved successfully", typeof(TaskDTO))]
		[SwaggerResponse(404, "Task with specified ID not found")]
		[HttpGet("{id}")]
		public async Task<ActionResult<TaskDTO>> GetTask(int id)
		{
			var userId = GetUserId();
			var task = await _taskService.GetTaskByIdAsync(id, userId);

			if (task == null)
			{
				return NotFound();
			}

			return Ok(task);
		}

		/// <summary>
		/// Creates new task
		/// </summary>
		/// <param name="createTaskDto">DTO for new task</param>
		/// <returns>Created task object</returns>
		[SwaggerResponse(201, "Task created successfully", typeof(TaskDTO))]
		[SwaggerResponse(400, "TaskDTO validation failed")]
		[HttpPost]
		public async Task<ActionResult<TaskDTO>> CreateTask([FromBody] CreateTaskDTO createTaskDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userId = GetUserId();
			var task = await _taskService.CreateTaskAsync(createTaskDto, userId);

			return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
		}

		/// <summary>
		/// Updates existing task
		/// </summary>
		/// <param name="taskDto">Updated DTO of a task</param>
		/// <returns>Updated task</returns>
		[SwaggerResponse(200, "Task updated successfully", typeof(TaskDTO))]
		[SwaggerResponse(400, "TaskDTO validation failed")]
		[SwaggerResponse(404, "Task not found")]
		[HttpPut("{id}")]
		public async Task<ActionResult<TaskDTO>> UpdateTask([FromBody] TaskDTO taskDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userId = GetUserId();
			var updatedTask = await _taskService.UpdateTaskAsync(taskDto.Id, taskDto, userId);

			if (updatedTask == null)
			{
				return NotFound();
			}

			return Ok(updatedTask);
		}

		/// <summary>
		/// Delete existing task
		/// </summary>
		/// <param name="id">ID of a task that should be deleted</param>
		[SwaggerResponse(204, "Task deleted successfully")]
		[SwaggerResponse(404, "Task not found")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			var userId = GetUserId();
			var result = await _taskService.DeleteTaskAsync(id, userId);

			if (!result)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}
