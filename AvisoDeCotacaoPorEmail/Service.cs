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
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace AvisoDeCotacaoPorEmail
{
    public class Service : ServiceBase
    {
        // Fields
        
        private Email emailSender = new Email();
        private bool cancellationToken = false;
        private List<Thread> threadsToWait = new List<Thread>();
        static Csv csv = new Csv();
        private static string csvPath = csv.CsvPath;
        private FileSystemWatcher _fileWatcher = new FileSystemWatcher();
        IConfigurationRoot config = new Configs().GetAppSettings();
        Logger log = LogManager.GetCurrentClassLogger();

        enum CHECK_CODES
        {
            ERROR,
            SUCCESS,
            NEUTRAL
        }
        
        protected override void OnPause()
        {
            log.Debug("Pausing");
            base.OnPause();
        }

        public void OnRun()
        {
            foreach (Stock stock in csv.GetStocks())
            {
                Thread.Sleep(1000);
                StartStockThread(stock);
            }
        }

        protected override void OnStart(string[] args)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            log.Debug("Starting");
            _fileWatcher = new FileSystemWatcher( Path.GetDirectoryName(csvPath) );
            _fileWatcher.Created += new FileSystemEventHandler(FileChanged);
            _fileWatcher.Deleted += new FileSystemEventHandler(FileChanged);
            _fileWatcher.Changed += new FileSystemEventHandler(FileChanged);
            _fileWatcher.EnableRaisingEvents = true;
            base.OnStart(args);
            OnRun();
        }

        protected override void OnStop()
        {
            log.Debug("Stopping");
            this.cancellationToken = true;
            this.threadsToWait.ForEach(t => t.Join());
            _fileWatcher.Created -= FileChanged;
            _fileWatcher.Deleted -= FileChanged;
            _fileWatcher.Changed -= FileChanged;
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();

            log.Debug("Serviço encerrado");
            base.OnStop();
        }
        
        private void FileChanged( object sender, FileSystemEventArgs e )
        {
            if (e.FullPath == csvPath)
            {
                log.Debug("List changed, restarting threads");
                this.cancellationToken = true;
                this.threadsToWait.ForEach(t =>
                {
                    t.Join();
                    log.Info($"Thread {t.Name} stopped");
                });
                this.threadsToWait = new List<Thread>();
                this.cancellationToken = false;
                OnRun();
                log.Debug(
                    $"{e.Name} with path {e.FullPath} has been {e.ChangeType} at {DateTime.Now:MM/dd/yy H:mm:ss}");
            }
        }
        
        private void StartStockThread(Stock stock)
        {
            ThreadStart starter = delegate { this.RunJobThread(stock); };
            Thread t = new Thread(starter);
            t.Name = stock.Symbol;
            t.Start();
            this.threadsToWait.Add(t);
            log.Debug($"Thread for {stock.Symbol} started with Min = {stock.MinValue} and Max = {stock.MaxValue}");
        }
        
        private void RunJobThread(Stock stock)
        {
            CHECK_CODES sendSuccess = CHECK_CODES.NEUTRAL;
            do
            {
                int sleep = Int32.Parse(config["TimeBetweenChecks"]);
                try
                {
                    sendSuccess = CheckValueAndSendMail(stock);
                    if (sendSuccess == CHECK_CODES.SUCCESS)
                    {
                        log.Debug($"Success {stock.Symbol}");
                        // Caso sucesso, esperar 1h antes de verificar novamente
                        sleep = Int32.Parse(config["TimeBetweenChecksIfSuccess"]);
                    } else if (sendSuccess == CHECK_CODES.ERROR)
                    {
                        log.Error($"Error detected, sleeping for {config["TimeBetweenChecksIfError"]}ms");
                        sleep = Int32.Parse(config["TimeBetweenChecksIfError"]);

                    }
                }
                catch (Exception e)
                {
                    log.Debug("Erro ao inicializar Thread "+e);
                }
                log.Debug($"Sleeping for {sleep / 1000} seconds");
                Thread.Sleep(sleep);
            } while (this.cancellationToken != true);
            
        }
        private CHECK_CODES CheckValueAndSendMail(Stock s)
        {
            // TODO tratar melhor os retornos com uma estrutura para os tipos de retorno (ERRO, SUCESSO, NEUTRO)
            try
            {
                s.UpdateCurValue();
            }
            catch (Exception e)
            {
                return CHECK_CODES.ERROR;
            }

            log.Debug($"Stock {s.Symbol} value updated to ${s.CurValue}");
            if (s.CurValue >= s.MaxValue)
            {
                var message =
                    $"A ação {s.Symbol} atingiu o valor de ${s.CurValue:N2}%! E o valor máximo configurado era ${s.MaxValue:N2}%";
                log.Debug(message);
                emailSender.SendEmail(config["DestinationEmail"], message, "Aviso De Cotação!").Wait();
                return CHECK_CODES.SUCCESS;
            }
            if (s.CurValue <= s.MinValue)
            {
                var message =
                    $"A ação {s.Symbol} atingiu o valor de ${s.CurValue:N2}%! E o valor mínimo configurado era ${s.MaxValue:N2}%";
                log.Debug(message);
                emailSender.SendEmail(config["DestinationEmail"], message, "Aviso De Cotação!").Wait();
                return  CHECK_CODES.SUCCESS;
            }

            return  CHECK_CODES.NEUTRAL;
        }
    }
}