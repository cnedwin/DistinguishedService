// Decompiled with JetBrains decompiler
// Type: DistinguishedService.CompanionsDontSpawnPatch
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace DistinguishedService
{
  [HarmonyPatch]
  internal class CompanionsDontSpawnPatch
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
          MethodInfo method = type.GetMethod("CreateCompanion", BindingFlags.Instance | BindingFlags.NonPublic);
          if (method != (MethodInfo) null && !method.IsAbstract)
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

    [HarmonyPrefix]
    public static bool Prefix(CharacterObject companionTemplate) => !AddNewGuy.__instance.remove_tavern_companions;
  }
}
