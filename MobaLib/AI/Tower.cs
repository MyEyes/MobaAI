using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MobaLib
{
    public class Turret:Structure
    {
        ITargetable currentEnemy;
        float atkCooldown;

        public Turret(Map map, Vector3 position, Vector3[] vertices, string characterfile, int teamID) : base(map, position, vertices, characterfile, teamID) { }
        public Turret(Map map, BinaryReader reader) : base(map, reader) { }

        public override void Update(float dt)
        {
            if (currentEnemy == null)
            {
                List<ITargetable> targets = map.GetTargetsInRange(position, info.range);
                for (int x = 0; x < targets.Count; x++)
                {
                    if (targets[x] == this)
                        continue;
                    if (targets[x].GetTeam() != GetTeam())
                    {
                        currentEnemy = targets[x];
                    }
                }
            }
            else
            {
                if (CanAttack(currentEnemy))
                {
                    atkCooldown = 1.0f / info.attackSpeed;
                    map.Attacks.Add(new Attack(map, DamageType.Physical, info.attack, info.armorPen, position,this, currentEnemy, 60));
                }
                if (currentEnemy.IsDead() || (currentEnemy.GetPosition()-GetPosition()).LengthSquared() < info.range*info.range)
                    currentEnemy = null;
            }
            atkCooldown -= dt;

            base.Update(dt);
        }

        public override void Store(System.IO.BinaryWriter writer)
        {
            writer.Write((int)StructureType.Turret);
            base.Store(writer);
        }

        public bool CanAttack(ITargetable target)
        {
            Vector3 diff = target.GetPosition() - GetPosition();
            return atkCooldown < 0 && diff.LengthSquared() < info.range*info.range;
        }

        public override float GoldValue
        {
            get
            {
                return 150;
            }
        }
    }
}
