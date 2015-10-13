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

        public static CharacterInfo operator +(CharacterInfo cinfo1, CharacterInfo cinfo2)
        {
            CharacterInfo result = cinfo2;
            result.armor += cinfo1.armor;
            result.armorPen += cinfo1.armorPen;
            result.attack += cinfo1.attack;
            result.attackPerLevel += cinfo1.attackPerLevel;
            result.attackSpeed += cinfo1.attackSpeed;
            result.attackSpeedPerLevel += cinfo1.attackSpeedPerLevel;
            result.healthPerLevel += cinfo1.healthPerLevel;
            result.healthReg += cinfo1.healthReg;
            result.healthRegPerLevel += cinfo1.healthRegPerLevel;
            result.magic += cinfo1.magic;
            result.maxHealth += cinfo1.maxHealth;
            result.movespeed += cinfo1.movespeed;
            result.moveSpeedPerLevel += cinfo1.moveSpeedPerLevel;
            result.mres += cinfo1.mres;
            result.range += cinfo1.range;
            result.rangePerLevel += cinfo1.rangePerLevel;
            result.resMax += cinfo1.resMax;
            result.resPen += cinfo1.resPen;
            result.resPerLevel += cinfo1.resPerLevel;
            result.resReg += cinfo1.resReg;
            result.resRegPerLevel += cinfo1.resRegPerLevel;
            result.viewRadius += cinfo1.viewRadius;
            return result;
        }

        public static CharacterInfo operator *(CharacterInfo cinfo1, float val)
        {
            CharacterInfo result = cinfo1;
            result.armor *= val;
            result.armorPen *= val;
            result.attack *= val;
            result.attackPerLevel *= val;
            result.attackSpeed *= val;
            result.attackSpeedPerLevel *= val;
            result.healthPerLevel *= val;
            result.healthReg *= val;
            result.healthRegPerLevel *= val;
            result.magic *= val;
            result.maxHealth *= val;
            result.movespeed *= val;
            result.moveSpeedPerLevel *= val;
            result.mres *= val;
            result.range *= val;
            result.rangePerLevel *= val;
            result.resMax *= val;
            result.resPen *= val;
            result.resPerLevel *= val;
            result.resReg *= val;
            result.resRegPerLevel *= val;
            result.viewRadius *= val;
            return result;
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
