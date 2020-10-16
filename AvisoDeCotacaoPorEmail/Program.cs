using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace AvisoDeCotacaoPorEmail
{
    class Program
    {
        private static FileSystemWatcher _fileWatcher = new FileSystemWatcher();
        static Csv csv = new Csv();

        private static string csvPath = csv.CsvPath;

        private static void Main(string[] args)
        {
            ServiceBase.Run(new Service());

        }
    }
}
