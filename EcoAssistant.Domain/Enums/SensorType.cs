using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAssistant.Domain.Enums
{
    public enum SensorType
    {
        Unknown = 0,
        Temperature = 1,
        Humidity = 2,
        Pressure = 4,
        Light = 5,
        Current = 6,
        Voltage = 7,
        Power = 8,
        Energy = 9,
    }
}
