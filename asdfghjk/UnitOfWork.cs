using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Transactions;

namespace asdfghjk
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Members

        private DbContext dbContext;
        private bool disposed = false;

        /// <summary>
        /// İşlemlerde hata oluşusa bu liste doldurulur.
        /// </summary>
        public List<string> ErrorMessageList = new List<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Açılan veri bağlantısı.
        /// </summary>
        public DbContext DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = new MasterContext();
                }
                return dbContext;
            }
            set => dbContext = value;
        }


        #endregion

        #region Constructre

        /// <summary>
        /// UnitOfWork başlangıcı 
        /// </summary>
        /// <param name="authenticatedUser">Yalıtılacak Kullanıcı</param>
        /// <param name="databaseType">kullanılacak veritabanı</param>
        public UnitOfWork()
        {
        }

        #endregion

        #region IUnitOfWork Members

        /// <summary>
        /// Repository instance'ı başlatmak için kullanılır.
        /// </summary>
        /// <typeparam name="T">Veri Tabanı Tür Nesnesi</typeparam>
        /// <returns>Tür nesnesi ile ilgili Repository</returns>
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(DbContext);
        }

        /// <summary>
        /// Değişiklikleri kaydet.
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            try
            {
                int result = 0;
                using (TransactionScope tScope = new TransactionScope())
                {
                    result = DbContext.SaveChanges();
                    tScope.Complete();
                }
                return result;
            }
            catch (ValidationException ex)
            {
                string errorString = ex.Message;
                LogTrackedEntries(errorString);
                return -1;
            }
            catch (DbUpdateException ex)
            {
                string errorString = ex.Message;
                if (ex.InnerException != null)
                {
                    errorString += ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorString += ex.InnerException.InnerException.Message;
                        ErrorMessageList.Add(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        ErrorMessageList.Add(ex.InnerException.Message);
                    }
                }
                LogTrackedEntries(errorString);
                return -1;
            }
            catch (Exception ex)
            {
                LogTrackedEntries(ex.Message);
                ErrorMessageList.Add(ex.Message);
                return -1;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Nesneyi bellekten atmadan önce bağlantıyı kapatır.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                    DbContext = null;
                }
            }

            disposed = true;
        }

        /// <summary>
        /// IDisposable Design Pattern instance'ı
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void LogTrackedEntries(string additionalData)
        {
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries = DbContext.ChangeTracker.Entries();
            StringBuilder entityStringBuilder = new StringBuilder();
            entityStringBuilder.AppendLine(additionalData);
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in entries)
            {
                entityStringBuilder.AppendLine("-----------------ENTITYDATA----------------------");
                entityStringBuilder.AppendLine(JsonConvert.SerializeObject(entry.Entity));
                entityStringBuilder.AppendLine("-----------------ENTITYDATA----------------------");
            }

            //ToDo : log this object
            //Logger.Info(entityStringBuilder.ToString());
        }


        #endregion
    }
}
