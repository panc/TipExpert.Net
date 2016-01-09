using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    internal static class Extensions
    {
        public static Task<TDocument[]> ToArrayAsync<TDocument>(this IAsyncCursorSource<TDocument> source)
        {
            return ToArrayAsync<TDocument>(source, new CancellationToken());
        }

        public static async Task<TDocument[]> ToArrayAsync<TDocument>(this IAsyncCursorSource<TDocument> source, CancellationToken cancellationToken)
        {
            List<TDocument> list;
            using (IAsyncCursor<TDocument> source1 = await source.ToCursorAsync(cancellationToken).ConfigureAwait(false))
                list = await IAsyncCursorExtensions.ToListAsync<TDocument>(source1, cancellationToken).ConfigureAwait(false);
            return list.ToArray();
        }
    }
}