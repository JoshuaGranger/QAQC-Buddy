using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using QAQC_Buddy.Models;

namespace QAQC_Buddy.Misc
{
    static class CoverSheet
    {
        public static string Generate(IEnumerable<Document> documents)
        {
            // Get included document names
            List<string> names = new List<string>();
            foreach(Document d in documents)
            {
                names.Add(d.ShortFileName);
            }

            // Check that the cover sheet template exists
            if (!File.Exists(Globals.PathCover))
            {
                Globals.ShowMsg("You selected to include a cover sheet; however, the cover sheet template could not be found.\n\n" +
                    $"Expected path of template:\n{Globals.PathCover}", "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                return null;
            }

            // Check that the user did not try to include too many documents
            if(names.Count > 24)
            {
                Globals.ShowMsg($"The maximum number of documents allowed on the cover sheet is 24. You have selected {names.Count} documents. A cover sheet will not be generated.", "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                return null;
            }

            // Assemble file paths
            string targetFilePDF = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.pdf";
            string targetPathPDF = Globals.PathFiles + targetFilePDF;

            // Verify that the desired output folder exists; create the directory if not
            try
            {
                if (!System.IO.Directory.Exists(Globals.PathFiles))
                    System.IO.Directory.CreateDirectory(Globals.PathFiles);
            }
            catch (Exception e)
            {
                Misc.Globals.ShowMsg($"The directory {Globals.PathFiles} does not seem to exist, and an error occurred while trying to create it. Do you have sufficient permissions?\n\nError:\n{e.Message}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Open the document
            using (PdfDocument document = PdfReader.Open(Globals.PathCover, PdfDocumentOpenMode.Modify))
            {
                // Get the page
                var page = document.Pages[0];

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Create a font
                XFont font = new XFont("Consolas", 10, XFontStyle.Regular);

                // Draw the text on the graphic
                for (int i = 0; i < names.Count; i++)
                {
                    // Establish the string length and cut it to maxLen if it is longer
                    int maxLen = 35;
                    int actLen = names[i].Length > maxLen ? maxLen : names[i].Length;

                    // Write text
                    gfx.DrawString(names[i].Substring(0, actLen), font, XBrushes.Black, new XRect(20, 220 + (i * 18.29), 266, 40), XStringFormats.CenterLeft);
                }

                // Save the document
                document.Save(targetPathPDF);
                return targetPathPDF;
            }
        }
    }
}
