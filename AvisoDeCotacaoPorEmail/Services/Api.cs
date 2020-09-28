using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvisoDeCotacaoPorEmail
{
    public class API
    {
        // https://webhook.site/3ff4da5d-a13f-4000-b38c-84bc641cab72
        public async Task<Double> GetValue( String name)
        {
            var client = new HttpClient();

            var result = await client.GetAsync("https://webhook.site/3ff4da5d-a13f-4000-b38c-84bc641cab72");
            
            return result
            Console.WriteLine(result.StatusCode);
        }
    }
}