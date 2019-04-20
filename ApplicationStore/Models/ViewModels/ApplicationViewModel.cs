using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models.ViewModels
{
    public class ApplicationViewModel
    {
        public Application Application { get; set; }
        public IEnumerable<ApplicationCategory> ApplicationCategories { get; set; }

        public string RegisterDateShamsi { get; set; }
    }
}
