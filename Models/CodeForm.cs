using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSIAppMaintenance.Models
{
    public class CodeForm
    {
        public int id { get; set; }
        public List<IFormFile> textGetFile { get; set; }
        public string txtNameReport { get; set; }
        public string txtNameTableAd { get; set; }
    }
}
