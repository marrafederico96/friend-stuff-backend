using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Domain.Enums;
using FriendStuff.Features.Activities.DTOs;
using FriendStuff.Shared.Results;
using FriendStuff.Shared.Results.Enums;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Features.Activities.Services;

public class ActivityService(FriendStuffDbContext context) : IActivityService
{
    public async Task<Result> CreateActivity(CreateActivityRequest request, string adminUsername, CancellationToken ct)
    {
        var normalizedAdminUsername = adminUsername.Trim().ToUpperInvariant();
        var normalizedActivityName = request.Name.Trim().ToUpperInvariant();

        var adminId = await context.Users
            .Where(u => u.NormalizedUsername == normalizedAdminUsername)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct); // throw exception TO DO

        var checkActivityExists = await context.Activities
            .AnyAsync(
                a => a.AdminId == adminId && a.NormalizedName == normalizedActivityName &&
                     a.StartDate == request.StartDate, ct);

        if (checkActivityExists)
            return Result.Failure(new Error
            {
                Title = "Create activity error",
                Message = "Activity already exists",
                Type = ErrorType.Conflict
            });

        if (request.StartDate > request.EndDate)
            return Result.Failure(new Error
            {
                Title = "Create activity error",
                Message = "Start date must be earlier than the end date.",
                Type = ErrorType.Conflict
            });

        var newActivity = new Activity
        {
            AdminId = adminId,
            Name = request.Name,
            NormalizedName = normalizedActivityName,
            Description = request.Description,
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Participants =
            [
                new UserActivity
                {
                    UserId = adminId,
                    Role = UserRole.Admin
                }
            ]
        };

        context.Activities.Add(newActivity);
        await context.SaveChangesAsync(ct);

        return Result.Success("Activity created");
    }

    public async Task<Result> DeleteActivity(string publicActivityId, string username, CancellationToken ct)
    {
        var userId = await context.Users.Where(u => u.NormalizedUsername == username.Trim().ToUpperInvariant())
            .Select(u => u.Id).FirstOrDefaultAsync(ct);

        var checkActivity = await context.Activities
            .AnyAsync(a => a.PublicId.ToString() == publicActivityId && a.AdminId == userId, ct);

        if (!checkActivity)
            return Result.Failure(new Error
            {
                Title = "Delete activity error",
                Message = "Activity not found or user not admin",
                Type = ErrorType.Forbidden
            });

        await context.Activities
            .Where(a => a.PublicId.ToString() == publicActivityId)
            .ExecuteDeleteAsync(ct);

        return Result.Success("Activity deleted");
    }

    public async Task<Result> AddParticipants(AddParticipantsRequest request, string username, CancellationToken ct)
    {
        var normalizedUsernames = request.Usernames
            .Select(u => u.Trim().ToUpperInvariant())
            .ToList();

        var adminId = await context.Users.Where(u => u.NormalizedUsername == username.Trim().ToUpperInvariant())
            .Select(u => u.Id).FirstOrDefaultAsync(ct);

        var activityId = await context.Activities
            .Where(a => a.PublicId.ToString() == request.PublicActivityId && a.AdminId == adminId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(ct);

        var userIds = await context.Users
            .Where(u => normalizedUsernames.Contains(u.NormalizedUsername))
            .Select(u => u.Id)
            .ToListAsync(ct);

        var existingUserIds = await context.UsersActivities
            .Where(a => userIds.Contains(a.UserId) && a.ActivityId == activityId)
            .Select(a => a.UserId)
            .ToListAsync(ct);

        var newUserIds = userIds.Except(existingUserIds).ToList();

        if (newUserIds.Count == 0)
            return Result.Success("Participants already exist");

        List<UserActivity> newParticipants =
        [
            .. newUserIds.Select(u => new UserActivity
            {
                Role = UserRole.Participant,
                ActivityId = activityId,
                UserId = u
            })
        ];

        await context.UsersActivities.AddRangeAsync(newParticipants, ct);
        await context.SaveChangesAsync(ct);

        return Result.Success("Participants added");
    }

    public async Task<Result> RemoveParticipant(RemoveParticipantRequest request, string username, CancellationToken ct)
    {
        var adminId = await context.Users.Where(u => u.NormalizedUsername == username.Trim().ToUpperInvariant())
            .Select(u => u.Id).FirstOrDefaultAsync(ct);

        var checkActivityAdmin =
            await context.Activities.AnyAsync(
                a => a.PublicId.ToString() == request.PublicActivityId && a.AdminId == adminId, ct);

        if (!checkActivityAdmin)
            return Result.Failure(new Error
            {
                Title = "Error remove participant",
                Message = "You are not admin",
                Type = ErrorType.Unauthorized
            });

        var userIdToRemove = await context.Users
            .Where(u => u.NormalizedUsername == request.Username.Trim().ToUpperInvariant())
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        var activityId = await context.Activities
            .Where(a => a.PublicId.ToString() == request.PublicActivityId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(ct);

        await context.UsersActivities
            .Where(a => a.ActivityId == activityId && a.UserId == userIdToRemove)
            .ExecuteDeleteAsync(ct);

        return Result.Success("Participant removed");
    }
}