using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AvisoDeCotacaoPorEmail.ApiClasses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AvisoDeCotacaoPorEmail
{
    
    
    public class Api
    {
        // https://webhook.site/3ff4da5d-a13f-4000-b38c-84bc641cab72
        public async Task<Double> GetValue(String symbol)
        {
            var client = new HttpClient();
            String activeApi = new Configs().GetAppSettings()["ActiveApi"];
            var url = GetApiUrl(activeApi, symbol);
            var result = await client.GetAsync(url);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return GetApiCurrentValue(activeApi, json);
        }

        private String GetApiUrl(String activeApi, String symbol)
        {
            switch (activeApi)
            {
                case "AlphaVantage":
                    return new AlphaVantage().GetUrl(symbol);
            }
            throw new Exception("API "+ activeApi + " is not recognized");
        }
        private double GetApiCurrentValue(String activeApi, String json)
        {
            switch (activeApi)
            {
                case "AlphaVantage":
                    return new AlphaVantage().GetCurrentValueFromJson(json);
            }
            throw new Exception("API "+ activeApi + " is not recognized");
        }
        
       
    }
}