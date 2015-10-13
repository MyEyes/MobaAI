using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public static class PolygonHelper
    {
        public static Vector3 GetEdgeToCircumvent(this Polygon poly, Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float distance = direction.Length();
            direction /= distance;
            Vector3 clockwiseNormal = new Vector3(direction.Z, 0, -direction.X);
            Edge testEdge = new Edge();
            bool edgeFound=false;
            int testIndex = 0;
            //Find first hit edge
            for(int x=0; x<poly.Edges.Length; x++)
            {
                if(Vector3.Dot(clockwiseNormal, poly.Edges[x].Start-start)<0 && Vector3.Dot(clockwiseNormal, poly.Edges[x].End-start)>0)
                {
                    edgeFound = true;
                    testIndex = x;
                    testEdge = poly.Edges[x];
                }
            }
            if (!edgeFound)
                return end;

            float lengthRight = 0;
            float lengthLeft = 0;
            Vector3 currentPos = start;
            //Go around counterclockwise
            for(int x=0; x<poly.Edges.Length; x++)
            {
                int index = testIndex - x;
                if(index<0)
                    index+=poly.Edges.Length;
                Vector3 dir = end-currentPos;
                if (Vector3.Dot(poly.Edges[index].Normal, dir) < 0)
                {
                    lengthRight += (end - currentPos).Length();
                    break;
                }
                else
                {
                    lengthRight += (poly.Edges[index].Start - currentPos).Length();
                    currentPos = poly.Edges[index].Start;
                }
            }
            currentPos = start;
            //Go around clockwise
            for (int x = 0; x < poly.Edges.Length; x++)
            {
                int index = testIndex + x;
                if (index >= poly.Edges.Length)
                    index = 0;
                Vector3 dir = end - currentPos;
                if (Vector3.Dot(poly.Edges[index].Normal, dir) < 0)
                {
                    lengthLeft += (end - currentPos).Length();
                    break;
                }
                else
                {
                    lengthLeft += (poly.Edges[index].End - currentPos).Length();
                    currentPos = poly.Edges[index].End;
                }
            }

            return lengthLeft > lengthRight ? testEdge.Start : testEdge.End;
        }
    }
}
