using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QAQC_Buddy.Models;
using QAQC_Buddy.Misc;

namespace QAQC_Buddy.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Properties
        // Craft
        public ObservableCollection<Craft> Crafts { get; set; }
        private Craft selectedCraft;
        public Craft SelectedCraft
        {
            get { return selectedCraft; }
            set { selectedCraft = value; RaisePropertyChanged(nameof(SelectedCraft)); CraftChanged(); }
        }

        // Job
        public ObservableCollection<Job> Jobs { get; set; }
        public ObservableCollection<Job> FilteredJobs { get; set; }
        private Job selectedJob;
        public Job SelectedJob
        {
            get { return selectedJob; }
            set { selectedJob = value; RaisePropertyChanged(nameof(SelectedJob)); JobChanged(); }
        }
        private string jobFilterText;
        public string JobFilterText
        {
            get { return jobFilterText; }
            set { jobFilterText = value; RaisePropertyChanged(nameof(JobFilterText)); JobFilterChanged(); }
        }

        // Document
        public ObservableCollection<Document> Documents { get; set; }
        public ObservableCollection<Document> FilteredDocuments { get; set; }
        private Document _selectedDocument;
        public Document SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                RaisePropertyChanged(nameof(SelectedDocument));
                PreviewDocument.RaiseCanExecuteChanged();
            }
        }
        private string docFilterText;
        public string DocFilterText
        {
            get { return docFilterText; }
            set { docFilterText = value; RaisePropertyChanged(nameof(DocFilterText)); DocFilterChanged(); }
        }

        // ICommand
        public MyICommand ClearJobFilter { get; set; }
        public MyICommand ClearDocFilter { get; set; }
        public MyICommand PreviewDocument { get; set; }
        public MyICommand GenerateDocument { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            // ICommands
            ClearJobFilter = new MyICommand(OnClearJobFilter, CanClearJobFilter);
            ClearDocFilter = new MyICommand(OnClearDocFilter, CanClearDocFilter);
            PreviewDocument = new MyICommand(OnPreviewDocument, CanPreviewDocument);
            GenerateDocument = new MyICommand(OnGenerateDocument, CanGenerateDocument);
            
            // Initializations
            Crafts = new ObservableCollection<Craft>();
            Jobs = new ObservableCollection<Job>();
            FilteredJobs = new ObservableCollection<Job>();
            Documents = new ObservableCollection<Document>();
            FilteredDocuments = new ObservableCollection<Document>();
            JobFilterText = "";
            DocFilterText = "";

            // Check to make sure the user did not copy the executable to the desktop...
            // If they did, tell them and close the application
            Globals.CheckPaths();

            // Import Crafts, Jobs, and Documents
            ImportSetup();
        }
        #endregion

        #region Button Methods
        public void OnClearJobFilter()
        {
            JobFilterText = "";
        }
        public bool CanClearJobFilter()
        {
            return !string.IsNullOrEmpty(JobFilterText);
        }

        public void OnClearDocFilter()
        {
            DocFilterText = "";
        }
        public bool CanClearDocFilter()
        {
            return !string.IsNullOrEmpty(DocFilterText);
        }

        public void OnPreviewDocument()
        {
            PDFMerge.MergePDFs(new List<Document>() { SelectedDocument });
        }
        public bool CanPreviewDocument()
        {
            if (SelectedDocument != null)
                return System.IO.File.Exists(SelectedDocument.FullPath);

            return false;
        }

        public void OnGenerateDocument()
        {
            PDFMerge.MergePDFs(FilteredDocuments.Where(f => f.Selected == true));
        }
        public bool CanGenerateDocument()
        {
            return true;
        }
        #endregion

        #region Other Methods
        public void JobFilterChanged()
        {
            // Determine whether the button should be active
            ClearJobFilter.RaiseCanExecuteChanged();

            // Clear the collection
            FilteredJobs.Clear();

            // Add all jobs that match the filter
            if (SelectedCraft != null)
                if (SelectedCraft.Jobs.Any())
                    foreach (Job j in SelectedCraft.Jobs)
                        if (j.Name.ToLower().Contains(JobFilterText.ToLower()))
                            FilteredJobs.Add(j);

            // If a "Custom" entry exists, put it first
            if (FilteredJobs.Any(x => x.Name == "Custom"))
            {
                int index = FilteredJobs.IndexOf(FilteredJobs.First(x => x.Name == "Custom"));
                if (index != 0)
                    FilteredJobs.Move(index, 0);
            }
        }

        public void DocFilterChanged()
        {
            // Determine whether the button should be active
            ClearDocFilter.RaiseCanExecuteChanged();

            // Clear the collection
            FilteredDocuments.Clear();

            // Add all documents that match the filter
            foreach (Document d in Documents)
                if (d.ShortFileName.ToLower().Contains(DocFilterText.ToLower()))
                    FilteredDocuments.Add(d);
        }

        public void CraftChanged()
        {
            // Clear the document filter and show all documents
            DocFilterText = "";

            // Deselect job and document
            SelectedJob = null;
            SelectedDocument = null;

            // Don't clear the job filter, but reload everything as if the filter was changed
            JobFilterChanged();
        }

        public void JobChanged()
        {
            // Deselect document in listbox
            SelectedDocument = null;

            // Deselect all documents
            foreach (Document doc in Documents)
                doc.Selected = false;

            // Check boxes that are included
            if (SelectedJob != null)
            {
                foreach (string jdoc in SelectedJob.DocumentsIncluded)
                {
                    if (!Documents.Any(d => d.FullPath == jdoc))
                        Misc.Globals.ShowMsg($"This job's configuration includes a document that could not be found.\n\nJob:\n{SelectedJob.Name}\n\nDocument:\n{jdoc}",
                            "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        Documents.First(d => d.FullPath == jdoc).Selected = true;
                }
            }
        }

        public void ImportSetup()
        {
            // JSON Parsing
            string tFile = Globals.PathConfig + "gold.json";
            if (File.Exists(tFile))
            {
                string json = File.ReadAllText(tFile);
                var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Crafts = JsonConvert.DeserializeObject<ObservableCollection<Craft>>(json, settings);
            }
            else
                Misc.Globals.ShowMsg($"Critical configuration file not found!\n\nExpected file:\n{tFile}\n\nPlease correct the issue and try again.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Pull the Job list from each Craft into Jobs
            foreach (Craft c in Crafts)
            {
                // Alphabetize
                c.Jobs.Sort((x, y) => String.Compare(x.Name, y.Name));

                //Add to Jobs
                foreach (Job j in c.Jobs)
                    Jobs.Add(j);
            }

            // Import Documents
            foreach (Craft c in Crafts)
            {
                // return a List of Documents
                var tmpDocs = Document.Import(c);

                foreach (Document d in tmpDocs)
                    Documents.Add(d);
            }
        }
        #endregion

        #region PropertyChanged Management
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
