using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStore.Models.ViewModels;
namespace ApplicationStore.Models
{
    public class ApplicationPublish
    {
        //Properties======================================================================
        [Key]
        public int Id { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Version", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Version", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(10)]
        public string Version { get; set; }
        //________________________________________________________________________________
        [Display(Name = "PublishDate", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "PublishDate", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        //________________________________________________________________________________
        [Display(Name = "RegisterDate", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ChangeList", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "ChangeList", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public string ChangeList { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Price", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Price", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public double? Price { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Size", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Size", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public double Size { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Extension", ResourceType = typeof(StringDictionary.Model))]
        [MaxLength(10)]
        public string Extension { get; set; }
        //________________________________________________________________________________
        [Display(Name = "PlatformId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int PlatformId { get; set; }

        [ForeignKey("PlatformId")]
        public virtual Platform Platform { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int ApplicationId { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationStoreUserId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public string ApplicationStoreUserId { get; set; }
        [ForeignKey("ApplicationStoreUserId")]
        public virtual ApplicationStoreUser ApplicationStoreUser { get; set; }
        //================================================================================
    }
}
