using TaskTrackerApi.Enums;

namespace TaskTrackerApi.Models.Entities
{

	public class UserTask
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
		public ApplicationUser User { get; set; } = null!;

		public required string Title { get; set; }

        public string? Description { get; set; }

        public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } 

    }

}
