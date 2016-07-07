using MyProject01.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class MTData2Loader : DataLoader
    {
        RateDataControlDAO _rateDataCtrl;
        public MTData2Loader(string name)
        {
            _rateDataCtrl = RateDataControlDAO.GetByName(name);
            RateData[] dataArr = _rateDataCtrl.Get(0, _rateDataCtrl.Count);
            AddAll(dataArr);

            // DataValueAdjust();
        }

        private void AddAll(RateData[] buffer)
        {
            foreach (RateData data in buffer)
            {
                RateSet currRateSet;
                currRateSet = new RateSet(data);
                Add(currRateSet);
            }
        }
    }
}
