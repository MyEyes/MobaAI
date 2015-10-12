using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobaLib;

namespace ChampionAIs
{
    enum TestControllerState
    {
        Laning,
        Attacking,
        Retreating
    }

    public class TestController:AIController
    {
        Lane currentLane;
        int targetID = 1;
        bool evade;
        Vector3 evasionTarget;
        Vector3 targetPos;
        Vector3 nextTarget;
        ITargetable currentEnemy;
        TestControllerState state;

        public TestController(Map map, Champion champion)
            : base(map, champion)
        {
            state = TestControllerState.Laning;
        }

        public override void Update(float dt)
        {
            //Make sure we retreat when we're dying
            if (champion.GetHealth() < champion.GetInfo().maxHealth * 0.2f)
            {
                state = TestControllerState.Retreating;
                if (currentLane != null)
                {
                    targetID--;
                    if (targetID < 0) targetID = 0;
                    nextTarget = currentLane.Waypoints[targetID];
                    targetPos = nextTarget;
                }
            }

            switch(state)
            {
                case TestControllerState.Retreating: RetreatUpdate(dt); break;
                case TestControllerState.Laning: LaneUpdate(dt); break;
                case TestControllerState.Attacking: AttackUpdate(dt); break;
            }

            Vector3 tp;
            if (evade)
                tp = evasionTarget;
            else
                tp = targetPos;

            Vector3 tdiff = tp - champion.GetPosition();
            float tlength = tdiff.Length();
            tdiff /= tlength;
            if (tlength > 0)
            {
                Vector3 predictedPosition = champion.GetPosition() + tdiff * champion.GetInfo().movespeed * 0.5f;

                if (map.InCollision(predictedPosition))
                {
                    evade = true;
                    ICollidable collider = map.GetCollider(predictedPosition);
                    if (collider != null)
                    {
                        Vector3 vertex = collider.Bounds.ClosestVertex(predictedPosition);
                        Vector3 dir = vertex - collider.Bounds.Center;
                        dir /= dir.Length();
                        evasionTarget = vertex + dir;
                    }
                }

                champion.Move(tdiff * champion.GetInfo().movespeed * dt);
            }

            if (evade && tlength < 1)
                evade = false;
        }

        public void LaneUpdate(float dt)
        {
            if (currentLane == null)
            {
                Team team = champion.GetTeam();
                targetID = 0;
                int index = MobaGame.random.Next(map.Lanes.Length);
                if (map.Lanes[index].Team == team)
                    currentLane = map.Lanes[index];
                if (currentLane != null)
                    nextTarget = currentLane.Waypoints[0];
            }

            targetPos = nextTarget;
            Vector3 diff = nextTarget - champion.GetPosition();
            float length = diff.Length();
            diff /= length;
            if (length < 5)
            {
                targetID++;
                if (targetID >= currentLane.Waypoints.Length)
                    targetID = currentLane.Waypoints.Length - 1;
                nextTarget = currentLane.Waypoints[targetID];
            }

            TryFindTarget();
            if (currentEnemy != null)
                state = TestControllerState.Attacking;
        }

        public void AttackUpdate(float dt)
        {
            targetPos = currentEnemy.GetPosition();
            Vector3 diff = currentEnemy.GetPosition() - champion.GetPosition();
            float length = diff.Length();
            diff /= length;
            if (length < champion.GetInfo().range)
            {
                champion.Attack(currentEnemy);
                targetPos = champion.GetPosition();
            }
            if (currentEnemy.IsDead())
            {
                currentEnemy = null;
                state = TestControllerState.Laning;
            }
        }

        public void RetreatUpdate(float dt)
        {
            if(EnemyInRange(champion.GetInfo().viewRadius) && currentLane!=null)
            {
                targetPos = nextTarget;
                Vector3 diff = nextTarget - champion.GetPosition();
                float length = diff.Length();
                diff /= length;
                if (length < 5)
                {
                    targetID--;
                    if (targetID < 0)
                        targetID = 0;
                    nextTarget = currentLane.Waypoints[targetID];
                }
            }
            else
            {
                if (!champion.GoingBack)
                {
                    champion.GoBack();
                    nextTarget = champion.GetPosition();
                    targetPos = nextTarget;
                    currentLane = null;
                    targetID = 0;
                }
            }

            if (champion.GetHealth() > champion.GetInfo().maxHealth * 0.4f)
                state = TestControllerState.Laning;
        }

        public void TryFindTarget()
        {
            List<ITargetable> targets = map.GetTargetsInRange(champion.GetPosition(), champion.GetInfo().viewRadius);
            for (int x = 0; x < targets.Count; x++)
            {
                if (targets[x] == this)
                    continue;
                if (targets[x].GetTeam() != champion.GetTeam())
                {
                    currentEnemy = targets[x];
                    break;
                }
            }
        }

        public bool EnemyInRange(float range)
        {
            List<ITargetable> targets = map.GetTargetsInRange(champion.GetPosition(), range);

            for (int x = 0; x < targets.Count; x++)
            {
                if (targets[x] == this)
                    continue;
                if (targets[x].GetTeam() != champion.GetTeam())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
