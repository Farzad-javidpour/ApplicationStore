using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ApplicationStore.Models.ViewModels
{
    public enum MemberTypeEnum : byte
    {
        [EnumMember(Value = "Member")]
        Member = 1,

        [EnumMember(Value = "Admin")]
        Admin = 2,

        [EnumMember(Value = "Super Admin")]
        SuperAdmin = 3,
    }
}
