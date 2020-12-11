using System;
using System.Linq;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public List<CensoredWord> GetCensoredWords()
        {
            try
            {
                return _context.CensoredWords.ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("CensorDA.GetCensoredWords", ex);
            }
            return null;
        }

        #endregion

        #region "Insert Methods"
        #endregion

        #region "Delete Methods"
        #endregion
    }
}
