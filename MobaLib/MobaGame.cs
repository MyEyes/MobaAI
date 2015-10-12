using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public class MobaGame
    {
        public static Random random = new Random();
        Map map;
        Team[] teams;

        int minionsToSpawn = minionsPerWave;
        const int minionsPerWave = 6;
        float nextMinionSpawn = 20;
        const float minionSpawnCooldown = 0.4f;
        const float minionWaveSpawnCooldown = 30;

        public MobaGame(string mapFile)
        {
            map = new Map(mapFile);
            teams = new Team[]{
                new Team("Dick",0,255,255),
                new Team("Butt",255,0,0)
            };
            map.SetTeams(teams);
        }

        public void Update(float dt)
        {
            CheckMinionSpawn(dt);
            map.Update(dt);
        }

        public void CheckMinionSpawn(float dt)
        {
            nextMinionSpawn -= dt;
            if(nextMinionSpawn<0)
            {
                SpawnMinions();
            }
        }

        public void SpawnMinions()
        {
            if (minionsToSpawn == 0)
            {
                minionsToSpawn = minionsPerWave;
                nextMinionSpawn = minionWaveSpawnCooldown;
                return;
            }
            else
            {
                minionsToSpawn--;
                nextMinionSpawn = minionSpawnCooldown;
            }
            for(int x=0; x<map.Lanes.Length; x++)
            {
                if (random.NextDouble() < 0.05f)
                    map.Add(new Minion(map, map.Lanes[x], teams[map.Lanes[x].TeamID], new CharacterInfo("RapidFireMinion.ci"), map.Lanes[x].Waypoints[0]));
                else
                    map.Add(new Minion(map, map.Lanes[x], teams[map.Lanes[x].TeamID], new CharacterInfo("Minion.ci"), map.Lanes[x].Waypoints[0]));
            }
        }

        public Team[] Teams { get { return teams; } }

        public Map Map { get { return map; } }
    }
}
