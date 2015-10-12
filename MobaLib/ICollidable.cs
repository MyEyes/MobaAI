using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public interface ICollidable
    {
        Polygon Bounds { get; }
    }
}
