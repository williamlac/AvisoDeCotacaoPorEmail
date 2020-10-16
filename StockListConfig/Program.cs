using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AvisoDeCotacaoPorEmail;
using CsvHelper;

namespace StockListConfig
{
    class Program
    {
        static Csv csv = new Csv();
        static void Main(string[] args)
        {
            Console.WriteLine("Bem vindo ao Configurador da Lista de Ações a serem pesquisadas");
            Console.WriteLine("Por favor escreva ADICIONAR para adicionar uma ação na lista.");
            Console.WriteLine("Ou DELETAR para deletar uma ação da lista.");
            string command = Console.ReadLine().ToLower();
            if (command == "adicionar" || command == "a")
            {
                Stock s = GetStockToAddFromUser();
                csv.AddStock(s);
            }
            else if (command == "deletar" || command == "d")
            {
                String stockToDelete = GetStockToDeleteFromUser(csv.GetStockSymbols());
                csv.RemoveStock(stockToDelete);
            }
            Console.WriteLine("Alterado com Sucesso. Pressione qualquer tecla para sair.");
            Console.Read();
            // AddStock(csvPath, new Stock("HGLG11.SA", 20.0, 22.0));
            // RemoveStock("HGLG11.SA");
        }

        private static Stock GetStockToAddFromUser()
        {
            Console.WriteLine("Escreva o nome da ação");
            string symbol = Console.ReadLine().ToLower();
            Task<List<SearchResult>> task = new Api().SearchStocks(symbol);
            task.Wait();
            List<SearchResult> stockList = task.Result;
            bool hasStock = false;
            string stockSymbol = "";
            string val = "";
            while (!hasStock)
            {
                if (stockList.Count == 0)
                {
                    Console.WriteLine("Ação não encontrada!");
                }
                else
                {
                    hasStock = true;
                    Console.WriteLine("Por favor digite o número relacionado a ação desejada:");
                    int count = 0;
                    foreach (var stock in stockList)
                    {
                        Console.WriteLine($"{count++} - {stock.Symbol} : {stock.Name}");
                    } 
                    val = Console.ReadLine();
                    int choosenStockId = Convert.ToInt32(val);
                    stockSymbol = stockList[choosenStockId].Symbol;
                }
            }

            Console.WriteLine("Digite o preço mínimo para o aviso:");
            val = Console.ReadLine().Replace(",", ".");
            double min = Convert.ToDouble(val,  CultureInfo.InvariantCulture);
            Console.WriteLine("Digite o preço máximo para o aviso:");
            val = Console.ReadLine().Replace(",", ".");
            double max = Convert.ToDouble(val,  CultureInfo.InvariantCulture);
            return new Stock(stockSymbol, min, max);
        }
        private static String GetStockToDeleteFromUser(List<String> stockList)
        {
            Console.WriteLine("Por favor digite o número relacionado a ação desejada:");
            int count = 0;
            foreach (var stock in stockList)
            {
                Console.WriteLine($"{count++} - {stock}");
            }
            var val = Console.ReadLine();
            int choosenStockId = Convert.ToInt32(val);
            return stockList[choosenStockId];
        }

        
        
    
        
    }
}