using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ApplicationStore.Models.ViewModels
{
    public class ApplicationPublishViewModel
    {
        public ApplicationPublish ApplicationPublish { get; set; }
        public IEnumerable<Application> Applications { get; set; }
        public IEnumerable<Platform> Platforms { get; set; }
        public IEnumerable<Comment> Comments { get; set; }

        public string RegisterDateShamsi { get; set; }
        public string PublishDateShamsi { get; set; }
        public string PictureUrl { get; set; }

        [Display(Name = "لینک فایل")]
        public IFormFile AppLink { get; set; }
        
        [Display(Name = "تصویر")]
        public IFormFile AppPicture { get; set; }
    }
}
