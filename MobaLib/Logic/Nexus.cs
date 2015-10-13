using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MobaLib
{
    public class Nexus:Structure
    {
        public Nexus(Map map, Vector3 position, Vector3[] vertices, string characterfile, int teamID) : base(map, position, vertices, characterfile, teamID) { }
        public Nexus(Map map, BinaryReader reader) : base(map, reader) { }

        public override void Die()
        {
            GetTeam().NexusDied();
            base.Die();
        }

        public override void Store(System.IO.BinaryWriter writer)
        {
            writer.Write((int)StructureType.Nexus);
            base.Store(writer);
        }
    }
}
