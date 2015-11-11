using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Core;
using Core.Finance;
using Core.Traders;
using LykkeMarketPlace.Models;

namespace LykkeMarketPlace.Controllers
{
    [Authorize]
    public class MarketPlaceController : Controller
    {
        private readonly ITradersRepository _tradersRepository;
        private readonly SrvBalanceAccess _srvBalanceAccess;

        public MarketPlaceController(ITradersRepository tradersRepository, SrvBalanceAccess srvBalanceAccess)
        {
            _tradersRepository = tradersRepository;
            _srvBalanceAccess = srvBalanceAccess;
        }

        [HttpPost]
        public async Task<ActionResult> Index()
        {
            var id = this.GetTraderId();
            var viewModel = new MarketPlaceIndexViewModel
            {
                Trader = await _tradersRepository.GetByIdAsync(id),
                CurrencyBalances = await _srvBalanceAccess.GetCurrencyBalances(id)
            };

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult GetAssets(string currency)
        {

            var viewModel = new GetAssetsViewModel
            {
                Currency = currency,
                Instruments = GlobalSettings.GetFinanceInstruments(currency).ToArray()
            };

            return View(viewModel);
        }
    }
}