using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAssistant.Domain.Enums
{
    public enum DeviceStatus
    {
            Unknown = 0,
            Maintenance = -1,
            Active = 100,
            Inactive = 101,
            NotFound = 404
    }
}
