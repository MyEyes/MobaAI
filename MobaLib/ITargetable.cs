using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobaLib
{
    public interface ITargetable
    {
        Vector3 GetPosition();
        bool IsTargetable();
        Team GetTeam();
        void TakePhysDmg(float dmg, float pen);
        void TakeMagDmg(float dmg, float pen);
        bool IsDead();
    }
}
