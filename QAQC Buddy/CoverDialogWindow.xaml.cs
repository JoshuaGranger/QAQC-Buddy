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
    /// Interaction logic for CoverDialogWindow.xaml
    /// </summary>
    public partial class CoverDialogWindow : Window
    {
        private Misc.CoverSheet CoverSheet;

        public CoverDialogWindow(Misc.CoverSheet coverSheet)
        {
            InitializeComponent();
            CoverSheet = coverSheet;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Parse the textbox values into the object
            CoverSheet.Lockbox = lockbox.Text;
            CoverSheet.Date = date.Text;
            CoverSheet.WorkOrder = workOrder.Text;
            CoverSheet.EquipmentDesc = description.Text;

            // Close the dialog box
            this.Close();
        }
    }
}
