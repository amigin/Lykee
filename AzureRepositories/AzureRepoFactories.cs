using AzureRepositories.Finance;
using AzureRepositories.Orders;
using AzureRepositories.Traders;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates;
using Common.Log;

namespace AzureRepositories
{
    public static class AzureRepoFactories
    {

        public static TradersRepository CreateTradersRepository(string connstring, ILog log)
        {
            const string tableName = "Traders";
            return new TradersRepository(
                new AzureTableStorage<TraderEntity>(connstring, tableName, log), 
                new AzureTableStorage<AzureIndex>(connstring, tableName, log));
        }

        public static BalanceRepository CreateBalanceRepository(string connstring, ILog log)
        {
            return new BalanceRepository(new AzureTableStorage<TraderBalanceEntity>(connstring, "AccountBalances", log));
        }


        public static IdentityGenerator CreateIdentityGenerator(string connstring, ILog log)
        {
            return new IdentityGenerator(new AzureTableStorage<IdentityEntity>(connstring, "Setup", log));
        }



        public static OrdersRepository CreateOrdersRepository(string connstring, ILog log)
        {
            return new OrdersRepository(new AzureTableStorage<OrderEntity>(connstring, "Orders", log));
        }

    }
}
