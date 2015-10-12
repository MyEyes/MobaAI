using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public class Collision : ICollidable
    {
        Polygon collisionPolygon;

        public Collision(BinaryReader reader)
        {
            collisionPolygon = new Polygon(reader);
        }

        public Collision(Vector3[] vertices)
        {
            collisionPolygon = new Polygon(vertices);
        }

        public Polygon Bounds { get { return collisionPolygon; } }

        public void Store(BinaryWriter writer)
        {
            collisionPolygon.Store(writer);
        }
    }
}
