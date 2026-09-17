
using TaskTrackerApi.Enums;

namespace TaskTrackerApi.Models.Dtos
{
    public class UserTaskDto
    {
		public int Id { get; set; }
		public required string Title { get; set; }

		public string? Description { get; set; }

		public string UserId { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public DateTime? UpdatedAt { get; set; }
	}

	
}
