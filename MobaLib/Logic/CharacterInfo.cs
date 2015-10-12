using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobaLib
{
    public struct CharacterInfo
    {
        public float maxHealth;
        public float healthReg;

        public float resMax;
        public float resReg;

        public float armor;
        public float mres;

        public float attack;
        public float magic;
        public float attackSpeed;
        public float movespeed;
        public float range;

        public float resPen;
        public float armorPen;

        public float viewRadius;
        const string basePath = "Characters/";

        public float healthPerLevel;
        public float healthRegPerLevel;
        public float resPerLevel;
        public float resRegPerLevel;
        public float attackPerLevel;
        public float attackSpeedPerLevel;
        public float moveSpeedPerLevel;
        public float rangePerLevel;

        public CharacterInfo(bool isThisDumb)
        {
            maxHealth = 650;
            healthReg = 0.5f;

            resMax = 100;
            resReg = 1;

            armor = 30;
            mres = 30;

            attack = 50;
            magic = 0;
            attackSpeed = 0.6f;
            movespeed = 50;
            range = 40;

            resPen = 0;
            armorPen = 0;

            viewRadius = 60;

            healthPerLevel = 130;
            healthRegPerLevel = 0.1f;
            resPerLevel = 0;
            resRegPerLevel = 0;
            attackPerLevel = 10;
            attackSpeedPerLevel = 0.1f;
            moveSpeedPerLevel = 0;
            rangePerLevel = 0;
        }

        public void Store(string file)
        {
            StreamWriter writer = new StreamWriter(basePath + file);
            writer.WriteLine(maxHealth);
            writer.WriteLine(healthReg);
            writer.WriteLine(resMax);
            writer.WriteLine(resReg);
            writer.WriteLine(armor);
            writer.WriteLine(mres);
            writer.WriteLine(attack);
            writer.WriteLine(magic);
            writer.WriteLine(attackSpeed);
            writer.WriteLine(movespeed);
            writer.WriteLine(range);
            writer.WriteLine(resPen);
            writer.WriteLine(armorPen);
            writer.WriteLine(viewRadius);
            writer.WriteLine(healthPerLevel);
            writer.WriteLine(healthRegPerLevel);
            writer.WriteLine(resPerLevel);
            writer.WriteLine(resRegPerLevel);
            writer.WriteLine(attackPerLevel);
            writer.WriteLine(attackSpeedPerLevel);
            writer.WriteLine(moveSpeedPerLevel);
            writer.WriteLine(rangePerLevel);
            writer.Close();
        }

        public CharacterInfo(string file)
        {
            StreamReader reader = new StreamReader(basePath + file);
            maxHealth = float.Parse(reader.ReadLine());
            healthReg = float.Parse(reader.ReadLine());
            resMax = float.Parse(reader.ReadLine());
            resReg = float.Parse(reader.ReadLine());
            armor = float.Parse(reader.ReadLine());
            mres = float.Parse(reader.ReadLine());
            attack = float.Parse(reader.ReadLine());
            magic = float.Parse(reader.ReadLine());
            attackSpeed = float.Parse(reader.ReadLine());
            movespeed = float.Parse(reader.ReadLine());
            range = float.Parse(reader.ReadLine());
            resPen = float.Parse(reader.ReadLine());
            armorPen = float.Parse(reader.ReadLine());
            viewRadius = float.Parse(reader.ReadLine());
            healthPerLevel = float.Parse(reader.ReadLine());
            healthRegPerLevel = float.Parse(reader.ReadLine()); 
            resPerLevel = float.Parse(reader.ReadLine()); 
            resRegPerLevel = float.Parse(reader.ReadLine()); 
            attackPerLevel = float.Parse(reader.ReadLine()); 
            attackSpeedPerLevel = float.Parse(reader.ReadLine()); 
            moveSpeedPerLevel = float.Parse(reader.ReadLine()); 
            rangePerLevel = float.Parse(reader.ReadLine()); 
            reader.Close();
        }
    }
}
