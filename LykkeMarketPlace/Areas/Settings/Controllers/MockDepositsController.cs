using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;
using Core.Finance;
using LykkeMarketPlace.Areas.Settings.Models;
using LykkeMarketPlace.Controllers;
using LykkeMarketPlace.Hubs;
using LykkeMarketPlace.Strings;

namespace LykkeMarketPlace.Areas.Settings.Controllers
{
    [Authorize]
    public class MockDepositsController : Controller
    {
        private readonly SrvBalanceAccess _srvBalanceAccess;

        public MockDepositsController(SrvBalanceAccess srvBalanceAccess)
        {
            _srvBalanceAccess = srvBalanceAccess;
        }

        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> MockDeposit(MockDepositModel model)
        {
            if (string.IsNullOrEmpty(model.Amount))
                return this.JsonFailResult("#amount", Phrases.FieldShouldNotBeEmpty);

            double amount = 0;

            try
            {
                amount = model.Amount.ParseAnyDouble();
            }
            catch (Exception)
            {
                return this.JsonFailResult("#amount", Phrases.InvalidAmountFormat);
            }

            var id = this.GetTraderId();

            await _srvBalanceAccess.ChangeBalance(id, model.Currency, amount);

            await LkHub.RefreshBalance(id);

            return this.JsonHideDialog();
        }
    }
}