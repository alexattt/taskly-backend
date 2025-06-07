using Microsoft.EntityFrameworkCore;
using TasklyAPI.DbContext;
using TasklyAPI.Models.DTOs;

namespace TasklyAPI.Services
{
	public class TaskService
	{
		private readonly TaskDbContext _context;

		public TaskService(TaskDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<TaskDTO>> GetUserTasksAsync(string userId)
		{
			var tasks = await _context.Tasks
				.Where(t => t.UserId == userId)
				.OrderBy(t => t.Deadline)
				.OrderBy(t => t.IsFinished)
				.ToListAsync();

			return tasks.Select(MapToDto);
		}

		public async Task<TaskDTO?> GetTaskByIdAsync(int id, string userId)
		{
			var task = await _context.Tasks
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

			return task != null ? MapToDto(task) : null;
		}

		public async Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTaskDto, string userId)
		{
			var task = new Models.TaskItem
			{
				Name = createTaskDto.Name,
				Description = createTaskDto.Description,
				Priority = createTaskDto.Priority,
				Deadline = createTaskDto.Deadline,
				UserId = userId,
				CreatedAt = DateTime.UtcNow
			};

			_context.Tasks.Add(task);
			await _context.SaveChangesAsync();

			return MapToDto(task);
		}
		public async Task<TaskDTO?> UpdateTaskAsync(int id, TaskDTO taskDto, string userId)
		{
			var task = await _context.Tasks
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

			if (task == null)
			{
				return null;
			}

			task.Name = taskDto.Name;
			task.Description = taskDto.Description;
			task.Priority = taskDto.Priority;
			task.Deadline = taskDto.Deadline;
			task.IsFinished = taskDto.IsFinished;

			await _context.SaveChangesAsync();

			return MapToDto(task);
		}

		public async Task<bool> DeleteTaskAsync(int id, string userId)
		{
			var task = await _context.Tasks
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

			if (task == null)
			{
				return false;
			}

			_context.Tasks.Remove(task);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> ToggleTaskCompletionAsync(int id, string userId)
		{
			var task = await _context.Tasks
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

			if (task == null)
			{
				return false;
			}

			task.IsFinished = !task.IsFinished;
			await _context.SaveChangesAsync();

			return true;
		}

		private static TaskDTO MapToDto(Models.TaskItem task)
		{
			return new TaskDTO
			{
				Id = task.Id,
				Name = task.Name,
				Description = task.Description,
				Priority = task.Priority,
				Deadline = task.Deadline,
				IsFinished = task.IsFinished,
				CreatedAt = task.CreatedAt
			};
		}
	}
}
