﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaLib
{
    public class Character:ITargetable
    {
        Vector3 position;

        Team team;

        protected CharacterInfo info;
        
        protected float health = 650;

        float ressources = 100;

        float atkCooldown = 0;

        protected Bush currentBush;

        protected bool dead = false;
        protected Map map;

        public Character (Map map, Team team, CharacterInfo info, Vector3 position)
        {
            this.map = map;
            this.team = team;
            this.info = info;
            health = info.maxHealth;
            ressources = info.resMax;
            this.position = position;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Team GetTeam()
        {
            return team;
        }

        public bool IsTargetable(Team team)
        {
            return !dead && (currentBush == null || team.CanSeeInBush(currentBush.ID));
        }

        public float GetHealth()
        {
            return health;
        }

        public virtual void Attack(ITargetable target)
        {
            if (CanAttack(target))
            {
                atkCooldown = 1.0f / info.attackSpeed;
                map.Attacks.Add(new Attack(map, DamageType.Physical, info.attack, info.armorPen, position, this, target, 50));
                //target.TakePhysDmg(info.attack, info.armorPen);
            }
        }

        public virtual void TakePhysDmg(float dmg, float pen)
        {
            health -= dmg / (float)Math.Pow(2, (info.armor - pen) / 100);
            if (health < 0)
                Die();
        }

        public virtual float Size
        {
            get { return 10; }
        }

        public virtual void Move(Vector3 delta)
        {
            map.RemoveFromPartitioning(this);
            
            if (currentBush != null)
                team.LeaveBush(currentBush.ID);
            position += delta;
            
            if (map.InCollision(position))
                position -= delta;

            currentBush = map.GetBushAt(position);
            if (currentBush != null)
                team.EnterBush(currentBush.ID);

            map.AddToPartitioning(this);
        }

        public virtual void TakeMagDmg(float dmg, float pen)
        {
            health -= dmg / (float)Math.Pow(2, (info.mres - pen) / 100);
            if (health < 0)
                Die();
        }

        public float GetSize()
        {
            return Size;
        }

        public virtual void Update(float dt)
        {
            //Health
            health += info.healthReg * dt;
            if (health > GetInfo().maxHealth)
                health = GetInfo().maxHealth;

            //Res
            ressources += info.resReg * dt;
            if (ressources > info.resMax)
                ressources = info.resMax;

            //Cooldowns
            atkCooldown -= dt;
        }

        public bool InRange(ITargetable target)
        {
            return (this.position - target.GetPosition()).Length() < info.range + this.GetSize()/2 + target.GetSize()/2;
        }

        public bool CanAttack(ITargetable target)
        {
            return (AttackAvailable && target.IsTargetable(team) && InRange(target));
        }

        public bool AttackAvailable { get { return atkCooldown <= 0; } }

        public float Health { get { return health; } }

        public virtual float ExpValue { get { return 0; } }
        public virtual float GoldValue { get { return 0; } }

        public virtual void ReceiveGold(float gold) { }
        public virtual void ReceiveXP(float xp) { }

        public virtual void Die()
        {
            dead = true;
            if (currentBush != null)
                team.LeaveBush(currentBush.ID);
            map.Remove(this);
        }

        public bool IsDead()
        {
            return dead;
        }

        public virtual CharacterInfo GetInfo()
        { return info; }
    }
}
