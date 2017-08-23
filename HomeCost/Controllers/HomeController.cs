using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using HomeCost.Models;

namespace HomeCost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "The system is used for input the family cost and will send out the cost list to mailbox daily";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Please check below contact info:";

            return View();
        }

        public ActionResult HomeCost(HomeCost.Models.Home_Cost curHomeCost)
        {
            ViewBag.Message = "Cost Input Page.";
            return View();
        }

        public JsonResult Save(HomeCost.Models.Home_Cost curHomeCost)
        {
            String result="error";
            IHomeCostHandle curHomeCostHandle = new HomeCostHandle();
            try
            {
                // _ITProjectService.SaveNewProjectInfo(projectInfo);
                var curUser = AuthoriyService.GetUser();
                if (curUser.UserLoginAccount.IsEmpty())
                {
                    result = "errorNoUser";
                }
                else
                {
                    curHomeCost.CreateByUserID = curUser.UserID;
                    curHomeCost.CreateByUserAccount = curUser.UserLoginAccount;
                    curHomeCost.CreateDate = DateTime.Now;
                    curHomeCostHandle.SaveCostInfoToDb(curHomeCost);
                    result = "success";
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                AuthoriyService.LogInfo(ex.Message,"Save Cost");
                return Json(result);
            }
        }
        public ActionResult AngularPage()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult HomeCostLogin()
        {
            ViewBag.Message = "Home Cost Login Page.";
            return View();
        }


        public ActionResult HomeCostLogOff()
        {
            AuthoriyService.SetUser(null);
            return View("HomeCostLogin");
        }
        public JsonResult HomeUserValid(string userLoginAccount, string userPwd)
        {
            String result;
            IHomeCostHandle curHomeCostHandle = new HomeCostHandle();
            try
            {
                if (AuthoriyService.ValidUser(userLoginAccount, userPwd))
                {
                    result = "success";
                }
                else
                {
                    result = "error";
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                result = "error" + ex.Message;
                AuthoriyService.LogInfo(ex.Message,"User Valid");
                return Json(result);
            }
        }

        public JsonResult GetCostTypeList()
        {

            IHomeCostHandle curHomeCostHandle = new HomeCostHandle();
            var costList = curHomeCostHandle.GetCostTypeList();
            StringBuilder listInfo = new StringBuilder();
            listInfo.Append("[");
            foreach (var item in costList)
            {
                listInfo.Append("{name: '" + item.CostTypeName + "', value: '" + item.CostTypeID + "'},");
            }
            return Json(listInfo.ToString().TrimEnd(',') + "]", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportToExcel()
        {
            String result;
            ADOHandle curAdoHandle = new ADOHandle();
            curAdoHandle.GetHomeCost();
            result = "{\"result\":\"true\"}"; 
            return Json(result);
        }
    }
}