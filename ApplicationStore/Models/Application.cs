using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ApplicationStore.Models.ViewModels;
namespace ApplicationStore.Models
{
    public class Application
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
        [Display(Name = "Code", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Code", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(50)]
        public string Code { get; set; }
        //________________________________________________________________________________
        [Display(Name = "RegisterDate", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Description", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Description", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public string Description { get; set; }
        //________________________________________________________________________________
        [Display(Name = "State", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public RowStateEnum State { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationCategoryId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int ApplicationCategoryId { get; set; }
        [ForeignKey("ApplicationCategoryId")]
        public virtual ApplicationCategory ApplicationCategory { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationStoreUserId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public string ApplicationStoreUserId { get; set; }
        [ForeignKey("ApplicationStoreUserId")]
        public virtual ApplicationStoreUser ApplicationStoreUser { get; set; }
        //================================================================================
    }
}
