using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MobaLib
{
    class Turret:Structure
    {
        ITargetable currentEnemy;
        float atkCooldown;

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
                    map.Attacks.Add(new Attack(map, DamageType.Physical, info.attack, info.armorPen, position, currentEnemy, 60));
                }
                if (currentEnemy.IsDead())
                    currentEnemy = null;
            }
            atkCooldown -= dt;

            base.Update(dt);
        }

        public bool CanAttack(ITargetable target)
        {
            Vector3 diff = target.GetPosition() - GetPosition();
            return atkCooldown < 0 && diff.LengthSquared() < info.range*info.range;
        }
    }
}
