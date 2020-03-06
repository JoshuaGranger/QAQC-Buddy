using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using QAQC_Buddy.Models;

namespace QAQC_Buddy.Misc
{
    static class CoverSheet
    {
        public static PdfDocument Generate(List<Document> documents)
        {
            // Get included document names
            List<string> names = new List<string>();
            foreach(Document d in documents)
            {
                names.Add(d.ShortFileName);
            }

            // Check that the cover sheet template exists
            if (!File.Exists(Globals.PathCover))
                return null;

            // Open spreadsheet and add the items
            if (names != null)
            {
                
            }

            // Return result
            return new PdfDocument();
        }
    }
}
