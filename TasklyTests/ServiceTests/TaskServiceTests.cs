using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TasklyAPI.Controllers;
using TasklyAPI.DbContext;
using TasklyAPI.Enums;
using TasklyAPI.Models;
using TasklyAPI.Models.DTOs;
using TasklyAPI.Services;

namespace TasklyTests.ServiceTests
{
	public class TaskServiceTests : IDisposable
	{

		private readonly TaskDbContext _context;
		private readonly TaskService _taskService;
		private readonly TaskController _controller;
		private readonly string _testUserId = "test-user-id";

		public TaskServiceTests()
		{
			var options = new DbContextOptionsBuilder<TaskDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new TaskDbContext(options);
			_taskService = new TaskService(_context);
			_controller = new TaskController(_taskService);
		}

		[Fact]
		public async Task CreateTaskAsync_ShouldCreateTask_WhenValidDto()
		{
			// Arrange
			var createTaskDto = new CreateTaskDTO
			{
				Name = "Test Task",
				Description = "Test Description",
				Priority = TaskPriority.High,
				Deadline = DateTime.UtcNow.AddDays(7)
			};

			// Act
			var result = await _taskService.CreateTaskAsync(createTaskDto, _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(createTaskDto.Name, result.Name);
			Assert.Equal(createTaskDto.Description, result.Description);
			Assert.Equal(createTaskDto.Priority, result.Priority);
			Assert.False(result.IsFinished);
		}

		[Fact]
		public async Task GetUserTasksAsync_ShouldReturnOnlyUserTasks()
		{
			// Arrange
			var user1Tasks = new[]
			{
				new TaskItem { Name = "Task 1", Description = "Task 1 description", UserId = _testUserId, Priority = TaskPriority.Low },
				new TaskItem { Name = "Task 2", Description = "Task 2 description", UserId = _testUserId, Priority = TaskPriority.Low }
			};

			var user2Tasks = new[]
			{
				new TaskItem { Name = "Task 3", Description = "Task 3 description", UserId = "other-user-id", Priority = TaskPriority.High }
			};

			_context.Tasks.AddRange(user1Tasks);
			_context.Tasks.AddRange(user2Tasks);
			await _context.SaveChangesAsync();

			// Act
			var result = await _taskService.GetUserTasksAsync(_testUserId);

			// Assert
			Assert.Equal(2, result.Count());
			Assert.All(result, task => Assert.Contains(task.Name, new[] { "Task 1", "Task 2" }));
		}

		[Fact]
		public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
		{
			// Arrange
			var task = new TaskItem
			{
				Name = "Original Task",
				Description = "Original description",
				Priority = TaskPriority.Low,
				Deadline = DateTime.UtcNow.AddDays(1),
				UserId = _testUserId
			};

			_context.Tasks.Add(task);
			await _context.SaveChangesAsync();

			var updateDto = new TaskDTO
			{
				Id = task.Id,
				Name = "Updated Task",
				Description = "Updated description",
				Priority = TaskPriority.High,
				Deadline = DateTime.UtcNow.AddDays(2),
				IsFinished = true,
				CreatedAt = task.CreatedAt
			};

			// Act
			var result = await _taskService.UpdateTaskAsync(task.Id, updateDto, _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Updated Task", result.Name);
			Assert.Equal("Updated description", result.Description);
			Assert.Equal(TaskPriority.High, result.Priority);
			Assert.True(result.IsFinished);
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
