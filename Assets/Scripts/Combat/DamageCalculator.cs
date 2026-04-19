using System;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
      public static int Calculate(List<Modifier> attackerState, List<Modifier> targetState, DamageInfo info)
      {
            int flat = 0, percent = 0, reduction = 0;
            foreach (var mod in attackerState)
            {
                  switch (mod.type)
                  {
                        case Modifier.Type.Flat:
                              flat += mod.amount;
                              break;
                        case Modifier.Type.Percent:
                              percent += mod.amount;
                              break;
                  }
            }
            
            int damage = Mathf.RoundToInt((info.baseDamage + flat) * (1 + percent * 0.01f));
            return Mathf.Max(1, damage);
      }

      public static event Action<IDamageSource, IDamageable, int> OnDamageDealt;

      public static void DamageDealt(IDamageSource dealer, IDamageable receiver, int amount)
      {
            OnDamageDealt?.Invoke(dealer, receiver, amount);
      }
}
