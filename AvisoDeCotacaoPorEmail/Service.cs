using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AvisoDeCotacaoPorEmail
{
    public class Service : ServiceBase
    {
        // Fields
        private const string _logFileLocation = @"C:\temp\servicelog.txt";
        private Email emailSender = new Email();
        private bool cancellationToken = false;
        private List<Thread> threadsToWait = new List<Thread>();

        // Methods
        private List<Stock> GetStocks() => 
            new List<Stock> { new Stock("PETR4", 20.0, 22.0) };

        private void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.UtcNow.ToString() + " : " + logMessage + Environment.NewLine);
        }

        protected override void OnPause()
        {
            Log("Pausing");
            base.OnPause();
        }

        public void OnRun()
        {
            foreach (Stock stock in GetStocks())
            {
                StartStockThread(stock);
            }
        }

        protected override void OnStart(string[] args)
        {
            Log("Starting");
            emailSender.SendEmail("williamslacerda@gmail.com", "Teste", "Aviso").Wait();
            Log("First Email Sent");
            emailSender.SendEmail("williamslacerda@gmail.com", "Teste", "Aviso 2").Wait();
            Log("Second Email Sent");
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Log("Stopping");
            this.cancellationToken = true;
            this.threadsToWait.ForEach(t => t.Join());

            Log("Serviço encerrado");
            base.OnStop();
        }
        
        private void StartStockThread(Stock stock)
        {
            ThreadStart starter = delegate { this.RunJobThread(stock); };
            Thread t = new Thread(starter);
            t.Start();
            this.threadsToWait.Add(t);
        }
        
        private void RunJobThread(Stock stock)
        {
            bool sendSuccess = false;
            do
            {
                int sleep = 1000;
                try
                {
                    sendSuccess = CheckValueAndSendMail(stock);
                }
                catch (Exception e)
                {
                    // _logger.Error(e, "Thread {0}@{1} :: Erro ao chamar Motor: {2}", job.nome, threadIndex);
                }
                
                Thread.Sleep(sleep);
            } while (this.cancellationToken != true && !sendSuccess);
            
        }
        private bool CheckValueAndSendMail(Stock s)
        {
            s.UpdateCurValue();
            if (s.CurValue >= s.MaxValue)
            {
                emailSender.SendEmail("williamslacerda@gmail.com", "Acima", "Aviso Max").Wait();
                return true;
            }
            if (s.CurValue <= s.MinValue)
            {
                emailSender.SendEmail("williamslacerda@gmail.com", "Abaixo", "Aviso Min").Wait();
                return true;
            }

            return false;
        }
    }
}