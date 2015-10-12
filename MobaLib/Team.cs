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

        string name;

        public Team(string name, int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.name = name;
        }

        public int R { get { return r; } }
        public int G { get { return g; } }
        public int B { get { return b; } }
        public string Name { get { return name; } }
    }
}
