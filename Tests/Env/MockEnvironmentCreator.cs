using System;
using AzureRepositories;
using Common.IocContainer;
using Common.Log;
using Core.Feed;
using Core.Finance;
using Core.Traders;
using LkeServices;

namespace Tests.Env
{
    public static class MockEnvironmentCreator
    {

        public static IoC Create()
        {

            var ioc = new IoC();
            var log = new LogToConsole();

            ioc.Register<ILog>(log);

            AzureRepoBinder.BindAzureReposInMem(ioc);
            SrvBinder.BindTraderPortalServices(ioc);

            return ioc;

        }


        public static ITrader RegisterTrader(this IoC ioc, string email)
        {
            var trader = new Trader {Email = email};

            return ioc.GetObject<ITradersRepository>().RegisterAsync(trader, "123").Result;
        }


        public static void SetMarketPrice(this IoC ioc, string asset, double bid, double ask)
        {
            MarketProfileCache.SetPrice(new AssetQuote {DateTime = DateTime.UtcNow, Ask = ask, Bid = bid, Id = asset});
        }


        public static void DepositAcount(this IoC ioc, string traderId, string currency, double amount)
        {
            ioc.GetObject<SrvBalanceAccess>().ChangeBalance(traderId, currency, amount);
        }


        public static double GetBalance(this IoC ioc, string traderId, string currency)
        {
            return ioc.GetObject<SrvBalanceAccess>().GetCurrencyBalances(traderId).Result.GetBalance(currency);
        }

    }
}
