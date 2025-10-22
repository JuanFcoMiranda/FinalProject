using FinalProject.Domain.Entities;

namespace FinalProject.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
