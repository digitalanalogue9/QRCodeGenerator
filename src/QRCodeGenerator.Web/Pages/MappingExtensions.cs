namespace QRCodeGenerator.Web.Pages
{
    using System.Linq;
    using System.Threading.Tasks;

    using QRCodeGenerator.Web;

    public static class MappingExtensions
    {
        public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
            => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);
    }
}