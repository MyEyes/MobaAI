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
        List<Champion> champions;

        CharacterInfo MinionBonusPerMinute;
        CharacterInfo MinionBaseInfo;

        int minionsToSpawn = minionsPerWave;
        const int minionsPerWave = 6;
        float nextMinionSpawn = 20;
        const float minionSpawnCooldown = 0.4f;
        const float minionWaveSpawnCooldown = 30;

        float gameTime;

        public MobaGame(string mapFile)
        {
            map = new Map(mapFile);
            teams = new Team[]{
                new Team("Dick",0,255,255, new Vector3(10,0,990), map.Bushes.Length),
                new Team("Butt",255,0,0,new Vector3(990,0,10),map.Bushes.Length)
            };
            map.SetTeams(teams);
            MinionBaseInfo = new CharacterInfo("Minion.ci");
            MinionBonusPerMinute = new CharacterInfo("MinionBonus.ci");
        }

        public void Update(float dt)
        {
            gameTime += dt;
            if (teams[0].Alive && teams[1].Alive)
            {
                CheckMinionSpawn(dt);
                map.Update(dt);
            }
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
                /*
                if (random.NextDouble() < 0.05f)
                    map.Add(new Minion(map, map.Lanes[x], teams[map.Lanes[x].TeamID], new CharacterInfo("RapidFireMinion.ci"), map.Lanes[x].Waypoints[0]));
                else*/
                map.Add(new Minion(map, map.Lanes[x], teams[map.Lanes[x].TeamID], MinionBaseInfo+MinionBonusPerMinute*(gameTime/60.0f), map.Lanes[x].Waypoints[0]));
            }
        }

        public Team[] Teams { get { return teams; } }

        public Map Map { get { return map; } }
    }
}
