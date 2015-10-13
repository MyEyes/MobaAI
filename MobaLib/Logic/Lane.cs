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

        float totalLength;

        public Team Team;

        public Lane(BinaryReader reader)
        {
            teamID = reader.ReadInt32();
            int numWayPoints = reader.ReadInt32();
            waypoints = new Vector3[numWayPoints];
            for (int x = 0; x < numWayPoints; x++)
                waypoints[x] = reader.ReadVector3();
            totalLength = 0;
            for (int x = 0; x < waypoints.Length - 1; x++)
                totalLength += (waypoints[x + 1] - waypoints[x]).Length();
        }

        public Lane(int teamID, Vector3[] waypoints)
        {
            this.teamID = teamID;
            this.waypoints = waypoints;
            totalLength=0;
            for (int x = 0; x < waypoints.Length - 1; x++)
                totalLength += (waypoints[x + 1] - waypoints[x]).Length();
        }

        public Vector3[] Waypoints { get { return waypoints;} }
        public int TeamID { get { return teamID; } }

        public Vector3 GetClosest(Vector3 point)
        {
            Vector3 result = waypoints[0];
            float currentDistance = (waypoints[0] - point).Length();

            for (int x = 0; x < waypoints.Length-1; x++)
            {
                Vector3 start = waypoints[x];
                Vector3 dir = waypoints[x + 1] - start;
                float length = dir.Length();
                dir /= length;

                Vector3 diff = point - start;
                float lengthOnEdge = Vector3.Dot(diff, dir);
                float distance;
                if (lengthOnEdge < 0)
                    lengthOnEdge = 0;
                //distance = (diff).Length();
                else if (lengthOnEdge > length)
                    //distance = (waypoints[x + 1] - start).Length();
                    lengthOnEdge = length;
                    /*
                else
                    distance = Math.Abs(Vector3.Dot(new Vector3(-dir.Z, 0, dir.X), diff));*/
                distance = (start + dir * lengthOnEdge - point).Length();

                if(distance<currentDistance)
                {
                    currentDistance = distance;
                    result = start + dir * lengthOnEdge;
                }
            }
            return result;
        }

        public Vector3 GetPointOnLane(float distance)
        {
            for (int x = 0; x < waypoints.Length - 1; x++)
            {
                Vector3 start = waypoints[x];
                Vector3 dir = waypoints[x + 1] - start;
                float length = dir.Length();
                dir /= length;
                if (distance * totalLength < length)
                    return start + dir * distance * totalLength;
                else
                    distance -= length / totalLength;
            }
            return new Vector3(500, 0, 500);
        }



        public void Store(BinaryWriter writer)
        {
            writer.Write(teamID);
            writer.Write(waypoints.Length);
            for (int x = 0; x < waypoints.Length; x++)
                writer.Write(waypoints[x]);
        }
    }
}
