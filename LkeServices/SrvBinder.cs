using Common.IocContainer;
using Core.Finance;
using Core.Orders;
using LkeServices.Orders;

namespace LkeServices
{
    public static class SrvBinder
    {

        public static void BindTraderPortalServices(IoC ioc)
        {

            ioc.RegisterSingleTone<SrvBalanceAccess>();
            ioc.RegisterSingleTone<SrvOrdersRegistrator>();
            ioc.RegisterSingleTone<SrvOrdersExecutor>();
            ioc.SelfBond<IOrderExecuter, SrvOrdersExecutor>();
            ioc.RegisterSingleTone<SrvLimitOrderBookGenerator>();

        }


        public static void StartTraderPortalServices(IoC ioc)
        {

        }
    }
}
