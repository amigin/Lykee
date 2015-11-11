using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Common;
using Common.Log;
using Core;
using Core.Feed;
using Core.Finance;

namespace LykkeMarketPlace.Services
{
    public class MockFeed : TimerPeriod
    {
        private readonly Action<AssetQuote> _change;


        private readonly AssetQuote[] _prices;
        private readonly Random _random = new Random();

        public MockFeed(Action<AssetQuote> change, ILog log):base("MockFeed", 500, log)
        {
            _change = change;

            _prices =
                GlobalSettings.FinanceInstruments.Values.Select(
                    itm => new AssetQuote { Id = itm.Id, Bid = 1.55555, Ask = 1.55565}).ToArray();
        }

        protected override Task Execute()
        {
            foreach (var assetPrice in _prices)
            {
                var delta = _random.Next(-5, 5);
                assetPrice.AddPunkts(delta);

                _change?.Invoke(assetPrice);
            }

            return Task.FromResult(0);
        }
    }
}