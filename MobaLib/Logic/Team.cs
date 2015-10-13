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
        bool alive = true;
        int[] MembersInBush;

        public Team(string name, int r, int g, int b, Vector3 basePos, int bushes)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.name = name;
            this.basePos = basePos;
            MembersInBush = new int[bushes];
            for (int x = 0; x < MembersInBush.Length; x++)
                MembersInBush[x] = 0;
        }

        public void EnterBush(int bush)
        {
            MembersInBush[bush]++;
        }

        public void LeaveBush(int bush)
        {
            MembersInBush[bush]--;
        }

        public bool CanSeeInBush(int bush)
        {
            return MembersInBush[bush] > 0;
        }

        public int R { get { return r; } }
        public int G { get { return g; } }
        public int B { get { return b; } }
        public string Name { get { return name; } }
        public Vector3 BasePosition { get { return basePos; } }
        public void NexusDied() { alive = false; }
        public bool Alive { get { return alive; } }
    }
}
