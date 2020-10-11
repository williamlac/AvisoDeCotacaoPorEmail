using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;

namespace AvisoDeCotacaoPorEmail.ApiClasses
{
    public class AlphaVantage
    {
        [JsonProperty(PropertyName = "Global Quote")]
        public GlobalQuote GlobalQuote { get; set; }
        
        [JsonProperty(PropertyName = "bestMatches")]
        public List<SymbolSearch> SymbolSearchList { get; set; }
        

        public string GetUrl(String symbol, String action)
        {
            var config = new Configs().GetAppSettings();
            switch (action)
            {
                case "GetCurrentStockValue":
                    return "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=" + symbol + "&apikey=" +
                           config["ApiKeys:AlphaVantage"];
                case "SearchStocks":
                    return "https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=" + symbol + "&apikey=" +
                        config["ApiKeys:AlphaVantage"];
                    
            }
            throw new Exception("Invalid Action "+ action);
        }

        public double GetCurrentValueFromJson(string json)
        {
            var res = JsonConvert.DeserializeObject<AlphaVantage>(json);
            return res.GlobalQuote.Price;
      
        }
        public List<SearchResult> GetSearchResFromJson(string json)
        {
            var res = JsonConvert.DeserializeObject<AlphaVantage>(json);
            Console.WriteLine(res);
            return  res.SymbolSearchList.Select( x => new SearchResult() 
            { 
                Name = x.Name,
                Symbol = x.Symbol
            } ).ToList();
            ;
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
    public class SymbolSearch
    {
        [JsonProperty(PropertyName = "1. symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "2. name")]
        public String Name { get; set; }
    }

}
