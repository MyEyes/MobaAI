using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public class Bush
    {
        Polygon boundary;
        int id;

        public Bush(BinaryReader reader)
        {
            id = reader.ReadInt32();
            boundary = new Polygon(reader);
        }

        public Bush(int id, Vector3[] vertices)
        {
            this.id = id;
            this.boundary = new Polygon(vertices);
        }

        public Polygon Bounds { get { return boundary; } }
        public int ID { get { return id; } }

        public void Store(BinaryWriter writer)
        {
            writer.Write(id);
            boundary.Store(writer);
        }
    }
}
