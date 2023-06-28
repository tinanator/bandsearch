using Microsoft.Extensions.Caching.Memory;

namespace BandSearch.Web.Database
{
    public class BandSearchMemoryCache
    {
        public MemoryCache Cache { get; } = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024
            }
        );
    }
}
