// Decompiled with JetBrains decompiler
// Type: DistinguishedService.DSBattleBehavior
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.MountAndBlade;

namespace DistinguishedService
{
  internal class DSBattleBehavior : CampaignBehaviorBase
  {
    private AddNewGuy _ang_instance;

    public DSBattleBehavior(AddNewGuy ang) => this._ang_instance = ang;

    public override void RegisterEvents() => CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener((object) this, new Action<IMission>(this.FindBattle));

    public override void SyncData(IDataStore dataStore)
    {
    }

    public void FindBattle(IMission misson)
    {
      if (((Mission) misson).CombatType > Mission.MissionCombatType.Combat || !((NativeObject) Mission.Current.Scene != (NativeObject) null))
        return;
      Mission.Current.AddMissionBehaviour((MissionBehaviour) new DSBattleLogic(this._ang_instance));
    }
  }
}
