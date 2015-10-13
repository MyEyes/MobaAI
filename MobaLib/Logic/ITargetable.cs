using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public interface ITargetable
    {
        Vector3 GetPosition();
        bool IsTargetable(Team team);
        Team GetTeam();
        void TakePhysDmg(float dmg, float pen);
        void TakeMagDmg(float dmg, float pen);
        bool IsDead();
        float GetHealth();
        float ExpValue { get; }
        float GoldValue { get; }
        void ReceiveXP(float xp);
        void ReceiveGold(float gold);
        CharacterInfo GetInfo();
    }
}
