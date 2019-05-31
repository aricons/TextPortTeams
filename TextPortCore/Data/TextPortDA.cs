using System;
using System.Configuration;

//using Microsoft.EntityFrameworkCore;

namespace TextPortCore.Data
{
    public partial class TextPortDA : IDisposable
    {
        private TextPortContext _context;

        // Constructors
        //public TextPortDA(TextPortContext context)
        //{
        //    this._context = context;
        //}

        public TextPortDA()
        {
            this._context = new TextPortContext();
        }

        // Public methods
        public int SaveChanges()
        {
            return this._context.SaveChanges();
        }

        #region "Disposal"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        #endregion
    }
}
