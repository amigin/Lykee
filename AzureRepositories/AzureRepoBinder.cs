using AzureRepositories.Finance;
using AzureRepositories.Orders;
using AzureRepositories.Traders;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates;
using Common.IocContainer;
using Common.Log;
using Core;
using Core.Finance;
using Core.Orders;
using Core.Traders;

namespace AzureRepositories
{
    public static class AzureRepoBinder
    {

        public static void BindAzureRepositories(IoC ioc, string connString, ILog log)
        {
            ioc.Register<ITradersRepository>(
                AzureRepoFactories.CreateTradersRepository(connString, log));

            ioc.Register<IBalanceRepository>(
                AzureRepoFactories.CreateBalanceRepository(connString, log));

            ioc.Register<IIdentityGenerator>(
                AzureRepoFactories.CreateIdentityGenerator(connString, log));

            ioc.Register<IOrdersRepository>(
                AzureRepoFactories.CreateOrdersRepository(connString, log));
        }



        public static void BindAzureReposInMem(IoC ioc)
        {
            ioc.Register<ITradersRepository>(
                new TradersRepository(new NoSqlTableInMemory<TraderEntity>(), new NoSqlTableInMemory<AzureIndex>()));

            ioc.Register<IBalanceRepository>(
                new BalanceRepository(new NoSqlTableInMemory<TraderBalanceEntity>()));

            ioc.Register<IIdentityGenerator>(
                new IdentityGenerator(new NoSqlTableInMemory<IdentityEntity>()));

            ioc.Register<IOrdersRepository>(
                new OrdersRepository(new NoSqlTableInMemory<OrderEntity>()));
        }

    }

}
