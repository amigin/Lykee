﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using AzureRepositories;
using Common.IocContainer;
using Common.Log;
using Core.Feed;
using Core.Finance;
using LkeServices;
using LkeServices.Feed;
using LkeServices.Orders;
using LykkeMarketPlace.Hubs;

namespace LykkeMarketPlace
{
    public static class Settings
    {

        public static string ConnectionString
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ConnectionString"];
                }
                catch (Exception)
                {

                    return "UseDevelopmentStorage=true";
                }
            }
        }

        public static IPEndPoint FeedIpEndPoint
        {
            get
            {
                try
                {
                    var strings = ConfigurationManager.AppSettings["FeedIp"].Split(':');
                    return new IPEndPoint(IPAddress.Parse(strings[0]), int.Parse(strings[1]));
                }
                catch (Exception)
                {

                    return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8090);
                }
            }
        }

    }




    public static class Dependencies
    {


        //   private static MockFeed _mockFeed;
        private static Mt4BridgeFeedSource _mt4BridgeFeedSource;


        public static SrvBalanceAccess BalanceAccess { get; private set; }

        public static SrvOrdersRegistrator SrvOrdersRegistrator { get; private set; }
        public static SrvOrdersExecutor SrvOrdersExecutor { get; private set; }
        public static SrvLimitOrderBookGenerator SrvLimitOrderBookGenerator { get; private set; }




        public static IDependencyResolver CreateDepencencyResolver()
        {


            var dr = new MyDependencyResolver();
            var log = new LogToConsole();
            dr.IoC.Register<ILog>(log);

#if DEBUG
            AzureRepoBinder.BindAzureReposInMem(dr.IoC);
#else
              AzureRepoBinder.BindAzureRepositories(dr.IoC, Settings.ConnectionString, log);
#endif




            SrvBinder.BindTraderPortalServices(dr.IoC);

            //_mockFeed = new MockFeed(ap =>
            //{
            //    LkHub.BroadcastPrice(ap);
            //    MarketProfileCache.SetPrice(ap);
            //}, log);
            //_mockFeed.Start();

            _mt4BridgeFeedSource = new Mt4BridgeFeedSource(Settings.FeedIpEndPoint, log);

            _mt4BridgeFeedSource.RegisterFeed(aqs =>
            {
                foreach (var assetQuote in aqs)
                {
                    LkHub.BroadcastPrice(assetQuote);
                    MarketProfileCache.SetPrice(assetQuote);
                }
            });

            _mt4BridgeFeedSource.Start();

            BalanceAccess = dr.IoC.GetObject<SrvBalanceAccess>();
            SrvOrdersRegistrator = dr.IoC.GetObject<SrvOrdersRegistrator>();
            SrvOrdersExecutor = dr.IoC.GetObject<SrvOrdersExecutor>();
            SrvLimitOrderBookGenerator = dr.IoC.GetObject<SrvLimitOrderBookGenerator>();

            SrvOrdersExecutor.RegisterUpdateOrderBook(LkHub.RefreshOrderBooks);

            SrvBinder.StartTraderPortalServices(dr.IoC);

            SrvOrdersExecutor.RegisterBalanceChange(LkHub.RefreshBalance);

            return dr;
        }
    }

    public class MyDependencyResolver : IDependencyResolver
    {

        public readonly IoC IoC = new IoC();

        public void RegisterSingleTone<T, TI>() where TI : T
        {
            IoC.RegisterSingleTone<T, TI>();
        }

        public void RegisterSingleTone<T, TI>(TI instance) where TI : T
        {
            IoC.Register<T>(instance);
        }

        public T GetType<T>() where T : class
        {
            var result = IoC.GetObject<T>();
            return result;
        }

        public object GetService(Type serviceType)
        {
            var result = IoC.CreateInstance(serviceType);

            return result;
        }

        private readonly object[] _nullData = new object[0];
        public IEnumerable<object> GetServices(Type serviceType)
        {
            var result = IoC.CreateInstance(serviceType);
            return result == null ? _nullData : new[] { result };
        }

    }
}