using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaLib
{
    public struct Edge
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Normal;
        public Vector3 Dir;
        public float Length;

        public Edge(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
            Dir = End - Start;
            Length = Dir.Length();
            Dir /= Length;
            Normal.X = -Dir.Z;
            Normal.Y = 0;
            Normal.Z = Dir.X;
        }
    }
}
