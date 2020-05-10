using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAQC_Buddy.Models
{
    // Constructor
    public class Craft
    {
        // Properties
        public string Name { get; set; }
        public string Folder { get; set; }
        public List<Job> Jobs { get; set; }
    }
}
