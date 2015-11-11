using System.Collections.Generic;
using System.Threading;

namespace Core.Feed
{
    public static class MarketProfileCache
    {

        private static readonly Dictionary<string, AssetQuote> Profile = new Dictionary<string, AssetQuote>();

        private static readonly ReaderWriterLockSlim ReaderWriterLockSlim = new ReaderWriterLockSlim();

        public static void SetPrice(AssetQuote price)
        {
            ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                if (!Profile.ContainsKey(price.Id))
                    Profile.Add(price.Id, price);
                else
                    Profile[price.Id] = price;
            }
            finally 
            {
                ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public static AssetQuote GetPrice(string id)
        {
            ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return Profile.ContainsKey(id) ? Profile[id] : null;
            }
            finally
            {
                ReaderWriterLockSlim.ExitReadLock();
            }

        }
        
    }
}
