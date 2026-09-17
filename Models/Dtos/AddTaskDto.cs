using TaskTrackerApi.Enums;

namespace TaskTrackerApi.Models.Dtos
{
    public class AddTaskDto
    {
		public required string Title { get; set; }
		public string? Description { get; set; }

	
	}
}
