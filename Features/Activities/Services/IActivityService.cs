using FriendStuff.Features.Activities.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.Activities.Services;

public interface IActivityService
{
    public Task<Result> CreateActivity(CreateActivityRequest request, string adminUsername, CancellationToken ct);

    public Task<Result> DeleteActivity(string publicActivityId, string username, CancellationToken ct);

    public Task<Result> AddParticipants(AddParticipantsRequest request, string username, CancellationToken ct);

    public Task<Result> RemoveParticipant(RemoveParticipantRequest request, string username, CancellationToken ct);

    public Task<Result> CreateActivityType(CreateActivityTypeRequest request, CancellationToken ct);

    // View
    public Task<Result<List<UserActivityResponse>?>> GetUserActivities(string username);
    public Task<Result<UserActivityDetailsResponse>> GetUserActivity(string activityPublicId);
    public Task<Result<List<ActivityTypesResponse>>> GetActivityTypes();

}
