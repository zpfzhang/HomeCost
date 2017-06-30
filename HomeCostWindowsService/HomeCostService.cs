using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Quartz;
using Quartz.Impl;

namespace HomeCostWindowsService
{
    public partial class HomeCostService : ServiceBase
    {
        private log4net.ILog mylogger;
        // Grab the Scheduler instance from the Factory 
        private IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        public HomeCostService()
        {
            InitializeComponent();
            var configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(configFile);
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            mylogger = log4net.LogManager.GetLogger(type);
        }

        protected override void OnStart(string[] args)
        {
            int nIntervalHours = Convert.ToInt16(ConfigurationManager.AppSettings["SendIntervalInHours"]);
            mylogger.Info("HomeCostWindowsService Start");
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\HomeCost\\winServicelog.txt", true))
            //{
            //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start.");
            //}
            

            // and start it off
            scheduler.Start();
            mylogger.Info("系统启动");

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ExportAndMailJob>()
                .WithIdentity("HomeCostJob", "HomeCostGroup")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("HomeCostTrigger", "HomeCostGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(nIntervalHours)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            scheduler.ScheduleJob(job, trigger);
            mylogger.Info("设置job和trigger");
            scheduler.Start();
            mylogger.Info("系统Job启动");
        }

        protected override void OnStop()
        {
            scheduler.Shutdown();
            mylogger.Info("HomeCostWindowsService Stop");
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\HomeCost\\winServicelog.txt", true))
            //{
            //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Stop.");
            //}
        }
    }
}
