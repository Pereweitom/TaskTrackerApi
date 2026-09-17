namespace TaskTrackerApi.Models.Dtos
{
    public class UpdateTaskDto
    {
		public required string Title { get; set; }
		public string? Description { get; set; }

	}
}
