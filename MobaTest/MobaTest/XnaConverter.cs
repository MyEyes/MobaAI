using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaTest
{
    public static class XnaConverter
    {
        public static Microsoft.Xna.Framework.Vector3 ToXNAVector(this MobaLib.Vector3 vec)
        {
            return new Microsoft.Xna.Framework.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}
