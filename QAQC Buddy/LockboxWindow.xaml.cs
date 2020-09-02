using QAQC_Buddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QAQC_Buddy
{
    /// <summary>
    /// Interaction logic for LockboxWindow.xaml
    /// </summary>
    public partial class LockboxWindow : Window
    {
        private List<Job> _jobs;
        private List<string> _items;

        public LockboxWindow(List<Job> jobs)
        {
            InitializeComponent();
            _jobs = jobs;
        }
    }
}
