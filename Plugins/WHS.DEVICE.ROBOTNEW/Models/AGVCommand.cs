using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.DEVICE.ROBOTNEW.Models
{
    public enum AGVCommand : int
    {
        left = 1,
        right,
        up,
        down,
        move,
        rotate,
        charge,
        change_floor
    }
}
