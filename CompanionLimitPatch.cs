// Decompiled with JetBrains decompiler
// Type: DistinguishedService.CompanionLimitPatch
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DistinguishedService
{
  [HarmonyPatch]
  internal class CompanionLimitPatch
  {
    private static IEnumerable<MethodBase> TargetMethods(Harmony h)
    {
      List<MethodInfo> methodInfoList = new List<MethodInfo>();
      Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
      for (int index1 = 0; index1 < assemblyArray.Length; ++index1)
      {
        Assembly assem = assemblyArray[index1];
        Type[] typeArray = assem.GetTypes();
        for (int index2 = 0; index2 < typeArray.Length; ++index2)
        {
          Type type = typeArray[index2];
          MethodInfo method = type.GetMethod("GetCompanionLimitFromTier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          if (method != (MethodInfo) null && !method.IsAbstract && !(method.ReturnType == typeof (void)))
          {
            FileLog.Log("found " + method.Name + " in " + type.Name + ", in " + assem.GetName()?.ToString());
            yield return (MethodBase) method;
          }
        }
        typeArray = (Type[]) null;
        assem = (Assembly) null;
      }
      assemblyArray = (Assembly[]) null;
    }

    [HarmonyPostfix]
    public static void Postfix(ref int __result, int clanTier) => __result += AddNewGuy.__instance.bonus_companion_slots_base + AddNewGuy.__instance.bonus_companion_slots_per_clan_tier * clanTier;
  }
}
