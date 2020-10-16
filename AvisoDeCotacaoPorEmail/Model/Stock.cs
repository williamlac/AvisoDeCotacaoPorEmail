using System;
using System.Threading.Tasks;
using NLog;
using NLog.Extensions.Logging;

namespace AvisoDeCotacaoPorEmail
{
    public class Stock
    {
        // Fields

        // Methods
        public Stock(string symbol, double minValue, double maxValue)
        {
            this.Symbol = symbol;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        // Properties
        public double CurValue { get; set; }

        public string Symbol { get; }

        public double MinValue { get; }

        public double MaxValue { get; }

        public void UpdateCurValue(){
            Task<double> task = new Api().GetCurrentStockValue(this.Symbol);
            task.Wait();
            this.CurValue = task.Result > 0 ? task.Result : this.CurValue;
                
        }
    
    }



}