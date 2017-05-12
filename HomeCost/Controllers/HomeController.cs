using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult HomeCost(HomeCost.Models.Home_Cost curHomeCost)
        {
            ViewBag.Message = "Cost Input Page.";
            return View();
        }

        public JsonResult Save(HomeCost.Models.Home_Cost curHomeCost)
        {
            String result;
            HomeCostHandle curHomeCostHandle = new HomeCostHandle();
            try
            {
                // _ITProjectService.SaveNewProjectInfo(projectInfo);
                curHomeCostHandle.SaveCostInfoToDB(curHomeCost);
                result = "保存成功";
                return Json(result);
            }
            catch (Exception ex)
            {
                result = "保存失败" + ex.Message;
                return Json(result);
            }
        }
        public ActionResult AngularPage()
        {
            return View();
        }
    }
}