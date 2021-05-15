using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqSelectExtensions
    {
        public static IEnumerable<SelectTryResult<TSource, TResult>> SelectTry<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, TResult> selector)
        {
            foreach (TSource source in sources)
            {
                SelectTryResult<TSource, TResult> selectTryResult;
                try
                {
                    selectTryResult = new SelectTryResult<TSource, TResult>(source, selector(source), null);
                }
                catch (Exception ex)
                {
                    selectTryResult = new SelectTryResult<TSource, TResult>(source, default(TResult), ex);
                }
                yield return selectTryResult;
            }
        }

        public static IEnumerable<TResult> OnCaughtException<TSource, TResult>(this IEnumerable<SelectTryResult<TSource, TResult>> selectTryResults, Func<Exception, TResult> exceptionHandler)
        {
            return selectTryResults.Select(x => x.CaughtException == null ? x.Result : exceptionHandler(x.CaughtException));
        }
    }

    public class SelectTryResult<TSource, TResult>
    {
        public TSource Source { get; private set; }
        public TResult Result { get; private set; }

        public Exception CaughtException { get; private set; }

        internal SelectTryResult(TSource source, TResult result, Exception exception)
        {
            Source = source;
            Result = result;
            CaughtException = exception;
        }
    }
}
