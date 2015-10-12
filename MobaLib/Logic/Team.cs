using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaLib
{
    public class Team
    {
        //Color
        int r;
        int g;
        int b;
        Vector3 basePos;
        string name;

        public Team(string name, int r, int g, int b, Vector3 basePos)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.name = name;
            this.basePos = basePos;
        }

        public int R { get { return r; } }
        public int G { get { return g; } }
        public int B { get { return b; } }
        public string Name { get { return name; } }
        public Vector3 BasePosition { get { return basePos; } }
    }
}
