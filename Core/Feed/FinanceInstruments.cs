using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Core.Feed
{
    public class FinanceInstrument
    {

        public string Id { get; set; }

        /// <summary>
        /// Base currency
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Quoting currency
        /// </summary>
        public string Quoting { get; set; }

        public int Accuracy { get; set; }

        private int _multiplier = -1;
        public int Multiplier
        {
            get
            {
                if (_multiplier > 0)
                    return _multiplier;

                return _multiplier = (int)Math.Pow(10, Accuracy);
            }
            
        }

    }


    public static class FinanceInstrumentExt
    {
        private static readonly Dictionary<int, string> AccuracyMasks = new Dictionary<int, string>(); 

        public static string RateToString(this double rate, FinanceInstrument instr)
        {
            if (!AccuracyMasks.ContainsKey(instr.Accuracy))
                AccuracyMasks.Add(instr.Accuracy, "0."+new string('0', instr.Accuracy));

            var mask = AccuracyMasks[instr.Accuracy];
            return rate.ToString(mask);
        }


        public static double AddPunkts(this FinanceInstrument fi, double rate, int punkts)
        {
            var divider = Math.Pow(10, fi.Accuracy);
            rate = (rate * divider + punkts) / divider;
            return Math.Round(rate, fi.Accuracy);
        } 
    }

}
