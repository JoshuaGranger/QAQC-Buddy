using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QAQC_Buddy.Models;
using System.Windows;

namespace QAQC_Buddy.Misc
{
    static class PDFMerge
    {
        // Merge the PDF files (sent as Document with valid FullPath property) and included a cover if selected
        public static void MergePDFs(IEnumerable<Document> pdfs, bool includeCover)
        {
            // Assemble output filename and path
            string targetFile = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.pdf";
            string targetPath = Globals.PathFiles + targetFile;

            // Check that the folder exists
            if (!System.IO.Directory.Exists(Globals.PathFiles))
                System.IO.Directory.CreateDirectory(Globals.PathFiles);

            // Check if the file exists. If so, add (2) to the end
            if (System.IO.File.Exists(targetPath))
                targetPath.Replace(".pdf", "(2).pdf");

            // Assemble document
            using (PdfDocument targetDoc = new PdfDocument())
            {
                if (pdfs.Any())
                {
                    // Generate cover sheet if selected
                    if (includeCover)
                    {
                        // Get the cover PDF path (result is either null or value)
                        CoverSheet coverSheet = new CoverSheet();
                        coverSheet.DocumentPath = coverSheet.Generate(pdfs);

                        if (coverSheet.DocumentPath != null)
                        {
                            PdfDocument cover = PdfReader.Open(coverSheet.DocumentPath, PdfDocumentOpenMode.Import);

                            if (cover != null)
                                targetDoc.AddPage(cover.Pages[0]);
                        }
                    }
                        
                    // Append each selected document
                    foreach (Document pdf in pdfs)
                        using (PdfDocument pdfDoc = PdfReader.Open(pdf.FullPath, PdfDocumentOpenMode.Import))
                            for (int i = 0; i < pdfDoc.PageCount; i++)
                                targetDoc.AddPage(pdfDoc.Pages[i]);

                    // Save and open the PDF file if there are pages
                    if (targetDoc.Pages.Count > 0)
                    {
                        if (!File.Exists(targetPath) || !IsFileinUse(new FileInfo(targetPath)))
                        {
                            // Save the file
                            try
                            {
                                targetDoc.Save(targetPath);
                            }
                            catch (Exception e)
                            {
                                Misc.Globals.ShowMsg($"An unexpected error occurred while trying to save the file.\nPlease try again.\n\nMessage details:\n{e.Message}\n\nTarget filepath:\n{targetPath}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                            // Open the file
                            try
                            {
                                System.Diagnostics.Process.Start(targetPath);
                            }
                            catch (Exception e)
                            {
                                Misc.Globals.ShowMsg($"An unexpected error occurred while trying to open the file.\nPlease try again.\n\nMessage details:\n{e.Message}\n\nTarget filepath:\n{targetPath}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                        }
                    }
                }
            }
        }

        // Check if a file is in use
        public static bool IsFileinUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
