using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaLib
{
    public struct Rectangle
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Bottom { get { return Y + Height; } }
        public float Top { get { return Y; } }
        public float Right { get { return X + Width; } }
        public float Left { get { return X; } }
    }
}
