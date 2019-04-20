using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models
{
    public class CommentLike
    {
        //Properties======================================================================
        [Key]
        public int Id { get; set; }
        //________________________________________________________________________________
        [Display(Name = "IsLike", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "IsLike", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public bool IsLike { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Count", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "Count", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public double Count { get; set; }
        //________________________________________________________________________________
        [Display(Name = "CommentId", ResourceType = typeof(StringDictionary.Model))]
        [Required()]
        public int CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }
        //================================================================================
    }
}
