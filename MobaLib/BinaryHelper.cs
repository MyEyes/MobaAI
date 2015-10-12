using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public static class BinaryHelper
    {
        public static void Write(this BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            Vector3 vec;
            vec.X = reader.ReadSingle();
            vec.Y = reader.ReadSingle();
            vec.Z = reader.ReadSingle();
            return vec;
        }
    }
}
