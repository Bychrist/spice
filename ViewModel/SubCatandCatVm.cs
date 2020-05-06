using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.ViewModel
{
    public class SubCatandCatVm
    {
        public IEnumerable<Category> Categories { get; set; }
        public List<string> SubCategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public string StatusMessage { get; set; }
    }
}
