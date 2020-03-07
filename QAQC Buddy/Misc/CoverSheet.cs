using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Pdf.IO;
using ClosedXML.Excel;
using Spire.Xls;
using QAQC_Buddy.Models;

namespace QAQC_Buddy.Misc
{
    static class CoverSheet
    {
        public static PdfSharp.Pdf.PdfDocument Generate(IEnumerable<Document> documents)
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
                    $"Expected Path: {Globals.PathCover}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return null;
            }

            // Check that the user did not try to include too many documents
            if(names.Count > 23)
            {
                Globals.ShowMsg("It appears that you have included more than 24 documents. The cover sheet cannot handle this many documents. Please use less documents if you want an" +
                    " automatically generated cover sheet.", "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                return null;
            }

            // Output File Information
            // Assemble output filename and path
            string targetFileExcel = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.xlsx";
            string targetPathExcel = Globals.PathFiles + targetFileExcel;
            string targetFilePDF = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.pdf";
            string targetPathPDF = Globals.PathFiles + targetFilePDF;

            // Check that the folder exists
            if (!System.IO.Directory.Exists(Globals.PathFiles))
                System.IO.Directory.CreateDirectory(Globals.PathFiles);

            // Open cover spreadsheet and add the items using ClosedXML
            if (names != null)
            {
                using (var wb = new XLWorkbook(Globals.PathCover))
                {
                    var ws = wb.Worksheet(1);

                    for (int i = 0; i < names.Count; i++)
                        ws.Cell(i + 7, 1).Value = names[i];

                    wb.SaveAs(targetPathExcel);
                }
            }

            // Convert the new spreadsheet to PDF using FreeSpire
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(targetPathExcel);
            workbook.SaveToFile(targetPathPDF, Spire.Xls.FileFormat.PDF);

            // Return result
            var output = PdfReader.Open(targetPathPDF, PdfDocumentOpenMode.Import);
            return output;
        }
    }
}
