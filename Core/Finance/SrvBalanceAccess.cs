using System.Linq;
using System.Threading.Tasks;

namespace Core.Finance
{
    public class CurrencyBalance
    {
        public string Id { get; set; }
        public double Value { get; set; }

        public static CurrencyBalance Create(string id, double value)
        {
            return new CurrencyBalance
            {
                Id = id,
                Value = value
            };
        }
    }

    /// <summary>
    /// Service, which calculates balance via Instruments
    /// </summary>
    public class SrvBalanceAccess
    {
        private readonly IBalanceRepository _balanceRepository;

        public SrvBalanceAccess(IBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public async Task<CurrencyBalance[]> GetCurrencyBalances(string traderId)
        {

            var balances = (await _balanceRepository.GetAsync(traderId)).ToArray();

            var result = GlobalSettings.Currencies.Values.Select(value => CurrencyBalance.Create(value.Id, 0)).ToArray();

            foreach (var currencyBalance in result)
            {
               var balance = balances.FirstOrDefault(itm => itm.Currency == currencyBalance.Id);

                if (balance != null)
                    currencyBalance.Value = balance.Amount;
            }

            return result;

        }


        public Task ChangeBalance(string traderId, string currency, double amount)
        {
            return _balanceRepository.ChangeBalanceAsync(traderId, currency, amount);
        }


    }


    public static class BalanceAccessExt
    {
        public static double GetBalance(this CurrencyBalance[] balances, string currency)
        {
            var balance = balances.FirstOrDefault(itm => itm.Id == currency);
            return balance == null ? 0 : balance.Value;
        }
    }
}
