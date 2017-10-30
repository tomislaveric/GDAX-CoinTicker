using System;
using System.Globalization;

namespace coinTicker
{
    public static class ExtensionMethods
    {
        public static string ToCurrency(this double value, string currencyToShow)
        {
            const string bitcoin = "\u0243";
            const string euro = "\u20ac";
            const string dollar = "\u0024";
            const string britishPound = "\u00a3";

            var price = value.ToString("F2");
            switch (currencyToShow)
            {
                case "BTC":
                    return $"{price} {bitcoin}";
                case "EUR":
                    return $"{price} {euro}";
                case "USD":
                    return $"{price} {dollar}";
                case "GBP":
                    return $"{price} {britishPound}";
                default:
                    return price;
            }
        }
    }
}