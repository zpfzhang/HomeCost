using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace HomeCostWindowsService
{
    public class AdoHandle
    {
        private log4net.ILog mylogger = log4net.LogManager.GetLogger(typeof(ExportAndMailJob));
        /// <summary> 
        /// 将数据集中的数据导出到EXCEL文件 
        /// </summary> 
        /// <param name="dataSet">输入数据集</param> 
        /// <param name="isShowExcle">是否显示该EXCEL文件</param> 
        /// <returns></returns> 
        public bool DataSetToExcel(DataSet dataSet, bool isShowExcle)
        {
            DataTable dataTable = dataSet.Tables[0];
            int rowNumber = dataTable.Rows.Count;//不包括字段名 
            int columnNumber = dataTable.Columns.Count;
            int colIndex = 0;

            if (rowNumber == 0)
            {
                return false;
            }

            //建立Excel对象 
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Application.Workbooks.Add(true); 
            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            excel.Visible = isShowExcle;
            Microsoft.Office.Interop.Excel.Range excelRange;

            //生成字段名称 
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.ColumnName;
            }

            //Format date
            excelRange = worksheet.get_Range("B2","B"+Convert.ToString(1+rowNumber)); //range of setting data
            excelRange.NumberFormat = "yyyy-MM-dd;@";

            excelRange = worksheet.get_Range("H2", "H" + Convert.ToString(1 + rowNumber));//range of setting data
            excelRange.NumberFormat = "yyyy-MM-dd HH:mm:ss;@";

            //填充数据 
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    excel.Cells[i + 2, j + 1] = dataTable.Rows[i].ItemArray[j];
                }
            }

            //Autofit column
            excel.Columns.AutoFit();

            String mailAttachPath = ConfigurationManager.AppSettings["MailAttchPath"];
            String fileName = AppDomain.CurrentDomain.BaseDirectory + mailAttachPath +"\\HomeCostList"+ DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:ffff").Replace(':', '_') + ".xlsx";
            
            try
            {
                workbook.SaveAs(fileName);
                workbook.Saved = true;
                excel.UserControl = false;
            }
            catch (Exception exception)
            {
                mylogger.Info("Exception in Excel Save"+exception.Message);
            }
            finally
            {
                workbook.Close(Microsoft.Office.Interop.Excel.XlSaveAction.xlSaveChanges);
                excel.Quit();               
            }

            try
            {
                Sendmail(fileName);
            }
            catch (Exception exception)
            {

                mylogger.Info("Exception in Send Mail"+exception.Message); 
            }
            mylogger.Info("Send mail success"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return true;
        }

        //read Home_cost from DB into dataset
        public void GetHomeCost()
        {
            String connString = ConfigurationManager.ConnectionStrings["AdoConnection"].ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Home_Cost WHERE CreateDate >= @CREATEDATE order by CostDate desc", sqlConnection))
                {
                    //Note that there is no call to Open the adapter. When the Fill method
                    // is called, the Adapter handles opening and closing the connection
                    sqlCommand.Parameters.AddWithValue("@CREATEDATE", "2017-06-13");//the date to begin input real data
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        DataSet currentSet = new DataSet("CurrentSet");
                        sqlAdapter.Fill(currentSet);

                        DataSetToExcel(currentSet, true);
                    }
                }
            }
        }

        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <returns></returns>
        public bool Sendmail(String strAttachfile)
        {
            String mailFrom = ConfigurationManager.AppSettings["MailFrom"];
            String mailTo = ConfigurationManager.AppSettings["MailTo"];
            String mailTo2 = ConfigurationManager.AppSettings["MailTo2"];
            MailMessage mm = new MailMessage(mailFrom, mailTo);
            if(mailTo2.Length>0)
                mm.CC.Add(mailTo2);
            mm.Subject = "家庭收支统计-"+DateTime.Now.ToString("yyyy-MM-dd");
            mm.Body = "看看我们花了多少银子吧.....";
            Attachment dataAttachment = new Attachment(strAttachfile);
            mm.Attachments.Add(dataAttachment);
            SmtpClient sc = new SmtpClient("smtp.qq.com");
            sc.Port = 587;
            sc.UseDefaultCredentials = false;
            sc.EnableSsl = true;
            sc.Credentials = new NetworkCredential(mailFrom, "vbscoxdvhrgpbhbh");//QQ smtp 授权码,在邮箱中设置
            try
            {
                sc.Send(mm);
                dataAttachment.Dispose();
                return true;
            }
            catch (Exception err)
            {
                mylogger.Info("Exception in Send Mail"+err.Message);
                return false;
            }
        }
    }
}