using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace AvisoDeCotacaoPorEmail
{
    public class Csv
    {
        public  string csvPath = new Configs().GetAppSettings()["StockListCsv"];

        public  string CsvPath => csvPath;

        public List<String> GetStockSymbols()
        {
            List<string> myStringColumn= new List<string>();
            using (var fileReader = File.OpenText(csvPath))
            using (var csvResult  = new CsvHelper.CsvReader(fileReader, CultureInfo.InvariantCulture))
            {
                csvResult.Read();
                csvResult.ReadHeader();
                while (csvResult.Read())
                {
                    string stringField=csvResult.GetField<string>("Symbol");
                    myStringColumn.Add(stringField);    
                }
            }

            return myStringColumn;
        }
        public List<Stock> GetStocks()
        {
            List<Stock> stocks = new List<Stock>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                Stock stock;
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();


                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    stock = csv.GetRecord<Stock>();
                    stocks.Add(stock);
                }                
            }

            return stocks;

        }
        public void AddStock(Stock stock)
        {
            bool writeHeader = !File.Exists(csvPath);
            using (var stream = File.Open(csvPath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (writeHeader)
                {
                    csvWriter.WriteHeader(typeof(Stock));
                }
                csvWriter.NextRecord();
                csvWriter.WriteRecord(stock);
            }
        }
        public void RemoveStock(String stockSymbol)
        {
            string tempPath = csvPath.Replace(".csv", "_temp.csv");
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var writer = new StreamWriter(tempPath))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                Stock record;
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();


                csv.Read();
                csv.ReadHeader();

                csvWriter.WriteHeader(typeof(Stock));

                while (csv.Read())
                {
                    record = csv.GetRecord<Stock>();

                    if (record.Symbol != stockSymbol)
                    {
                        csvWriter.NextRecord();
                        csvWriter.WriteRecord(record);
                    }                    
                }                
            }

            File.Delete(csvPath);
            File.Move(tempPath, csvPath);
        }
    }
}