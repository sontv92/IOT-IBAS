using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models
{
    public class RenameFolder
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string newName { get; set; }
        public string newPath { get; set; }
        public int renamed { get; set; }
    }
}
