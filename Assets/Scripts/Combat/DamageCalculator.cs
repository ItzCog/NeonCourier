using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
      public static int Calculate(List<Modifier> attackerState, List<Modifier> targetState, DamageInfo info)
      {
            int flat = 0, percent = 0, reduction = 0;
            foreach (var mod in attackerState)
            {
                  if (mod.type == Modifier.Type.Flat)
                  {
                        flat += mod.amount;
                  }
                  else
                  {
                        percent += mod.amount;
                  }
            }
            
            int damage = Mathf.RoundToInt((info.baseDamage + flat) * (1 + percent * 0.01f));
            return Mathf.Max(1, damage);
      }
}
