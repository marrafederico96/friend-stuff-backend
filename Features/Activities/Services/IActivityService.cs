using System;
using FriendStuff.Features.Activities.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.Activities.Services;

public interface IActivityService
{
    public Task<Result> CreateActivity(CreateActivityRequest request, string adminUsername, CancellationToken ct);

    public Task<Result> DeleteActivity(string publicActivityId, string username, CancellationToken ct);

    public Task<Result> AddParticipants(AddParticpantsRequest request, string username, CancellationToken ct);

}
