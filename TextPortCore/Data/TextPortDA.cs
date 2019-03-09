﻿using System;

namespace TextPortCore.Data
{
    public partial class TextPortDA : IDisposable
    {
        private readonly TextPortContext _context;

        public TextPortDA(TextPortContext context)
        {
            this._context = context;
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
