// Decompiled with JetBrains decompiler
// Type: DistinguishedService.SubModule
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace DistinguishedService
{
  public class SubModule : MBSubModuleBase
  {
    public static SubModule instance;

    private Settings CurrentSettings { get; set; }

    protected override void OnSubModuleLoad()
    {
      SubModule.instance = this;
      Harmony.DEBUG = false;
      Harmony harmony = new Harmony("mod.bannerlord.distinguishedservice");
      try
      {
        harmony.PatchAll();
      }
      catch (Exception ex)
      {
        FileLog.Log("Error patching:\n" + ex.Message + " \n\n" + ex.InnerException?.Message);
      }
    }

    public override void OnCampaignStart(Game game, object starterObject)
    {
      try
      {
        this.DeserializeObject(Path.Combine(BasePath.Name, "Modules", "DistinguishedService", "Settings.xml"));
      }
      catch (Exception ex)
      {
        InformationManager.DisplayMessage(new InformationMessage("Could not serialize Settings.xml: " + ex.Message.ToString() + " Using default values!", Color.FromUint(4282569842U)));
        this.CurrentSettings = new Settings();
      }
      try
      {
        AddNewGuy ang = new AddNewGuy();
        ang.addDialogs((CampaignGameStarter) starterObject);
        ((CampaignGameStarter) starterObject).AddBehavior((CampaignBehaviorBase) new DSBattleBehavior(ang));
        if ((double) this.CurrentSettings.companion_extra_lethality > 0.0)
          CampaignEvents.HeroWounded.AddNonSerializedListener((object) this, new Action<Hero>(ang.OnHeroWounded));
        if ((double) this.CurrentSettings.ai_promotion_chance > 0.0)
          CampaignEvents.MapEventEnded.AddNonSerializedListener((object) this, new Action<MapEvent>(ang.MapEventEnded));
        if (this.CurrentSettings.upgrade_to_hero)
        {
          CampaignEvents.PlayerUpgradedTroopsEvent.AddNonSerializedListener((object) this, new Action<CharacterObject, CharacterObject, int>(ang.upgrade_to_hero));
          CampaignEvents.OnUnitRecruitedEvent.AddNonSerializedListener((object) this, new Action<CharacterObject, int>(ang.recruit_to_hero));
        }
        InformationManager.DisplayMessage(new InformationMessage(new TextObject("Distinguished Service loaded successfully", (Dictionary<string, object>) null).ToString(), Colors.Blue));
      }
      catch (Exception ex)
      {
        InformationManager.DisplayMessage(new InformationMessage(new TextObject("There was a problem:\n" + ex.ToString(), (Dictionary<string, object>) null).ToString(), Colors.Blue));
      }
    }

    public override void OnGameLoaded(Game game, object initializerObject)
    {
      if (!(game.GameType is Campaign))
        return;
      try
      {
        this.DeserializeObject(Path.Combine(BasePath.Name, "Modules", "DistinguishedService", "Settings.xml"));
      }
      catch (Exception ex)
      {
        InformationManager.DisplayMessage(new InformationMessage("Could not serialize Settings.xml: " + ex.Message.ToString() + " Using default values!", Color.FromUint(4282569842U)));
        this.CurrentSettings = new Settings();
      }
      try
      {
        AddNewGuy ang = new AddNewGuy();
        ang.addDialogs((CampaignGameStarter) initializerObject);
        ((CampaignGameStarter) initializerObject).AddBehavior((CampaignBehaviorBase) new DSBattleBehavior(ang));
        if ((double) this.CurrentSettings.companion_extra_lethality > 0.0)
          CampaignEvents.HeroWounded.AddNonSerializedListener((object) this, new Action<Hero>(ang.OnHeroWounded));
        if ((double) this.CurrentSettings.ai_promotion_chance > 0.0)
          CampaignEvents.MapEventEnded.AddNonSerializedListener((object) this, new Action<MapEvent>(ang.MapEventEnded));
        if (this.CurrentSettings.upgrade_to_hero)
        {
          CampaignEvents.PlayerUpgradedTroopsEvent.AddNonSerializedListener((object) this, new Action<CharacterObject, CharacterObject, int>(ang.upgrade_to_hero));
          CampaignEvents.OnUnitRecruitedEvent.AddNonSerializedListener((object) this, new Action<CharacterObject, int>(ang.recruit_to_hero));
        }
        InformationManager.DisplayMessage(new InformationMessage(new TextObject("最佳服务加载成功 (cnedwin)", (Dictionary<string, object>) null).ToString(), Colors.Blue));
      }
      catch (Exception ex)
      {
        InformationManager.DisplayMessage(new InformationMessage(new TextObject("There was a problem:\n" + ex.ToString(), (Dictionary<string, object>) null).ToString(), Colors.Blue));
      }
    }

    private void DeserializeObject(string filename)
    {
      Settings settings;
      using (Stream stream = (Stream) new FileStream(filename, FileMode.Open))
        settings = (Settings) new XmlSerializer(typeof (Settings)).Deserialize(stream);
      this.CurrentSettings = settings;
    }

    private void SerializeObject(string filename)
    {
      Console.WriteLine("Writing With XmlTextWriter");
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (Settings));
      Settings settings1 = new Settings();
      XmlWriter xmlWriter1 = XmlWriter.Create((Stream) new FileStream(filename, FileMode.Create), new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "\t",
        OmitXmlDeclaration = true
      });
      XmlWriter xmlWriter2 = xmlWriter1;
      Settings settings2 = settings1;
      xmlSerializer.Serialize(xmlWriter2, (object) settings2);
      xmlWriter1.Close();
    }
  }
}
