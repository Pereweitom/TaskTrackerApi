using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.Enums;
using TaskTrackerApi.Models.Dtos;
using TaskTrackerApi.Models.Entities;
using TaskTrackerApi.Responses;

namespace TaskTrackerApi.Controllers
{	
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public UserTasksController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

		private static string GetStatusText(UserTaskStatus status)
		{
			return status switch
			{
				UserTaskStatus.Todo => "todo",
				UserTaskStatus.InProgress => "in-progress",
				UserTaskStatus.Done => "done",
				_ => "Unknown"
			};
		}

		[Authorize]
		[HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}

			var allTasks = await dbContext.UserTasks.Where(t => t.UserId == userId).Select(t => new UserTaskDto { 
				Id = t.Id,
				Title = t.Title,
				UserId=t.UserId,
				Description = t.Description,
				Status = GetStatusText(t.Status),
				CreatedAt = t.CreatedAt,
				UpdatedAt = t.UpdatedAt,
			}).ToListAsync();

           return Ok( new ApiResponse<object>
		   {
			   Status="00",
			   Message="Tasks retrieved successfully",
			   Data=allTasks
		   });
        }
		[Authorize]
		[HttpGet("GetTasksByStatus")]
		public async Task<IActionResult> GetTasksByStatus(string status)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}

			UserTaskStatus taskStatus;

			switch (status.ToLower())
			{
				case "todo":
					taskStatus = UserTaskStatus.Todo;
					break;

				case "in-progress":
					taskStatus = UserTaskStatus.InProgress;
					break;

				case "done":
					taskStatus = UserTaskStatus.Done;
					break;

				default:
					return BadRequest(new ApiResponse<object>
					{ 
					Status = "99",
					Message = "Invalid status. Use: todo, in-progress, or done.",
					Data = null,
					}
					
					);
			}

			var tasks = await dbContext.UserTasks.Where(t => t.Status == taskStatus && t.UserId == userId).Select(t => new UserTaskDto
			{
				Id = t.Id,
				Title = t.Title,
				UserId= t.UserId,
				Description = t.Description,
				Status = GetStatusText(t.Status),
				CreatedAt = t.CreatedAt,
				UpdatedAt = t.UpdatedAt,
			}).ToListAsync();

			return Ok(new ApiResponse<object>
			{
				Status="00",
				Message="Tasks retrieved successfully",
				Data = tasks
			});
			}
		[Authorize]
		[HttpPost("AddTask")]
		public async Task<IActionResult> AddTask(AddTaskDto request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}

			var userTaskEntity = new UserTask
			{
				Title = request.Title,
				Description = request.Description,
				UserId = userId,
				Status = UserTaskStatus.Todo,
				CreatedAt = DateTime.UtcNow,
			
			};

			dbContext.UserTasks.Add(userTaskEntity);
			await dbContext.SaveChangesAsync();

			var response = new UserTaskDto
			{
				Id = userTaskEntity.Id,
				Title = userTaskEntity.Title,
				UserId = userTaskEntity.UserId,
				Description = userTaskEntity.Description,
				Status = GetStatusText(userTaskEntity.Status),
				CreatedAt = userTaskEntity.CreatedAt,
				UpdatedAt = userTaskEntity.UpdatedAt
			};

			return Ok(new ApiResponse<UserTaskDto>
			{
				Status = "00",
				Message = "Task Successfully Added",
				Data = response
			});
		}

		[Authorize]
		[HttpPut("UpdateTask")]
		public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto request)

		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}

			var task = dbContext.UserTasks.Find(id);

			if (task is null)
			{
				return NotFound(new ApiResponse<object> 
				{ 
				Status = "99",
				Data= null,
				Message= "Not Found"
				});
			}

			task.Title = request.Title;
			task.Description = request.Description;
			task.UpdatedAt = DateTime.Now;

			await dbContext.SaveChangesAsync();

			var response = new UserTaskDto
			{
				Id = task.Id,
				Title = task.Title,
				UserId = userId!,
				Description = task.Description,
				Status = GetStatusText(task.Status),
				CreatedAt = task.CreatedAt,
				UpdatedAt = task.UpdatedAt

			};
			return Ok(new ApiResponse<object>
			{
				Status= "00",
				Message="Task Successfully updated",
				Data=response
			});



		}

		[Authorize]
		[HttpPatch("UpdateTaskStatus")]
		public async Task<IActionResult> UpdateTaskStatus(int id, string status)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}

			var task = dbContext.UserTasks.Find(id);

			if (task is null)
			{
				return NotFound(new ApiResponse<object>
				{
					Status = "99",
					Data = null,
					Message = "Not Found"
				});
			}

			UserTaskStatus taskStatus;

			switch (status.ToLower())
			{
				
				case "in-progress":
					taskStatus = UserTaskStatus.InProgress;
					break;

				case "done":
					taskStatus = UserTaskStatus.Done;
					break;

				default:
					return BadRequest(new ApiResponse<object>
					{
						Status = "99",
						Message = "Invalid status. Use: in-progress, or done.",
						Data = null,
					}

					);
			}
			task.Status = taskStatus;

			await dbContext.SaveChangesAsync();

			var response = new UserTaskDto
			{
				Id = task.Id,
				Title = task.Title,
				UserId=userId!,
				Description = task.Description,
				Status = GetStatusText(task.Status),
				CreatedAt = task.CreatedAt,
				UpdatedAt = task.UpdatedAt

			};
			return Ok(new ApiResponse<object>
			{
				Status="00",
				Message="Status Updated Successfully",
				Data = response
			});
		}

		[Authorize]
		[HttpDelete("DeleteTask")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<object>
				{
					Status = "99",
					Message = "User is not authenticated",
					Data = null
				});
			}
			var task = dbContext.UserTasks.Find(id);

			if (task is null)
			{
				return NotFound(new ApiResponse<object>
				{
					Status = "99",
					Message="Not found",
				});
			}
			dbContext.Remove(task);
			dbContext.SaveChanges();


			return Ok(new ApiResponse<object>
			{
				Status="00",
				Message="Task deleted successfully"
			});
		}

	}
}
