﻿using NetStock.Contract;
using NetStock.DataFactory;
using System.Collections.Generic;

namespace NetStock.BusinessFactory
{
    public class APFileDetailBO
    {
        private NetStock.DataFactory.APFileDetailDAL  apfiledetailDAL;
        public APFileDetailBO() {

            apfiledetailDAL = new APFileDetailDAL();
        }

        public List<APFileDetail> GetList(string documentNo)
        {
            return apfiledetailDAL.GetList(documentNo);
        }


        public bool SaveAPFileDetail(APFileDetail newItem)
        {

            return apfiledetailDAL.Save(newItem);

        }

        public bool DeleteAPFileDetail(APFileDetail item)
        {
            return apfiledetailDAL.Delete(item);
        }

        public APFileDetail GetAPFileDetail(APFileDetail item)
        {
            return (APFileDetail)apfiledetailDAL.GetItem<APFileDetail>(item);
        }

    }
}
