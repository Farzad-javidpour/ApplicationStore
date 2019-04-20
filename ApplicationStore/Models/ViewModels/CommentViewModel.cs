using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public ApplicationPublish ApplicationPublish { get; set; }
        public ApplicationStoreUser ApplicationStoreUser { get; set; }

        public string RegisterDateShamsi { get; set; }
    }
}
