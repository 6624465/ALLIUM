﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using NetStock.Contract;
using System.Data.Common;



namespace NetStock.DataFactory
{
    public class ARTransactionDAL
    {
        private Database db;
        private DbTransaction currentTransaction = null;
        private DbConnection connection = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ARTransactionDAL()
        {

            db = DatabaseFactory.CreateDatabase("netStock");

        }

        #region IDataFactory Members

        public List<ARTransaction> GetList()
        {
            return db.ExecuteSprocAccessor(DBRoutine.LISTARTRANSACTION, MapBuilder<ARTransaction>.BuildAllProperties()).ToList();
        }

        public bool Save<T>(T item, DbTransaction parentTransaction) where T : IContract
        {
            currentTransaction = parentTransaction;
            return Save(item);

        }




        public bool Save<T>(T item) where T : IContract
        {
            var result = 0;

            var artransaction = (ARTransaction)(object)item;

            if (currentTransaction == null)
            {
                connection = db.CreateConnection();
                connection.Open();
            }

            var transaction = (currentTransaction == null ? connection.BeginTransaction() : currentTransaction);


            try
            {
                var savecommand = db.GetStoredProcCommand(DBRoutine.SAVEARTRANSACTION);


                db.AddInParameter(savecommand, "DebtorCode", System.Data.DbType.String, artransaction.DebtorCode);
                db.AddInParameter(savecommand, "MatchDocumentType", System.Data.DbType.String, artransaction.MatchDocumentType);
                db.AddInParameter(savecommand, "MatchDocumentNo", System.Data.DbType.String, artransaction.MatchDocumentNo);
                db.AddInParameter(savecommand, "DocumentType", System.Data.DbType.String, artransaction.DocumentType);
                db.AddInParameter(savecommand, "DocumentNo", System.Data.DbType.String, artransaction.DocumentNo);
                db.AddInParameter(savecommand, "DocumentTypeSequence", System.Data.DbType.Int16, artransaction.DocumentTypeSequence);
                db.AddInParameter(savecommand, "DocumentDate", System.Data.DbType.DateTime, artransaction.DocumentDate);
                db.AddInParameter(savecommand, "CreditTermInDays", System.Data.DbType.Int16, artransaction.CreditTermInDays);
                db.AddInParameter(savecommand, "CurrencyCode", System.Data.DbType.String, artransaction.CurrencyCode);
                db.AddInParameter(savecommand, "ExchangeRate", System.Data.DbType.Decimal, artransaction.ExchangeRate);
                db.AddInParameter(savecommand, "BaseAmount", System.Data.DbType.Decimal, artransaction.BaseAmount);
                db.AddInParameter(savecommand, "LocalAmount", System.Data.DbType.Decimal, artransaction.LocalAmount);
                db.AddInParameter(savecommand, "AccountDate", System.Data.DbType.DateTime, artransaction.AccountDate);



                result = db.ExecuteNonQuery(savecommand, transaction);

                if (currentTransaction == null)
                    transaction.Commit();

            }
            catch (Exception)
            {
                if (currentTransaction == null)
                    transaction.Rollback();

                throw;
            }

            return (result > 0 ? true : false);

        }

        public bool Delete<T>(T item) where T : IContract
        {
            var result = false;
            var artransaction = (ARTransaction)(object)item;

            var connnection = db.CreateConnection();
            connnection.Open();

            var transaction = connnection.BeginTransaction();

            try
            {
                var deleteCommand = db.GetStoredProcCommand(DBRoutine.DELETEARTRANSACTION);

                db.AddInParameter(deleteCommand, "DebtorCode", System.Data.DbType.String, artransaction.DebtorCode);
                db.AddInParameter(deleteCommand, "DocumentNo", System.Data.DbType.String, artransaction.DocumentNo);
                db.AddInParameter(deleteCommand, "DocumentType", System.Data.DbType.String, artransaction.DocumentType);
                db.AddInParameter(deleteCommand, "MatchDocumentNo", System.Data.DbType.String, artransaction.MatchDocumentNo);
                db.AddInParameter(deleteCommand, "MatchDocumentType", System.Data.DbType.String, artransaction.MatchDocumentType);


                result = Convert.ToBoolean(db.ExecuteNonQuery(deleteCommand, transaction));

                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }

            return result;
        }

        public IContract GetItem<T>(IContract lookupItem) where T : IContract
        {
            var item = ((ARTransaction)lookupItem);

            var artransactionItem = db.ExecuteSprocAccessor(DBRoutine.SELECTARTRANSACTION,
                                                    MapBuilder<ARTransaction>.BuildAllProperties(),
                                                    item.DebtorCode,item.MatchDocumentType,item.MatchDocumentNo,item.DocumentType,item.DocumentNo).FirstOrDefault();
            return artransactionItem;
        }

        #endregion






    }
}

