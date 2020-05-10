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
    public class CoverSheet
    {
        public List<string> DocumentNames { get; set; }
        public string Lockbox { get; set; }
        public string Date { get; set; }
        public string EquipmentDesc { get; set; }
        public string WorkOrder { get; set; }
        public string DocumentPath { get; set; }

        public CoverSheet()
        {
            DocumentNames = new List<string>();
            Lockbox = Date = EquipmentDesc = WorkOrder = DocumentPath = "";
        }

        public string Generate(IEnumerable<Document> documents)
        {
            // Get included document names
            foreach(Document d in documents)
            {
                DocumentNames.Add(d.ShortFileName);
            }

            // Check that the cover sheet template exists
            if (!File.Exists(Globals.PathCover))
            {
                Globals.ShowMsg("You selected to include a cover sheet; however, the cover sheet template could not be found.\n\n" +
                    $"Expected path of template:\n{Globals.PathCover}", "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                return null;
            }

            // Check that the user did not try to include too many documents
            if(DocumentNames.Count > 24)
            {
                Globals.ShowMsg($"The maximum number of documents allowed on the cover sheet is 24. You have selected {DocumentNames.Count} documents. A cover sheet will not be generated.", "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

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

            // Prompt the user for some cover sheet details
            CoverDialogWindow cvrDlg = new CoverDialogWindow(this);
            cvrDlg.Owner = Application.Current.MainWindow;
            cvrDlg.ShowDialog();

            // Open the document
            using (PdfDocument document = PdfReader.Open(Globals.PathCover, PdfDocumentOpenMode.Modify))
            {
                // Get the PdfDocument page for the cover sheet
                var page = document.Pages[0];

                // Get an XGraphics object for drawing and create a font
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font_docs = new XFont("Consolas", 10, XFontStyle.Regular);
                XFont font_hdr = new XFont("Consolas", 12, XFontStyle.Regular);

                // Draw the Lockbox/set #
                gfx.DrawString(Lockbox, font_hdr, XBrushes.Black, new XRect(120, 119, 1, 1), XStringFormats.CenterLeft);

                // Draw the Date
                gfx.DrawString(Date, font_hdr, XBrushes.Black, new XRect(320, 119, 1, 1), XStringFormats.CenterLeft);

                // Draw the Work Order #
                gfx.DrawString(WorkOrder, font_hdr, XBrushes.Black, new XRect(130, 144, 1, 1), XStringFormats.CenterLeft);

                // Draw the Equipment Description
                gfx.DrawString(EquipmentDesc, font_hdr, XBrushes.Black, new XRect(105, 168, 1, 1), XStringFormats.CenterLeft);

                // Draw the text on the graphic for the documents list
                for (int i = 0; i < DocumentNames.Count; i++)
                {
                    // Establish the string length and cut it to maxLen if it is longer
                    // since there is a limited number of characters that can fit in the box
                    int maxLen = 35;
                    int actLen = DocumentNames[i].Length > maxLen ? maxLen : DocumentNames[i].Length;

                    // Write text
                    gfx.DrawString(DocumentNames[i].Substring(0, actLen), font_docs, XBrushes.Black, new XRect(21.5, 220 + (i * 18.29), 266, 40), XStringFormats.CenterLeft);
                }

                // Save the document
                document.Save(targetPathPDF);
                return targetPathPDF;
            }
        }
    }
}
