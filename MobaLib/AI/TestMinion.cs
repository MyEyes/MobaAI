using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public class TestMinion:Character
    {
        Lane lane;
        int targetID = 1;
        bool evade;
        Vector3 evasionTarget;
        Vector3 targetPos;
        Vector3 nextTarget;
        ITargetable currentEnemy;

        public TestMinion(Map map, Lane lane, Team team, CharacterInfo info, Vector3 position)
            : base(map, team, info, position)
        {
            this.lane = lane;
            nextTarget = lane.Waypoints[1];
        }

        public override void Update(float dt)
        {
            Vector3 tp;
            if (!evade)
                tp = targetPos;
            else
                tp = evasionTarget;

            Vector3 tdiff = tp - GetPosition();
            float tlength = tdiff.Length();
            tdiff /= tlength;
            if (tlength > 0)
            {
                Vector3 predictedPosition = GetPosition() + tdiff * info.movespeed * 0.1f;

                if (map.InCollision(predictedPosition))
                {
                    evade = true;
                    ICollidable collider = map.GetCollider(predictedPosition);
                    if (collider != null)
                    {
                        Vector3 vertex = collider.Bounds.GetEdgeToCircumvent(GetPosition(), tp);
                        Vector3 dir = vertex-collider.Bounds.Center;
                        dir/=dir.Length();
                        evasionTarget = vertex + dir*Size;
                    }
                }

                Move(tdiff * info.movespeed * dt);
            }

            if (evade && tlength < 1)
                evade = false;

            base.Update(dt);
        }

        public void SetTarget(Vector3 position)
        {
            targetPos = position;
            evade = false;
        }

        public override void TakeMagDmg(float dmg, float pen)
        {
            
        }

        public override void TakePhysDmg(float dmg, float pen)
        {
            
        }


        public override float ExpValue
        {
            get
            {
                return 50;
            }
        }

        public override float GoldValue
        {
            get
            {
                return 25;
            }
        }

        public override float Size
        {
            get
            {
                return 4;
            }
        }
    }
}
