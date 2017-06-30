using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Config;
using Quartz;
using Quartz.Impl;

namespace ConsoleApplicationQuartz
{
    class Program
    {

        private static void Main(string[] args)
        {
            var configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(configFile);
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            log4net.ILog mylogger = log4net.LogManager.GetLogger(type);
            try
            {

                //            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter
                //            {
                //                Level = Common.Logging.LogLevel.Info
                //};
                // Grab the Scheduler instance from the Factory 
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

                // and start it off
                scheduler.Start();
                mylogger.Info("系统启动");

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(2)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);
                mylogger.Info("设置job和trigger");
                // some sleep to show what's happening
                Thread.Sleep(TimeSpan.FromSeconds(10));

                // and last shut down the scheduler when you are ready to close your program
                scheduler.Shutdown();
                mylogger.Info("系统关闭");
            }

            catch (SchedulerException se)
            {
                Console.WriteLine(se);
                mylogger.Error("系统错误:" + se.ToString());
            }
            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
    }
    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Greetings from HelloJob-{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
        }
    }

}
