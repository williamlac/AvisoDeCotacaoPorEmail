using System.Threading.Tasks;

namespace AvisoDeCotacaoPorEmail
{
    public class Stock
    {
        // Fields
        private string _name;
        private double _minValue;
        private double _maxValue;
        private double _curValue;

        // Methods
        public Stock(string name, double minValue, double maxValue)
        {
            this._name = name;
            this._minValue = minValue;
            this._maxValue = maxValue;
        }

        // Properties
        public double CurValue
        {
            get => 
                this._curValue;
            set => 
                this._curValue = value;
        }

        public string Name =>
            this._name;

        public double MinValue =>
            this._minValue;

        public double MaxValue =>
            this._maxValue;
        
        public void UpdateCurValue(){
            Task<double> task = new Api().GetValue("PETR4");
            task.Wait();
            this.CurValue = task.Result;
        }
    
    }



}