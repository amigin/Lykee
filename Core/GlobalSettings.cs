using System.Collections.Generic;
using System.Linq;
using Core.Feed;
using Core.Finance;

namespace Core
{

    public static class GlobalSettings
    {
        public const int Mt4TimeOffset = 2;

        public static readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>
        {
            {"CHF", new Currency {Id = "CHF", Name ="Swiss Franc"} },
            {"USD", new Currency {Id = "USD", Name ="US Dollar"} },
            {"EUR", new Currency {Id = "EUR", Name ="Euro"} },
            {"GBP", new Currency {Id = "GBP", Name ="British Pound"} },
            {"JPY", new Currency {Id = "JPY", Name ="Japanese Yen"} },
            {"CAD", new Currency {Id = "CAD", Name ="Canadian Dollar"} },
            {"AUD", new Currency {Id = "AUD", Name ="Australian Dollar"} },
            {"NZD", new Currency {Id = "NZD", Name ="New-Zealand Dollar"} },

 //           {"CNY", new Currency {Id = "CNY", Name ="Chinese Yuan"} },
        };


        public static readonly Dictionary<string, FinanceInstrument> FinanceInstruments = new Dictionary
            <string, FinanceInstrument>
        {
            {"AUDNZD", new FinanceInstrument {Id = "AUDNZD", Base = "AUD", Quoting = "NZD", Accuracy = 5}},
            {"AUDCAD", new FinanceInstrument {Id = "AUDCAD", Base = "AUD", Quoting = "CAD", Accuracy = 5}},
            {"AUDCHF", new FinanceInstrument {Id = "AUDCHF", Base = "AUD", Quoting = "CHF", Accuracy = 5}},
            {"AUDJPY", new FinanceInstrument {Id = "AUDJPY", Base = "AUD", Quoting = "JPY", Accuracy = 3}},
            {"AUDUSD", new FinanceInstrument {Id = "AUDUSD", Base = "AUD", Quoting = "USD", Accuracy = 5}},

            {"CADCHF", new FinanceInstrument {Id = "CADCHF", Base = "CAD", Quoting = "CHF", Accuracy = 5}},
            {"CADJPY", new FinanceInstrument {Id = "CADJPY", Base = "CAD", Quoting = "JPY", Accuracy = 3}},

            {"CHFJPY", new FinanceInstrument {Id = "CHFJPY", Base = "CHF", Quoting = "JPY", Accuracy = 3}},

            {"EURAUD", new FinanceInstrument {Id = "EURAUD", Base = "EUR", Quoting = "AUD", Accuracy = 5}},
            {"EURCAD", new FinanceInstrument {Id = "EURCAD", Base = "EUR", Quoting = "CAD", Accuracy = 5}},
            {"EURCHF", new FinanceInstrument {Id = "EURCHF", Base = "EUR", Quoting = "CHF", Accuracy = 5}},
            {"EURGBP", new FinanceInstrument {Id = "EURGBP", Base = "EUR", Quoting = "GBP", Accuracy = 5}},
            {"EURJPY", new FinanceInstrument {Id = "EURJPY", Base = "EUR", Quoting = "JPY", Accuracy = 3}},
            {"EURNZD", new FinanceInstrument {Id = "EURNZD", Base = "EUR", Quoting = "NZD", Accuracy = 5}},
            {"EURUSD", new FinanceInstrument {Id = "EURUSD", Base = "EUR", Quoting = "USD", Accuracy = 5}},

            {"GBPAUD", new FinanceInstrument {Id = "GBPAUD", Base = "GBP", Quoting = "AUD", Accuracy = 5}},
            {"GBPCAD", new FinanceInstrument {Id = "GBPCAD", Base = "GBP", Quoting = "CAD", Accuracy = 5}},
            {"GBPCHF", new FinanceInstrument {Id = "GBPCHF", Base = "GBP", Quoting = "CHF", Accuracy = 5}},
            {"GBPJPY", new FinanceInstrument {Id = "GBPJPY", Base = "GBP", Quoting = "JPY", Accuracy = 3}},
            {"GBPNZD", new FinanceInstrument {Id = "GBPNZD", Base = "GBP", Quoting = "NZD", Accuracy = 5}},
            {"GBPUSD", new FinanceInstrument {Id = "GBPUSD", Base = "GBP", Quoting = "USD", Accuracy = 5}},

            {"NZDCAD", new FinanceInstrument {Id = "NZDCAD", Base = "NZD", Quoting = "CAD", Accuracy = 5}},
            {"NZDCHF", new FinanceInstrument {Id = "NZDCHF", Base = "NZD", Quoting = "CHF", Accuracy = 5}},
            {"NZDJPY", new FinanceInstrument {Id = "NZDJPY", Base = "NZD", Quoting = "JPY", Accuracy = 3}},
            {"NZDUSD", new FinanceInstrument {Id = "NZDUSD", Base = "NZD", Quoting = "USD", Accuracy = 5}},

            {"USDCAD", new FinanceInstrument {Id = "USDCAD", Base = "USD", Quoting = "CAD", Accuracy = 5}},
            {"USDCHF", new FinanceInstrument {Id = "USDCHF", Base = "USD", Quoting = "CHF", Accuracy = 5}},
            {"USDJPY", new FinanceInstrument {Id = "USDJPY", Base = "USD", Quoting = "JPY", Accuracy = 3}},


        };


        public static IEnumerable<FinanceInstrument> GetFinanceInstruments(string currency)
        {
            return FinanceInstruments.Values.Where(itm => itm.Base == currency || itm.Quoting == currency);
        }

        public static FinanceInstrument FindInstument(string curFrom, string curTo)
        {
            foreach (var fi in FinanceInstruments.Values)
            {
                if (fi.Base == curFrom && fi.Quoting == curTo)
                    return fi;

                if (fi.Quoting == curFrom && fi.Base == curTo)
                    return fi;


            }

            return null;
        }

    }
}
