using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Utils.Page;

namespace Sleep.Infrasctructure.Utils.Page
{
    public static class PagedListExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(
            this IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<T>(items, page, pageSize, totalCount);
        }
    }
}
