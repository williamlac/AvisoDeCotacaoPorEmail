using System;
using System.Threading.Tasks;

namespace AvisoDeCotacaoPorEmail
{
    class Program
    {
        private static void Main(string[] args)
        {
            Task<double> task = new Api().GetValue("PETR4.SA");
            task.Wait();
            double result = task.Result;
            // new Email().SendEmail("williamslacerda@gmail.com", "Acima "+result, "Aviso Max").Wait();

            Console.WriteLine(result);
            Console.WriteLine("fim");
        }
    }
}
