using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public class Lane
    {
        Vector3[] waypoints;
        int teamID;

        public Lane(BinaryReader reader)
        {
            teamID = reader.ReadInt32();
            int numWayPoints = reader.ReadInt32();
            waypoints = new Vector3[numWayPoints];
            for (int x = 0; x < numWayPoints; x++)
                waypoints[x] = reader.ReadVector3();
        }

        public Lane(int teamID, Vector3[] waypoints)
        {
            this.teamID = teamID;
            this.waypoints = waypoints;
        }

        public Vector3[] Waypoints { get { return waypoints;} }
        public int TeamID { get { return teamID; } }

        public void Store(BinaryWriter writer)
        {
            writer.Write(teamID);
            writer.Write(waypoints.Length);
            for (int x = 0; x < waypoints.Length; x++)
                writer.Write(waypoints[x]);
        }
    }
}
