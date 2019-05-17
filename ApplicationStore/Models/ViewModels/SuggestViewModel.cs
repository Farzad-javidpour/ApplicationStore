using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models.ViewModels
{
    public class SuggestViewModel
    {
        public List<ApplicationPublishViewModel> SuggestByDownload { get; set; }
        public List<ApplicationPublishViewModel> SuggestBySize { get; set; }
    }
}
