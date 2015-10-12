using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public class Polygon
    {
        bool convex;
        bool ccw;
        Edge[] edges;
        Vector3[] satHelpers;
        Vector3 center;
        float height;
        Rectangle boundingBox;
        Vector3 boundingBoxPosition;

        public IEnumerable<Vector3> points { get { return edges.Select(e => e.Start); } }

        //Used so that inherited classes can potentially reorder input data
        protected bool WasNotCounterClockwise = false;

        public Polygon(BinaryReader reader)
        {
            int vertexCount = reader.ReadInt32();
            Vector3[] vertices = new Vector3[vertexCount];
            for (int x = 0; x < vertexCount; x++)
                vertices[x] = reader.ReadVector3();
            CalculateValues(vertices);
        }

        public void Store(BinaryWriter writer)
        {
            writer.Write(edges.Length);
            for (int x = 0; x < edges.Length; x++)
                writer.Write(edges[x].Start);
        }

        public Polygon(Vector3[] points, float height = 0)
        {
            this.height = height;
            CalculateValues(points);
        }

        public Polygon(Rectangle rect, float height = 0)
        {
            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(rect.X, 0, rect.Y);
            points[1] = new Vector3(rect.X, 0, rect.Bottom);
            points[2] = new Vector3(rect.Right, 0, rect.Bottom);
            points[3] = new Vector3(rect.Right, 0, rect.Y);
            CalculateValues(points);
        }

        private Polygon()
        {
            // COPY CONSTRUCTOR
        }

        public void Recreate(Vector3[] points, float height = 0)
        {
            this.height = height;
            CalculateValues(points);
        }

        private void CalculateValues(Vector3[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("Polygon must consist of at least 2 points");

            edges = new Edge[points.Length];
            center = Vector3.Zero;
            for (int x = 0; x < points.Length - 1; x++)
            {
                edges[x] = new Edge(points[x], points[x + 1]);
                center += points[x];
            }
            edges[edges.Length - 1] = new Edge(points[points.Length - 1], points[0]);

            center += points[points.Length - 1];
            center /= points.Length;

            convex = CheckIfConvex();
            ccw = CheckCounterClockwise();

            //TurnCounterClockwise();

            CalculateSATHelpers();

            CalculateBoundingBox();
        }

        public Edge GetEdge(Vector3 dir)
        {
            float maxVal = float.MinValue;
            Edge result = new Edge();
            for (int x = 0; x < edges.Length; x++)
            {
                float testVal = Vector3.Dot(dir, edges[x].Normal);
                if (testVal > maxVal)
                {
                    result = edges[x];
                    maxVal = testVal;
                }
            }
            return result;
        }

        public Vector3 ClosestVertex(Vector3 position)
        {

            float minVal = float.MaxValue;
            Vector3 result = Vector3.Zero;
            for (int x = 0; x < edges.Length; x++)
            {
                float testVal = (edges[x].Start - position).LengthSquared();
                if (testVal < minVal)
                {
                    result = edges[x].Start;
                    minVal = testVal;
                }
            }
            return result;
        }

        //Needed information to optimize collision detection
        private bool CheckIfConvex()
        {
            //Check last point first
            Vector3 dir = edges[0].End - edges[edges.Length - 1].Start;
            Vector3 normal = new Vector3(dir.Z,0, -dir.X);
            float proj = Vector3.Dot(normal, edges[edges.Length - 1].End - edges[edges.Length - 1].Start);
            if (proj < 0)
                return false;

            for (int x = 0; x < edges.Length - 1; x++)
            {
                //Remove connect two points leaving out the one in the middle
                //Check if left out point is closer to the center or further away
                //If closer the polygon is not convex
                //Calculate normal
                dir = edges[x + 1].End - edges[x].Start;
                normal = new Vector3(dir.Z, 0, -dir.X);
                //Dot(Normal, Left out point)
                proj = Vector3.Dot(normal, edges[x].End - edges[x].Start);
                if (proj < 0)
                    return false;
            }
            return true;
        }

        public void MoveTo(Vector3 position)
        {
            Move(position - center);
        }

        public void Move(Vector3 diff)
        {
            center += diff;
            for (int x = 0; x < edges.Length; x++)
            {
                edges[x].Start += diff;
                edges[x].End += diff;
            }
            boundingBoxPosition += diff;
            boundingBox.X = (int)boundingBoxPosition.X;
            boundingBox.Y = (int)boundingBoxPosition.Y;
        }

        private bool CheckCounterClockwise()
        {
            //Checking only one edge is sufficient since we are assuming convex polygons
            //and the sign is flipped for an inverted polygon
            return Vector3.Dot(edges[0].Normal, edges[0].Start) > Vector3.Dot(edges[0].Normal, center);
        }

        private void TurnCounterClockwise()
        {
            if (CheckCounterClockwise())
                return;
            //Invert the order of vertices
            for (int x = 0; x < edges.Length; x++)
            {
                Vector3 helper = edges[x].Start;
                edges[x].Start = edges[x].End;
                edges[x].End = helper;
                edges[x].Normal = -edges[x].Normal;
                edges[x].Dir = -edges[x].Dir;
            }
        }

        //Calculate how far the polygon extends along each normal
        //to speed up SAT calculation
        private void CalculateSATHelpers()
        {
            satHelpers = new Vector3[edges.Length];
            for (int x = 0; x < satHelpers.Length; x++)
            {
                float min = float.MaxValue;
                float max = float.MinValue;

                for (int y = 0; y < satHelpers.Length; y++)
                {
                    float proj = Vector3.Dot(edges[x].Normal, edges[y].Start - center);
                    if (proj < min)
                        min = proj;
                    if (proj > max)
                        max = proj;
                }

                satHelpers[x].X = min;
                satHelpers[x].Y = max;
            }
        }

        public bool Intersects(Polygon polygon)
        {
            //Check SAT, SAT will yield correct negatives for all polygons
            //But may yield false positives for concave polygons
            //So we check SAT first since we expect most things not to intersect
            //And if SAT says we collide and one polygon is not convex we test more intensively
            if (!InternalSATIntersect(polygon))
                return false;
            if (!polygon.InternalSATIntersect(this))
                return false;
            //For convex polygons SAT is sufficient
            if (IsConvex && polygon.IsConvex)
                return true;
            //TODO: For concave polygons we will need a more extensive check
            return true;
        }

        public bool Contains(Vector3 position)
        {
            for (int x = 0; x < edges.Length; x++)
            {
                Vector3 diff = position - edges[x].Start;
                float dot = Vector3.Dot(edges[x].Normal, diff);
                if ( dot < 0)
                    return false;
            }
            return true;
        }

        //Helper function to check if two ranges overlap
        private bool RangeOverlaps(Vector3 one, Vector3 two)
        {
            return (one.Y >= two.X && two.Y >= one.X);
        }

        private bool InternalSATIntersect(Polygon polygon)
        {
            //For every normal check if the projection of this polygon
            //intersects with the projection of the other one
            //If yes try next normal, if not we don't intersect
            Vector3 extent = Vector3.Zero;
            for (int x = 0; x < satHelpers.Length; x++)
            {
                extent.X = float.MaxValue;
                extent.Y = float.MinValue;

                for (int y = 0; y < polygon.edges.Length; y++)
                {
                    float proj = Vector3.Dot(edges[x].Normal, polygon.edges[y].Start - center);
                    if (proj < extent.X)
                        extent.X = proj;
                    if (proj > extent.Y)
                        extent.Y = proj;
                }
                if (!RangeOverlaps(satHelpers[x], extent))
                    return false;
            }
            return true;
        }

        public static void FindCollidingEdge(Polygon source, Polygon dest, Vector3 velocity, out Edge thatCollidesWith)
        {
            Edge edgeSource;
            float timeSource;

            Edge edgeDest;
            float timeDest;

            source.FindFirstCollidingEdge(dest, -velocity, out edgeSource, out timeSource);
            dest.FindFirstCollidingEdge(source, velocity, out edgeDest, out timeDest);

            thatCollidesWith = timeSource < timeDest ? edgeSource : edgeDest;
        }

        // 1. iterate through each Vertex on the source
        // 2. iterate through each of this polygons normals
        // 3. calculate time to that each vertex in source will collide against this's normals
        // 4. the vertex with the lowest time to collide against a normal is the one we collide with soonest
        // 5. store vertex -> T (inverse time to collide)
        // 6. find the vertex with the lowest T
        private void FindFirstCollidingEdge(Polygon source, Vector3 velocity, out Edge edgeCollidedWith, out float collideTime)
        {
            Edge collidesFirstEdge = new Edge();
            float collidesFirstTime = float.MaxValue;

            foreach (Vector3 vertex in source.points)
            {
                Edge closestEdge = new Edge();
                float closestEdgeTime = 0;
                float closestNotEdgeTime = float.MaxValue;
                bool found = false;

                foreach (Edge edge in this.edges)
                {
                    // time = -(nx*px)+(ny*py)/(vx*nx)+(vy*ny)
                    float top = Vector3.Dot(vertex - edge.Start, edge.Normal);
                    float bot = Vector3.Dot(velocity, edge.Normal);
                    float time = -((top) / bot);
                    bool facing = top >= 0;

                    // the normal with the highest T is the one we collide with soonest
                    if (facing && time > closestEdgeTime)
                    {
                        closestEdge = edge;
                        closestEdgeTime = time;
                        found = true;
                    }
                    else if (!facing && time > 0 && time < closestNotEdgeTime)
                    {
                        closestNotEdgeTime = time;
                    }
                }

                // the vertex with the lowest T is the vertex that will collide first
                if (found && closestEdgeTime < collidesFirstTime && closestEdgeTime < closestNotEdgeTime)
                {
                    collidesFirstEdge = closestEdge;
                    collidesFirstTime = closestEdgeTime;
                }
            }

            edgeCollidedWith = collidesFirstEdge;
            collideTime = collidesFirstTime;
        }

        public bool IsConvex
        {
            get { return convex; }
        }

        public Edge[] Edges
        {
            get { return edges; }
        }

        public Vector3 Center
        {
            get { return center; }
        }

        public float Height
        {
            get { return height; }
        }

        public bool IsCounterClockwise
        {
            get { return ccw; }
        }

        public Polygon Copy()
        {
            return new Polygon
            {
                convex = convex,
                edges = edges.ToArray(),
                satHelpers = satHelpers.ToArray(),
                center = center,
                height = height
            };
        }

        void CalculateBoundingBox()
        {
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (Vector3 v in points)
            {
                if (v.X < minX)
                    minX = v.X;
                if (v.Z < minY)
                    minY = v.Z;
                if (v.X > maxX)
                    maxX = v.X;
                if (v.Z > maxY)
                    maxY = v.Z;
            }
            boundingBoxPosition.X = minX;
            boundingBoxPosition.Y = minY;
            boundingBox.X = boundingBoxPosition.X;
            boundingBox.Y = boundingBoxPosition.Y;
            boundingBox.Width = (maxX - minX);
            boundingBox.Height = (maxY - minY);
        }

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

    }
}
