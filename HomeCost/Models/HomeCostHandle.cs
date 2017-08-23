using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop;

namespace HomeCost.Models
{
    public class HomeCostHandle: IHomeCostHandle
    {
        public void SaveCostInfoToDb(HomeCost.Models.Home_Cost curCost)
        {
            using (masterEntities curDBContext = new masterEntities())
            {
                curCost.ID = Guid.NewGuid().ToString();
                //curCost.CreateBy = "zpf";
                curDBContext.Home_Cost.Add(curCost);
                curDBContext.SaveChanges();
            }
        }

        public bool CheckLoginValid(Home_User currentUser)
        {
            bool bResult = false;
            using (masterEntities currentDBContext = new masterEntities())
            {
                //currentDBContext.
            }
            return bResult;
        }

        public List<CostTypeModel> GetCostTypeList()
        {
            List<CostTypeModel> curCostList = new List<CostTypeModel>();
            using (var curDbContext = new masterEntities())
            {
                var curList = (from costtype in curDbContext.Cost_Type
                    select new CostTypeModel()
                    {
                        CostTypeID = costtype.CostTypeID,
                        CostTypeName = costtype.CostTypeName
                    }).ToList();
                curCostList.AddRange(curList.OrderBy(x => x.CostTypeID));
            }
            return curCostList;
        }

    }
}