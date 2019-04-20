using ApplicationStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Models
{
    public class ApplicationStoreUser : IdentityUser
    {
        //Properties======================================================================
        override public string Id { get { return base.Id; } set { base.Id = value; } }
        //________________________________________________________________________________
        [Display(Name = "FirstName", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "FirstName", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(100)]
        public string FirstName { get; set; }
        //________________________________________________________________________________
        [Display(Name = "LastName", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "LastName", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        [MaxLength(100)]
        public string LastName { get; set; }
        //________________________________________________________________________________
        [Display(Name = "Email", ResourceType = typeof(StringDictionary.Model))]
        override public string Email { get { return base.Email; } set { base.Email = value; } }
        //________________________________________________________________________________
        [Display(Name = "PhoneNumber", ResourceType = typeof(StringDictionary.Model))]
        override public string PhoneNumber { get { return base.PhoneNumber; } set { base.PhoneNumber = value; } }
        //________________________________________________________________________________
        [NotMapped]
        [Display(Name = "MemberType", ResourceType = typeof(StringDictionary.Model))]
        [Required(ErrorMessageResourceName = "MemberType", ErrorMessageResourceType = typeof(StringDictionary.Model))]
        public MemberTypeEnum MemberType { get; set; }
        //================================================================================
    }
}
