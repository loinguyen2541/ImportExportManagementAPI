using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/17/2021 12:02:40 PM 
*/

namespace ImportExportManagementAPI.Models
{
    public enum AttributeKey
    {
        [EnumMember(Value = "StorageCapacity")]
        StorageCapacity,
        [EnumMember(Value = "AutoSchedule")]
        AutoSchedule,
        [EnumMember(Value = "SystemDate")]
        SystemDate
    }
}
