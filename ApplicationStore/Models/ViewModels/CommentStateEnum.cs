using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ApplicationStore.Models.ViewModels
{
    public enum CommentStateEnum : byte
    {
        [EnumMember(Value = "Hidden")]
        Hidden = 1,

        [EnumMember(Value = "Confirmed")]
        Confirmed = 2,

        [EnumMember(Value = "Deleted")]
        Deleted = 3,
    }
}
