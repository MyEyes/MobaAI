using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public enum StructureType
    {
        Turret,
        Nexus
    }

    public class Structure : ITargetable, ICollidable
    {
        protected Vector3 position;
        Polygon collisionPolygon;
        string characterFile;
        int teamID;
        Team team;
        protected CharacterInfo info;
        protected Map map;
        bool dead;
        float health;

        public Structure(Map map, BinaryReader reader)
        {
            position = reader.ReadVector3();
            collisionPolygon = new Polygon(reader);
            characterFile = reader.ReadString();
            teamID = reader.ReadInt32();
            info = new CharacterInfo(characterFile);
            health = info.maxHealth;
            this.map = map;
        }

        public Structure(Map map, Vector3 position, Vector3[] vertices, string characterfile, int teamID)
        {
            this.position = position;
            this.collisionPolygon = new Polygon(vertices);
            this.characterFile = characterfile;
            info = new CharacterInfo(characterfile);
            health = info.maxHealth;
            this.teamID = teamID;
            this.map = map;
        }

        public virtual void Update(float dt)
        {
        }

        public virtual float ExpValue { get { return 0; } }
        public virtual float GoldValue { get { return 0; } }

        public virtual void ReceiveGold(float gold) { }
        public virtual void ReceiveXP(float xp) { }
        public float GetHealth()
        {
            return health;
        }

        public CharacterInfo GetInfo()
        { return info; }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetTeam(Team team)
        {
            this.team = team;
        }

        public Team GetTeam()
        {
            return team;
        }

        public int TeamID { get { return teamID; } }

        public bool IsTargetable(Team team)
        {
            return !dead;
        }

        public void TakeMagDmg(float dmg, float pen)
        {

        }

        public float GetSize()
        {
            return (Bounds.BoundingBox.Width + Bounds.BoundingBox.Height) / 2;
        }

        public void TakePhysDmg(float dmg, float pen)
        {
            health -= dmg / (float)Math.Pow(2, (info.armor - pen) / 100);
            if (health < 0)
                Die();
        }

        public virtual void Die()
        {
            dead = true;
            map.RemoveFromPartitioning(this);
            map.RemoveCollider(this);
        }

        public bool IsDead()
        {
            return dead;
        }

        public Polygon Bounds
        {
            get { return collisionPolygon; }
        }

        public virtual void Store(BinaryWriter writer)
        {
            writer.Write(position);
            collisionPolygon.Store(writer);
            writer.Write(characterFile);
            writer.Write(teamID);
        }
    }
}
