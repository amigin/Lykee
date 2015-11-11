using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Orders;

namespace LkeServices.Orders
{
    public enum BookOrderType
    {
        Bid,
        Ask
    }

    public class LimitOrderBookItem
    {
        public double Volume { get; set; }
        public double Rate { get; set; }
        public BookOrderType Type { get; set; }

    }

    public class LimitOrderBookModel
    {
        public string Asset { get; set; }
        public LimitOrderBookItem[] Items { get; set; }
    }

    public class SrvLimitOrderBookGenerator
    {
        private readonly IOrdersRepository _ordersRepository;

        public SrvLimitOrderBookGenerator(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<LimitOrderBookModel[]> GetOrderBooksAsync()
        {
            var orders =
                (await _ordersRepository.GetOrdersByStatusAsync(OrderStatus.Registered))
                    .Where(itm => itm is LimitOrder)
                    .Cast<LimitOrder>()
                    .ToArray();


            return orders.GroupBy(itm => itm.Asset)
                .Select(ords =>
                {
                    var bid = ords.Where(o => o.Action == OrderAction.Buy)
                        .GroupBy(o => o.Price)
                        .Select(
                            o =>
                                new LimitOrderBookItem
                                {
                                    Rate = o.Key,
                                    Volume = o.Sum(i => i.Volume),
                                    Type = BookOrderType.Bid
                                })
                        .ToArray();


                    var ask = ords.Where(o => o.Action == OrderAction.Sell)
                        .GroupBy(o => o.Price)
                        .Select(
                            o =>
                                new LimitOrderBookItem
                                {
                                    Rate = o.Key,
                                    Volume = o.Sum(i => i.Volume),
                                    Type = BookOrderType.Ask
                                })
                        .ToArray();

                    var items = new List<LimitOrderBookItem>(bid.Length + ask.Length + 1);

                    items.AddRange(bid);
                    items.AddRange(ask);

                    return new LimitOrderBookModel
                    {
                        Asset = ords.Key,

                        Items = items.OrderByDescending(itm => itm.Rate).ToArray()

                    };

                }).ToArray();
        }
    }
}
