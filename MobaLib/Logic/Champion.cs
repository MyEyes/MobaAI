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
        Vector3 pasePosition;

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
            if (goingBack && backCountdown < 0)
            {
                goingBack = false;
                health = info.maxHealth;
                Move(GetTeam().BasePosition - GetPosition());
            }

            if (controller != null)
                controller.Update(dt);
            base.Update(dt);
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
