using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    class Minion:Character
    {
        Lane lane;
        int targetID = 1;
        bool evade;
        Vector3 evasionTarget;
        Vector3 targetPos;
        Vector3 nextTarget;
        ITargetable currentEnemy;

        public Minion(Map map, Lane lane, Team team, CharacterInfo info, Vector3 position):base(map,team,info,position)
        {
            this.lane = lane;
            nextTarget = lane.Waypoints[1];
        }

        public override void Update(float dt)
        {
            List<ITargetable> targets = map.GetTargetsInRange(GetPosition(), info.viewRadius);

            for (int x = 0; x < targets.Count; x++)
            {
                if (targets[x] == this)
                    continue;
                if (currentEnemy == null && targets[x].GetTeam() != GetTeam())
                {
                    currentEnemy = targets[x];
                }
                Vector3 diff = targets[x].GetPosition() - GetPosition();
                float length = diff.Length();
                diff /= length;
                if (length < 10 && length != 0)
                    Move(-diff * dt);
            }

            if (currentEnemy == null)
            {
                targetPos = nextTarget;
                Vector3 diff = nextTarget - GetPosition();
                float length = diff.Length();
                diff /= length;
                if (length < 5)
                {
                    targetID++;
                    if (targetID >= lane.Waypoints.Length)
                        targetID = lane.Waypoints.Length - 1;
                    nextTarget = lane.Waypoints[targetID];
                }
            }
            else
            {
                targetPos = currentEnemy.GetPosition();
                Vector3 diff = currentEnemy.GetPosition() - GetPosition();
                float length = diff.Length();
                diff /= length;
                if (length < info.range)
                {
                    Attack(currentEnemy);
                    targetPos = GetPosition();
                }
                if (currentEnemy.IsDead())
                    currentEnemy = null;
            }

            Vector3 tp;
            if (evade)
                tp = evasionTarget;
            else
                tp = targetPos;

            Vector3 tdiff = tp - GetPosition();
            float tlength = tdiff.Length();
            tdiff /= tlength;
            if (tlength > 0)
            {
                Vector3 predictedPosition = GetPosition() + tdiff * info.movespeed * 0.5f;

                if (map.InCollision(predictedPosition))
                {
                    evade = true;
                    ICollidable collider = map.GetCollider(predictedPosition);
                    if (collider != null)
                    {
                        Vector3 vertex = collider.Bounds.ClosestVertex(predictedPosition);
                        Vector3 dir = vertex-collider.Bounds.Center;
                        dir/=dir.Length();
                        evasionTarget = vertex + dir;
                    }
                }

                Move(tdiff * info.movespeed * dt);
            }

            if (evade && tlength < 1)
                evade = false;

            base.Update(dt);
        }
    }
}
