using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace HomeCostWindowsService
{
    class ExportAndMailJob:IJob
    {
        private log4net.ILog mylogger = log4net.LogManager.GetLogger(typeof(ExportAndMailJob));
        public void Execute(IJobExecutionContext context)
        {
            AdoHandle myAdoHandle = new AdoHandle();
            myAdoHandle.GetHomeCost();
            mylogger.Info("Export and Send mails-"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
        }
    }
}
