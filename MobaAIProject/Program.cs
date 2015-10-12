using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobaLib;

namespace MobaAIProject
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Character c1 = new Character(new Team(), new CharacterInfo("Minion.ci"), new Vector3(100,0,100));
            Character c2 = new Character(new Team(), new CharacterInfo("Champion.ci"), new Vector3(120, 0, 120));
            while(true)
            {
                c1.Update(1f);
                c2.Update(1f);
                c2.Attack(c1);
                Console.WriteLine("Character 2 health: {0}", c1.Health);
                Console.ReadLine();
            }
             */
            Map map = BuildDefaultMap();
            map.Store("Testmap.mm");
            Map map2 = new Map("Testmap.mm");
        }

        public static Map BuildDefaultMap()
        {
            Map map = new Map(1000, 1000);
            map.SetBushes(new Bush[] {
               new Bush(0, new Vector3[]{
                    new Vector3(43,0,2),
                    new Vector3(76,0,2),
                    new Vector3(78,0, 22),
                    new Vector3(43,0, 22)}),
               new Bush(1, new Vector3[]{
                    new Vector3(2,0,43),
                    new Vector3(22,0,43),
                    new Vector3(22,0, 78),
                    new Vector3(2,0, 78)}),
               new Bush(2, new Vector3[]{
                    new Vector3(494,0,440),
                    new Vector3(514,0,460),
                    new Vector3(460,0,510),
                    new Vector3(440,0,490)}),
               new Bush(3, new Vector3[]{
                    new Vector3(545,0,482),
                    new Vector3(565,0,502),
                    new Vector3(511,0, 564),
                    new Vector3(491,0, 543)}),
               new Bush(4, new Vector3[]{
                    new Vector3(921,0,980),
                    new Vector3(956,0,980),
                    new Vector3(956,0,1000),
                    new Vector3(921,0, 1000)}),
               new Bush(5, new Vector3[]{
                    new Vector3(983,0,913),
                    new Vector3(1000,0,913),
                    new Vector3(1000,0,945),
                    new Vector3(983,0, 945)}),
            });

            map.SetCollisions(new Collision[]{
               new Collision(new Vector3[]{
                    new Vector3(48,0,102),
                    new Vector3(460,0,516),
                    new Vector3(114,0,870),
                    new Vector3(48,0,823)}),
               new Collision(new Vector3[]{
                    new Vector3(489,0,550),
                    new Vector3(902,0,959),
                    new Vector3(190,0,960),
                    new Vector3(143,0,906)}),
               new Collision(new Vector3[]{
                    new Vector3(97,0,39),
                    new Vector3(812,0,39),
                    new Vector3(860,0,88),
                    new Vector3(517,0,453)}),
               new Collision(new Vector3[]{
                    new Vector3(884,0,131),
                    new Vector3(952,0,181),
                    new Vector3(952,0,882),
                    new Vector3(546,0,482)}),
                });

            map.SetStructures(new Structure[]
                {
                    new Structure(map, new Vector3(458,0,547), new Vector3[]{new Vector3(464,0,538), new Vector3(460,0,560), new Vector3(445,0,545)}, "Turret.ci", 0),
                    new Structure(map, new Vector3(542,0,453), new Vector3[]{new Vector3(533,0,463), new Vector3(540,0,440), new Vector3(555,0,455)}, "Turret.ci", 1),
                });

            map.SetLanes(new Lane[]
                {
                    new Lane(0, new Vector3[]{new Vector3(38,0,963), new Vector3(963,0,38)}),
                    new Lane(1, new Vector3[]{new Vector3(963,0,38),new Vector3(38,0,963)}),
                    new Lane(0, new Vector3[]{new Vector3(48,0,984), new Vector3(907,0,975), new Vector3(966, 0, 896), new Vector3(980,0,44)}),
                    new Lane(1, new Vector3[]{ new Vector3(980,0,44), new Vector3(966, 0, 896), new Vector3(907,0,975),new Vector3(48,0,984)}),
                    new Lane(0, new Vector3[]{new Vector3(20,0,953), new Vector3(33,0,103), new Vector3(91,0,27), new Vector3(951,0,20)}),
                    new Lane(1, new Vector3[]{new Vector3(951,0,20), new Vector3(91,0,27), new Vector3(33,0,103), new Vector3(20,0,953),})
                });
            return map;
        }
    }
}
