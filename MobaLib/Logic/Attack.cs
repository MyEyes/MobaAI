using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public enum DamageType
    {
        Physical,
        Magical,
        True
    }

    public class Attack
    {
        Map map;
        DamageType type;
        Vector3 position;
        ITargetable attacker;
        ITargetable target;
        Vector3 targetPosition;
        float damage;
        float pen;
        float Speed;

        public Attack(Map map, DamageType type, float damage, float pen, Vector3 position, ITargetable attacker, ITargetable target, float speed)
        {
            this.attacker = attacker;
            this.map = map;
            this.type = type;
            this.position = position;
            this.target = target;
            this.damage = damage;
            this.pen = pen;
            this.Speed = speed;
        }

        public void Update(float dt)
        {
            if (target != null)
                targetPosition = target.GetPosition();
            Vector3 diff = targetPosition-position;
            float length = diff.Length();
            diff /= length;
            position += diff * dt * Speed;

            if (target != null && target.IsDead())
            {
                map.Attacks.Remove(this);
                return;
            }

            if(target!=null && length<1)
            {
                map.Attacks.Remove(this);
                switch (type)
                {
                    case DamageType.Physical: target.TakePhysDmg(damage,pen);break;
                    case DamageType.Magical: target.TakeMagDmg(damage,pen);break;
                }
                if(target.IsDead())
                {
                    if (attacker != null)
                    {
                        attacker.ReceiveGold(target.GoldValue);
                        attacker.ReceiveXP(target.ExpValue);
                    }
                }
            }
        }

        public Vector3 GetPosition()
        {
            return position;
        }


    }
}
