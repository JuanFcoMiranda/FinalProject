using FinalProject.Application.Common.Models;

namespace FinalProject.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber,
        int pageSize)
  where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    }

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
  this IQueryable queryable,
    CancellationToken cancellationToken = default)
  where TDestination : class
    {
        return queryable
     .ProjectToType<TDestination>()
      .ToListAsync(cancellationToken);
    }
}
