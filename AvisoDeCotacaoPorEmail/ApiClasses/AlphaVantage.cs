using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AvisoDeCotacaoPorEmail.ApiClasses
{
    public class AlphaVantage
    {
        [JsonProperty(PropertyName = "Global Quote")]
        public GlobalQuote GlobalQuote { get; set; }

        public string GetUrl(String symbol)
        {
            var config = new Configs().GetAppSettings();

            return "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=" + symbol + "&apikey=" +
                   config["ApiKeys:AlphaVantage"];
        }

        public double GetCurrentValueFromJson(string json)
        {
            var res = JsonConvert.DeserializeObject<AlphaVantage>(json);
            return res.GlobalQuote.Price;
        }
        
    }

    public class GlobalQuote
    {
        [JsonProperty(PropertyName = "01. symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "02. open")]
        public double Open { get; set; }
        [JsonProperty(PropertyName = "03. high")]
        public double High { get; set; }
        [JsonProperty(PropertyName = "04. low")]
        public double Low { get; set; }
        [JsonProperty(PropertyName = "05. price")]
        public double Price { get; set; }
        [JsonProperty(PropertyName = "06. volume")]
        public int Volume { get; set; }
        [JsonProperty(PropertyName = "07. latest trading day")]
        public string LatestTradingDay { get; set; }
        [JsonProperty(PropertyName = "08. previous close")]
        public double PreviousClose { get; set; }
        [JsonProperty(PropertyName = "09. change")]
        public double Change { get; set; }
        [JsonProperty(PropertyName = "10. change percent")]
        public string ChangePercent { get; set; }
    }

}
