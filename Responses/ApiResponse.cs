namespace TaskTrackerApi.Responses;

public class ApiResponse<T>
{
	public string Status { get; set; } = string.Empty;

	public T? Data { get; set; }

	public string Message { get; set; } = string.Empty;
}