using ApplicationStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models
{
    public class Comment
    {
        //Properties======================================================================
        [Key]
        public int Id { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Text", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Text", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(100)]
        //________________________________________________________________________________
        public string Text { get; set; }
        [Display(Name = "RegisterDate", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
        //________________________________________________________________________________
        [Display(Name = "CommentState", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public CommentStateEnum CommentState { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationPublishId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int ApplicationPublishId { get; set; }
        [ForeignKey("ApplicationPublishId")]
        public virtual ApplicationPublish ApplicationPublish { get; set; }
        //________________________________________________________________________________
        [Display(Name = "ApplicationStoreUserId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public string ApplicationStoreUserId { get; set; }
        [ForeignKey("ApplicationStoreUserId")]
        public virtual ApplicationStoreUser ApplicationStoreUser { get; set; }
        //================================================================================
    }
}
