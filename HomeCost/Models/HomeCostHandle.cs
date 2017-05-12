using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCost.Models
{
    public class HomeCostHandle
    {
        public void SaveCostInfoToDB(HomeCost.Models.Home_Cost curCost)
        {
            using (masterEntities curDBContext = new masterEntities())
            {
                curCost.ID = Guid.NewGuid().ToString();
                curCost.CreateBy = "zpf";
                curDBContext.Home_Cost.Add(curCost);
                curDBContext.SaveChanges();
            }
        }
    }
}