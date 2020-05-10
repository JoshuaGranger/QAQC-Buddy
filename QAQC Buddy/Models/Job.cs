using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAQC_Buddy.Models
{
    public class Job
    {
        // Properties
        public string Name { get; set; }
        public List<string> DocumentsIncluded { get; set; }
    }
}
