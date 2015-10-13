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

        public Champion(Map map, Team team, CharacterInfo info, Vector3 position):base(map, team, info, position)
        {

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

        public bool GoingBack
        {
            get { return goingBack; }
        }

        public void Respawn()
        {
            health = info.maxHealth;
            Move(GetTeam().BasePosition - GetPosition());
            dead = false;
        }

        public void TeleportToBase()
        {
            goingBack = false;
            health = info.maxHealth;
            Move(GetTeam().BasePosition - GetPosition());
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
