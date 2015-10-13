using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public class Champion : Character
    {
        AIController controller;
        bool goingBack = false;
        float backCountdown = backBaseCountdown;
        const float backBaseCountdown = 5;
        float respawnCooldown;
        CharacterInfo totalInfo;

        int level = 1;
        float expToNextLevel = 100;
        float exp = 0;

        public Champion(Map map, Team team, CharacterInfo info, Vector3 position):base(map, team, info, position)
        {
            totalInfo = base.GetInfo();
        }

        public void SetController(AIController controller)
        {
            this.controller = controller;
        }

        public override void Update(float dt)
        {
            backCountdown -= dt;
            respawnCooldown -= dt;

            if (controller != null)
                controller.Update(dt);

            if (goingBack && backCountdown < 0)
            {
                TeleportToBase();
            }

            if (IsDead() && respawnCooldown < 0)
                Respawn();
            base.Update(dt);
        }

        public override void Move(Vector3 delta)
        {
            goingBack = false;
            base.Move(delta);
        }

        public override void Die()
        {
            respawnCooldown = 30;
            dead = true;
        }

        public override void TakePhysDmg(float dmg, float pen)
        {
            goingBack = false;
            base.TakePhysDmg(dmg, pen);
        }

        public void GoBack()
        {
            goingBack = true;
            backCountdown = backBaseCountdown;
        }

        public override CharacterInfo GetInfo()
        {
            return totalInfo;
        }

        public bool GoingBack
        {
            get { return goingBack; }
        }

        public void Respawn()
        {
            health = totalInfo.maxHealth;
            Move(GetTeam().BasePosition - GetPosition());
            dead = false;
        }

        public void TeleportToBase()
        {
            goingBack = false;
            health = totalInfo.maxHealth;
            Move(GetTeam().BasePosition - GetPosition());
        }

        public void LevelUp()
        {
            level++;
            exp -= expToNextLevel;
            expToNextLevel = 100 * (float)Math.Pow(level, 1.5f);
            
            totalInfo.maxHealth += totalInfo.healthPerLevel;
            health += totalInfo.healthPerLevel;
            totalInfo.healthReg += totalInfo.healthRegPerLevel;
            totalInfo.movespeed += totalInfo.moveSpeedPerLevel;
            totalInfo.range += totalInfo.rangePerLevel;
            totalInfo.resMax += totalInfo.resPerLevel;
            totalInfo.resReg += totalInfo.resRegPerLevel;
            totalInfo.attack += totalInfo.attackPerLevel;
            totalInfo.attackSpeed += totalInfo.attackSpeedPerLevel;

            Console.WriteLine("Champion for {0} leveled up to {1}!", GetTeam().Name, level);
        }

        public override void ReceiveXP(float xp)
        {
            exp += xp;
            if (exp > expToNextLevel)
                LevelUp();
        }

        public override float GoldValue
        {
            get
            {
                return 300;
            }
        }

        public override float ExpValue
        {
            get
            {
                return 100;
            }
        }

        public override float Size
        {
            get
            {
                return 8;
            }
        }
    }
}
