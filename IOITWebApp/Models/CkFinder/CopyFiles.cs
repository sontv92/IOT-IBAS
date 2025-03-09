using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models
{
    public class CopyFiles
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public int copied { get; set; }
    }
}
