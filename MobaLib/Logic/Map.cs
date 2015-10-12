using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public class Map
    {
        List<Character> characters;
        List<Attack> attacks;
        Bush[] bushes;
        Collision[] collisions;
        Structure[] structures;
        Lane[] lanes;
        float sizeX;
        float sizeY;

        List<ICollidable>[,] CollisionPartitioning;
        List<ITargetable>[,] TargetPartitioning;
        const float PartitioningDensity = 30;
        const string basePath = "Maps/";

        public Map(string file)
        {
            Stream stream = File.Open(basePath + file, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                sizeX = reader.ReadSingle();
                sizeY = reader.ReadSingle();
                int numBushes = reader.ReadInt32();
                bushes = new Bush[numBushes];
                for (int x = 0; x < numBushes; x++)
                    bushes[x] = new Bush(reader);

                int numCollisions = reader.ReadInt32();
                collisions = new Collision[numCollisions];
                for (int x = 0; x < numCollisions; x++)
                    collisions[x] = new Collision(reader);

                int numStructures = reader.ReadInt32();
                structures = new Structure[numStructures];
                for (int x = 0; x < numStructures; x++)
                    structures[x] = new Turret(this, reader);

                int numLanes = reader.ReadInt32();
                lanes = new Lane[numLanes];
                for (int x = 0; x < numLanes; x++)
                    lanes[x] = new Lane(reader);
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
            Initialize();
        }

        public Map(float sizeX, float sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public void SetBushes(Bush[] bushes)
        {
            this.bushes = bushes;
        }

        public void SetCollisions(Collision[] collisions)
        {
            this.collisions = collisions;
        }

        public void SetLanes(Lane[] lanes)
        {
            this.lanes = lanes;
        }

        public void SetStructures(Structure[] structures)
        {
            this.structures = structures;
        }

        public void SetTeams(Team[] teams)
        {
            for (int x = 0; x < structures.Length; x++)
            {
                structures[x].SetTeam(teams[structures[x].TeamID]);
                AddToPartitioning(structures[x]);
                AddCollider(structures[x]);
            }

            for (int x = 0; x < lanes.Length; x++)
                lanes[x].Team = teams[lanes[x].TeamID];
        }

        public void Initialize()
        {
            attacks = new List<Attack>();
            characters = new List<Character>();
            int partitioningSizeX = (int)(sizeX/PartitioningDensity)+1;
            int partitioningSizeY = (int)(sizeY/PartitioningDensity)+1;
            TargetPartitioning = new List<ITargetable>[partitioningSizeX, partitioningSizeY];
            CollisionPartitioning = new List<ICollidable>[partitioningSizeX, partitioningSizeY];
            for (int x = 0; x < partitioningSizeX; x++)
                for (int y = 0; y < partitioningSizeY; y++)
                {
                    TargetPartitioning[x, y] = new List<ITargetable>();
                    CollisionPartitioning[x, y] = new List<ICollidable>();
                }
            for (int x = 0; x < collisions.Length; x++)
                AddCollider(collisions[x]);
        }

        public void Update(float dt)
        {
            for (int x = 0; x < attacks.Count; x++)
                attacks[x].Update(dt);
            for (int x = 0; x < characters.Count; x++)
                characters[x].Update(dt);
            for (int x = 0; x < structures.Length; x++)
                if (!structures[x].IsDead())
                    structures[x].Update(dt);
        }

        public List<Attack> Attacks { get { return attacks; } }

        public void Add(Character character)
        {
            characters.Add(character);
            AddToPartitioning(character);
        }

        public void Remove(Character character)
        {
            characters.Remove(character);
            RemoveFromPartitioning(character);
        }

        public List<Character> Characters { get { return characters; } }

        public void RemoveFromPartitioning(ITargetable target)
        {
            Vector3 position = target.GetPosition();
            int x = (int)(position.X / PartitioningDensity);
            int y = (int)(position.Z / PartitioningDensity);
            TargetPartitioning[x, y].Remove(target);
        }

        public void AddToPartitioning(ITargetable target)
        {
            Vector3 position = target.GetPosition();
            int x = (int)(position.X / PartitioningDensity);
            int y = (int)(position.Z / PartitioningDensity);
            TargetPartitioning[x, y].Add(target);
        }

        public List<ITargetable> GetTargetsInRange(Vector3 center, float radius)
        {
            List<ITargetable> targets = new List<ITargetable>();
            int x = (int)(center.X / PartitioningDensity);
            int y = (int)(center.Z / PartitioningDensity);
            int dx = (int)(radius / PartitioningDensity) + 1;
            int dy = (int)(radius / PartitioningDensity) + 1;
            int minX = x - dx;
            if (minX < 0)
                minX = 0;
            int minY = y - dy;
            if (minY < 0)
                minY = 0;
            int maxX = x + dx;
            if (maxX >= TargetPartitioning.GetLength(0))
                maxX = TargetPartitioning.GetLength(0) - 1;
            int maxY = y + dy;
            if (maxY >= TargetPartitioning.GetLength(1))
                maxY = TargetPartitioning.GetLength(1) - 1;
            float radiusSqr = radius*radius;
            for(int xc=minX; xc<=maxX; xc++)
                for(int yc=minY; yc<=maxY; yc++)
                    for(int z=0; z<TargetPartitioning[xc,yc].Count; z++)
                    {
                        if ((TargetPartitioning[xc, yc][z].GetPosition() - center).LengthSquared() < radiusSqr)
                            targets.Add(TargetPartitioning[xc, yc][z]);
                    }
            return targets;
        }

        public void AddCollider(ICollidable collider)
        {
            int minX = (int)(collider.Bounds.BoundingBox.Left / PartitioningDensity);
            int maxX = (int)(collider.Bounds.BoundingBox.Right / PartitioningDensity)+1;
            int minY = (int)(collider.Bounds.BoundingBox.Top / PartitioningDensity);
            int maxY = (int)(collider.Bounds.BoundingBox.Bottom / PartitioningDensity) + 1;
            if (minX < 0)
                minX = 0;
            if (maxX >= CollisionPartitioning.GetLength(0))
                maxX = CollisionPartitioning.GetLength(0) - 1;
            if (minY < 0)
                minY = 0;
            if (maxY >= CollisionPartitioning.GetLength(1))
                maxY = CollisionPartitioning.GetLength(1) - 1;
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    CollisionPartitioning[x, y].Add(collider);
        }

        public void RemoveCollider(ICollidable collider)
        {
            int minX = (int)(collider.Bounds.BoundingBox.Left / PartitioningDensity);
            int maxX = (int)(collider.Bounds.BoundingBox.Right / PartitioningDensity) + 1;
            int minY = (int)(collider.Bounds.BoundingBox.Top / PartitioningDensity);
            int maxY = (int)(collider.Bounds.BoundingBox.Bottom / PartitioningDensity) + 1;
            if (minX < 0)
                minX = 0;
            if (maxX >= CollisionPartitioning.GetLength(0))
                maxX = CollisionPartitioning.GetLength(0) - 1;
            if (minY < 0)
                minY = 0;
            if (maxY >= CollisionPartitioning.GetLength(1))
                maxY = CollisionPartitioning.GetLength(1) - 1;
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    CollisionPartitioning[x, y].Remove(collider);
        }

        public bool InCollision(Vector3 position)
        {
            int x = (int)(position.X / PartitioningDensity);
            int y = (int)(position.Z / PartitioningDensity);
            if (x < 0)
                return true;
            if (y < 0)
                return true;
            if (x >= CollisionPartitioning.GetLength(0))
                return true;
            if (y >= CollisionPartitioning.GetLength(1))
                return true;
            for(int z=0; z<CollisionPartitioning[x,y].Count; z++)
                if(CollisionPartitioning[x,y][z].Bounds.Contains(position))
                    return true;
            return false;

        }

        public ICollidable GetCollider(Vector3 position)
        {
            int x = (int)(position.X / PartitioningDensity);
            int y = (int)(position.Z / PartitioningDensity);
            if (x < 0)
                return null;
            if (y < 0)
                return null;
            if (x >= CollisionPartitioning.GetLength(0))
                return null;
            if (y >= CollisionPartitioning.GetLength(1))
                return null;
            for (int z = 0; z < CollisionPartitioning[x, y].Count; z++)
                if (CollisionPartitioning[x, y][z].Bounds.Contains(position))
                    return CollisionPartitioning[x, y][z];
            return null;
        }

        public Bush[] Bushes { get { return bushes; } }
        public Lane[] Lanes { get { return lanes; } }
        public Collision[] Collisions { get { return collisions; } }
        public Structure[] Structures { get { return structures; } }

        public void Store(string file)
        {
            Stream stream = File.Open(basePath + file, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                writer.Write(sizeX);
                writer.Write(sizeY);
                writer.Write(bushes.Length);
                for (int x = 0; x < bushes.Length; x++)
                    bushes[x].Store(writer);

                writer.Write(collisions.Length);
                for (int x = 0; x < collisions.Length; x++)
                    collisions[x].Store(writer);

                writer.Write(structures.Length);
                for (int x = 0; x < structures.Length; x++)
                    structures[x].Store(writer);

                writer.Write(lanes.Length);
                for (int x = 0; x < lanes.Length; x++)
                    lanes[x].Store(writer);
            }
            finally
            {
                writer.Close();
                stream.Close();
            }
        }
    }
}
