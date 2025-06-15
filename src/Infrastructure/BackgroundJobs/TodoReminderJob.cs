using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.BackgroundJobs
{
    public class TodoReminderJob : IInvocable
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TodoReminderJob(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Invoke()
        {
            var nowUtc = _dateTimeProvider.UtcNow;

            // Assuming TodoItem has properties: IsCompleted, Reminder (DateTimeOffset?)
            var dueTodos = await _dbContext.TodoItems
                .Where(t => !t.Done && t.Reminder.HasValue && t.Reminder <= nowUtc)
                .ToListAsync();

            foreach (var todo in dueTodos)
            {
                Log.Information(
                    "🔔 Reminder triggered for Todo [Id: {Id}, Title: {Title}] scheduled at {ReminderTime}",
                    todo.Id, todo.Title, todo.Reminder?.ToString("u"));
            }
        }
    }
}
