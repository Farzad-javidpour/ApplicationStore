using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ApplicationStore.Models
{
    public class ApplicationPicture
    {
        //Properties======================================================================
        [Key]
        public int Id { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Title", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Title", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(50)]
        public string Title { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Size", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Size", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public double Size { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Data", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public byte[] Data { get; set; }
        //________________________________________________________________________________
        [Display(Name = "RegisterDate", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public DateTime RegisterDate { get; set; }
        //________________________________________________________________________________
        [Display(Name = "IsDefaultIcon", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public bool IsDefaultIcon { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationPublishId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int ApplicationPublishId { get; set; }
        [ForeignKey("ApplicationPublishId")]
        public virtual ApplicationPublish ApplicationPublish { get; set; }
        //================================================================================
    }
}
