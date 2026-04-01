using System;
using FriendStuff.Data;
using FriendStuff.Features.Activities.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;
using FriendStuff.Shared.Results.Enums;
using FriendStuff.Domain.Entities;
using FriendStuff.Domain.Enums;

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
            .FirstOrDefaultAsync(cancellationToken: ct); // throw exception TO DO

        var checkActivityExists = await context.Activities
            .AnyAsync(a => a.AdminId == adminId && a.NormalizedName == normalizedActivityName && a.StartDate == request.StartDate, cancellationToken: ct);

        if (checkActivityExists)
            return Result.Failure(new Error
            {
                Title = "Create activity error",
                Message = "Activty already exists",
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
}
