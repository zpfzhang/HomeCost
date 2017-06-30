using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace HomeCost.Models
{
    public static class AuthoriyService
    {
        public static void SetUser(Home_User currentUser)
        {
            HttpContext.Current.Session["LoginAccount"] = currentUser;
        }

        public static Home_User GetUser()
        {
            var currentUser = HttpContext.Current.Session["LoginAccount"] as Home_User;
            if (currentUser == null)
            {
                currentUser = new Home_User
                {
                    UserID = Guid.NewGuid().ToString(),
                    UserLoginAccount = "",
                    UserName = ""
                };

            }
            return currentUser;
        }

        public static bool ValidUser(string userLoginAccount, string userPwd)
        {
            Home_User currentUser;
            var bResult = true;
            using (var currentEntities = new masterEntities())
            {
                currentUser = currentEntities.Home_User.FirstOrDefault(
                    x => x.UserLoginAccount == userLoginAccount && x.UserPwd == userPwd);

            }
            if (currentUser == null)
            {
                bResult = false;
            }
            else
            {
                SetUser(currentUser);
                bResult = true;
            }
            return bResult;

        }

        public static void LogInfo(string infoMsg, string strSource)
        {
            String logPath=ConfigurationManager.AppSettings["LogPath"];
            string fileName = logPath+ "\\HomeCostLog"+DateTime.Now.Date.ToString("yyyy-MM-dd")+".txt";

            FileStream fs = new FileStream(fileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("info:{0};Source:{1};DateTime:{2}",infoMsg,strSource,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sw.Close();
            fs.Close();
        }
    }
}