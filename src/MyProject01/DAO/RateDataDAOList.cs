using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DAO
{
    public class RateDataDAOList : List<RateDataControlDAO>
    {
        public RateDataDAOList()
        {

            RateDataControlDAO[] daoArr = RateDataControlDAO.GetList();
            if (daoArr == null)
                return;
            foreach( RateDataControlDAO dao in daoArr)
            {
                Add(dao);
            }
        }

        public RateDataControlDAO Get(string name)
        {
            foreach(RateDataControlDAO dao in this)
            {
                if( dao.Name.CompareTo(name) == 0)
                {
                    dao.Update();
                    return dao;
                }

            }
           return null;
        }

        public RateDataControlDAO Add(string name, string symbolName, int timeFrame, DateTime startTime)
        {
            RateDataControlDAO dao = Get(name);
            if (dao == null)
            {
                dao = RateDataControlDAO.Create(name, symbolName, timeFrame, startTime);
                this.Add(dao);
            }
            return dao;
        }

        new public void Remove(RateDataControlDAO dao)
        {
            dao.Remove();
            Remove(dao);
        }

        public void Remove(string name)
        {
            RateDataControlDAO targetDao = null;
            foreach (RateDataControlDAO dao in this)
            {
                if (dao.Name.CompareTo(name) == 0)
                {
                    targetDao = dao;
                }
            }

            if (targetDao == null)
                return;
            Remove(targetDao);
        }


    }
}
