using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AvisoDeCotacaoPorEmail.ApiClasses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;

namespace AvisoDeCotacaoPorEmail
{

    public class SearchResult
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
    public class Api
    {
        public async Task<Double> GetCurrentStockValue(String symbol)
        {
            Logger log = LogManager.GetCurrentClassLogger();
            LogManager.Configuration =
                new NLogLoggingConfiguration((new Configs().GetAppSettings()).GetSection("NLog"));
            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                var client = new HttpClient();
                String activeApi = new Configs().GetAppSettings()["ActiveApi"];
                var url = GetApiUrl(activeApi, "GetCurrentStockValue", symbol);
                result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();
                return GetApiCurrentValue(activeApi, json);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                log.Error(await result.Content.ReadAsStringAsync());
                throw e;
            }
        }
        
        public async Task<List<SearchResult>> SearchStocks(String symbol)
        {
            var client = new HttpClient();
            String activeApi = new Configs().GetAppSettings()["ActiveApi"];
            var url = GetApiUrl(activeApi,"SearchStocks", symbol);
            var result = await client.GetAsync(url);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return GetApiSearchRes(activeApi, json);
        }
        
        

        private String GetApiUrl(String activeApi, String action,  String symbol)
        {
            switch (activeApi)
            {
                case "AlphaVantage":
                    return new AlphaVantage().GetUrl(symbol, action);
            }
            throw new Exception("API "+ activeApi + " is not recognized");
        }
        private double GetApiCurrentValue(String activeApi, String json)
        {
            try
            {
                switch (activeApi)
                {
                    case "AlphaVantage":
                        return new AlphaVantage().GetCurrentValueFromJson(json);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            throw new Exception("API "+ activeApi + " is not recognized");
        }
        private List<SearchResult> GetApiSearchRes(String activeApi, String json)
        {
            switch (activeApi)
            {
                case "AlphaVantage":
                    return new AlphaVantage().GetSearchResFromJson(json);
            }
            throw new Exception("API "+ activeApi + " is not recognized");
        }
        
       
    }
}