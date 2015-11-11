using System;

namespace Core.Feed
{
    public class AssetQuote
    {
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
    }

    public static class AssetPriceExt
    {


        public static void AddPunkts(this AssetQuote assetPrice, int punkts)
        {
            var fi = GlobalSettings.FinanceInstruments[assetPrice.Id];
            assetPrice.Bid = fi.AddPunkts(assetPrice.Bid, punkts);
            assetPrice.Ask = fi.AddPunkts(assetPrice.Ask, punkts);
        }
    }

}
