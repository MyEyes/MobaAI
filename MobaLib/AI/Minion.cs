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

        public Minion(Map map, Lane lane, Team team, CharacterInfo info, Vector3 position)
            : base(map, team, info, position)
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
                if (currentEnemy == null && targets[x].GetTeam() != GetTeam() && targets[x].IsTargetable(GetTeam()))
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
                else
                {
                    ITargetable test = GetClosestEnemy();
                    if (test != null)
                        currentEnemy = test;
                }
                if (currentEnemy.IsDead() || length > info.viewRadius)
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
                        evasionTarget = vertex + dir*Size;
                    }
                }

                Move(tdiff * info.movespeed * dt);
            }

            if (evade && tlength < 1)
                evade = false;

            base.Update(dt);
        }

        public ITargetable GetClosestEnemy()
        {
            List<ITargetable> targets = map.GetTargetsInRange(GetPosition(), info.viewRadius);

            float closest = float.MaxValue;
            ITargetable closestTarget = null;

            for (int x = 0; x < targets.Count; x++)
            {
                if (targets[x] == this)
                    continue;
                
                Vector3 diff = targets[x].GetPosition() - GetPosition();
                float length = diff.Length();

                if (targets[x].GetTeam() != GetTeam() && targets[x].IsTargetable(GetTeam()))
                {
                    if (length < closest)
                    {
                        closest = length;
                        closestTarget = targets[x];
                    }
                }
            }
            return closestTarget;
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
