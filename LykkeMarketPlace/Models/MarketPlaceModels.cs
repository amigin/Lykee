using System.Collections.Generic;
using Core.Feed;
using Core.Finance;
using Core.Traders;

namespace LykkeMarketPlace.Models
{
    public class MarketPlaceIndexViewModel
    {
        public ITrader Trader { get; set; }
        public CurrencyBalance[] CurrencyBalances { get; set; }
    }

    public class GetAssetsViewModel
    {
        public string Currency { get; set; }
        public FinanceInstrument[] Instruments { get; set; } 
    }

}