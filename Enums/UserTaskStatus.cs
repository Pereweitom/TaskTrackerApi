using System.Text.Json.Serialization;

namespace TaskTrackerApi.Enums;

public enum UserTaskStatus
{
	[JsonStringEnumMemberName("todo")]
	Todo,

	[JsonStringEnumMemberName("in-progress")]
	InProgress,

	[JsonStringEnumMemberName("done")]
	Done
}