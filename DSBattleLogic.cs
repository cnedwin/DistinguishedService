// Decompiled with JetBrains decompiler
// Type: DistinguishedService.DSBattleLogic
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DistinguishedService
{
  internal class DSBattleLogic : MissionLogic
  {
    private AddNewGuy _ang_instance;

    public DSBattleLogic(AddNewGuy ang)
    {
      this._ang_instance = ang;
      ang.nominations = new List<CharacterObject>();
      ang.killcounts = new List<int>();
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
    }

    private int totalKillCount()
    {
      int num = 0;
      foreach (Agent activeAgent in Mission.Current.PlayerTeam.ActiveAgents)
      {
        if (!activeAgent.IsHero)
          num += activeAgent.KillCount;
      }
      return num;
    }

    private List<float> kill_counts()
    {
      List<float> floatList = new List<float>();
      foreach (Agent activeAgent in Mission.Current.PlayerTeam.ActiveAgents)
      {
        if (!activeAgent.IsHero)
          floatList.Add((float) activeAgent.KillCount);
      }
      return floatList;
    }

    public static double Percentile(IEnumerable<float> seq, double percentile)
    {
      float[] array = seq.ToArray<float>();
      Array.Sort<float>(array);
      int index;
      double num = (double) (index = (int) (percentile * (double) (array.Length - 1))) - (double) index;
      return index + 1 < array.Length ? (double) array[index] * (1.0 - num) + (double) array[index + 1] * num : (double) array[index];
    }

    public override void ShowBattleResults()
    {
      if (TutorialPhase.Instance.TutorialQuestPhase != TutorialQuestPhase.Finalized || Mission.Current.Mode == MissionMode.Conversation || (Mission.Current.Mode == MissionMode.StartUp || Mission.Current.CombatType == Mission.MissionCombatType.ArenaCombat))
        return;
      int num1 = this.totalKillCount();
      if (num1 <= 0)
        return;
      float f = (float) DSBattleLogic.Percentile((IEnumerable<float>) this.kill_counts(), (double) this._ang_instance.outperform_percentile);
      foreach (Agent activeAgent in Mission.Current.PlayerTeam.ActiveAgents)
      {
        if (!activeAgent.IsHero)
        {
          CharacterObject characterObject = CharacterObject.Find(activeAgent.Character.StringId);
          if (MobileParty.MainParty.MemberRoster.Contains(characterObject))
          {
            int num2 = !characterObject.IsArcher ? (!characterObject.IsMounted ? this._ang_instance.inf_kill_threshold : this._ang_instance.cav_kill_threshold) : this._ang_instance.ran_kill_threshold;
            if (((double) this._ang_instance.outperform_percentile <= 0.0 ? 1 : (activeAgent.KillCount > MathF.Ceiling(f) ? 1 : 0)) != 0 && activeAgent.KillCount >= num2 && AddNewGuy.is_soldier_qualified(characterObject))
            {
              this._ang_instance.nominations.Add(characterObject);
              this._ang_instance.killcounts.Add(activeAgent.KillCount);
            }
          }
        }
      }
      this._ang_instance.begin_battle_size = num1;
      this._ang_instance.OnPCBattleEnded_results();
    }
  }
}
