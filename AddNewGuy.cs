// Decompiled with JetBrains decompiler
// Type: DistinguishedService.AddNewGuy
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9908B7A6-2293-42DC-B1A6-74547AA66AAB
// Assembly location: E:\Games\Lazy_Package\e1.5.6\Modules\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

using Fasterflect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace DistinguishedService
{
  public class AddNewGuy
  {
    private Random rand;
    public int begin_battle_size;
    public List<CharacterObject> nominations;
    public List<int> killcounts;
    public List<Hero> tocullonendmapevent;
    public static AddNewGuy __instance;

    public int tier_threshold { get; set; }

    private double nomination_chance { get; set; }

    private int base_additional_skill_points { get; set; }

    private int leadership_points_per_50_extra_skill_points { get; set; }

    public float combat_perf_nomination_chance_increase_per_kill { get; set; }

    public int battle_size_scale { get; set; }

    public int inf_kill_threshold { get; set; }

    public int cav_kill_threshold { get; set; }

    public int ran_kill_threshold { get; set; }

    public float outperform_percentile { get; set; }

    public int up_front_cost { get; set; }

    public int max_nominations { get; set; }

    public int mod_nominations { get; set; }

    public bool fill_perks { get; set; }

    public float kill_chance { get; set; }

    public float medicine_reduce { get; set; }

    public float companion_lethality { get; set; }

    public float ai_promotion_chance { get; set; }

    public bool respect_companion_limit { get; set; }

    public int bonus_companion_slots_base { get; set; }

    public int bonus_companion_slots_per_clan_tier { get; set; }

    public int num_skill_bonuses { get; set; }

    public bool remove_tavern_companions { get; set; }

    public AddNewGuy()
    {
      Settings settings;
      using (Stream stream = (Stream) new FileStream(Path.Combine(BasePath.Name, "Modules", "DistinguishedService", "Settings.xml"), FileMode.Open))
        settings = (Settings) new XmlSerializer(typeof (Settings)).Deserialize(stream);
      this.rand = new Random();
      this.nominations = new List<CharacterObject>();
      this.killcounts = new List<int>();
      this.battle_size_scale = settings.battle_size_scale;
      this.begin_battle_size = 0;
      this.ai_promotion_chance = settings.ai_promotion_chance;
      this.combat_perf_nomination_chance_increase_per_kill = (float) settings.combat_perf_nomination_chance_increase_per_kill;
      this.tier_threshold = settings.tier_threshold;
      this.max_nominations = settings.max_nominations;
      this.inf_kill_threshold = settings.inf_kill_threshold;
      this.cav_kill_threshold = settings.cav_kill_threshold;
      this.ran_kill_threshold = settings.ran_kill_threshold;
      this.outperform_percentile = settings.outperform_percentile;
      this.up_front_cost = settings.up_front_cost;
      this.fill_perks = settings.fill_in_perks;
      this.respect_companion_limit = settings.respect_companion_limit;
      this.bonus_companion_slots_base = settings.bonus_companion_slots_base;
      this.bonus_companion_slots_per_clan_tier = settings.bonus_companion_slots_per_clan_tier;
      this.companion_lethality = settings.companion_extra_lethality;
      this.nomination_chance = settings.nomination_chance;
      this.base_additional_skill_points = settings.base_additional_skill_points;
      this.leadership_points_per_50_extra_skill_points = settings.leadership_points_per_50_extra_skill_points;
      this.num_skill_bonuses = settings.number_of_skill_bonuses;
      this.remove_tavern_companions = settings.remove_tavern_companions;
      string[] strArray = new string[20];
      strArray[0] = "最多提名 = ";
      strArray[1] = this.max_nominations.ToString();
      strArray[2] = "\n等级阈值 = ";
      int num = this.tier_threshold;
      strArray[3] = num.ToString();
      strArray[4] = "\n提名概率 = ";
      strArray[5] = this.nomination_chance.ToString();
      strArray[6] = "\n杀敌数量:\n步兵 = ";
      num = this.inf_kill_threshold;
      strArray[7] = num.ToString();
      strArray[8] = ", 骑兵 = ";
      num = this.cav_kill_threshold;
      strArray[9] = num.ToString();
      strArray[10] = ", 远程 = ";
      num = this.ran_kill_threshold;
      strArray[11] = num.ToString();
      strArray[12] = "\n增加技能点数 = ";
      num = this.base_additional_skill_points;
      strArray[13] = num.ToString();
      strArray[14] = "\n领导点数 = ";
      num = this.leadership_points_per_50_extra_skill_points;
      strArray[15] = num.ToString();
      strArray[16] = "\n每杀死一个敌人奖励几率 = ";
      strArray[17] = this.combat_perf_nomination_chance_increase_per_kill.ToString();
      strArray[18] = "\n有效战役规模 = ";
      num = this.battle_size_scale;
      strArray[19] = num.ToString();
      InformationManager.DisplayMessage(new InformationMessage(string.Concat(strArray), Color.FromUint(4282569842U)));
      AddNewGuy.__instance = this;
      this.tocullonendmapevent = new List<Hero>();
    }

    public void OnHeroWounded(Hero _h)
    {
      if (new Random().NextDouble() > (double) this.companion_lethality || _h.CharacterObject.Occupation != Occupation.Wanderer)
        return;
      KillCharacterAction.ApplyByBattle(_h);
    }

    public void EnsureWanderers(CampaignGameStarter gcs)
    {
      foreach (Hero hero in Hero.All)
      {
        if (hero.CompanionOf != null && !hero.IsWanderer)
          hero.CharacterObject.TrySetPropertyValue("Occupation", (object) Occupation.Wanderer);
      }
    }

    public void MapEventEnded(MapEvent me)
    {
      if (!me.HasWinner)
        return;
      Random random = new Random();
      foreach (PartyBase partyBase in me.PartiesOnSide(me.WinningSide))
      {
        if (partyBase != null && partyBase != PartyBase.MainParty && (partyBase.LeaderHero != null && random.NextDouble() <= (double) this.ai_promotion_chance))
        {
          List<CharacterObject> characterObjectList1;
          if (partyBase == null)
          {
            characterObjectList1 = (List<CharacterObject>) null;
          }
          else
          {
            TroopRoster memberRoster = partyBase.MemberRoster;
            if ((object) memberRoster == null)
            {
              characterObjectList1 = (List<CharacterObject>) null;
            }
            else
            {
              IEnumerable<CharacterObject> troops = memberRoster.Troops;
              characterObjectList1 = troops != null ? troops.ToList<CharacterObject>() : (List<CharacterObject>) null;
            }
          }
          List<CharacterObject> characterObjectList2 = characterObjectList1;
          if (characterObjectList2 != null)
          {
            this.Shuffle<CharacterObject>((IList<CharacterObject>) characterObjectList2);
            foreach (CharacterObject co in characterObjectList2)
            {
              if (co != null && !co.IsHero && (co.IsSoldier && AddNewGuy.is_soldier_qualified(co)))
              {
                this.AddNewGuyToParty(co, partyBase.MobileParty);
                break;
              }
            }
          }
        }
      }
    }

    public void OnPCBattleEnded_results()
    {
      if (this.respect_companion_limit && Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit)
      {
        InformationManager.DisplayMessage(new InformationMessage(new TextObject("已经达到同伴上限，无法晋升新的跟随者。", (Dictionary<string, object>) null).ToString(), Colors.Blue));
      }
      else
      {
        float num1 = 0.0f;
        float num2 = this.begin_battle_size != 0 ? (float) (1.0 + (double) this.begin_battle_size / (double) this.battle_size_scale) : 1f;
        List<CharacterObject> _stripped_noms = new List<CharacterObject>();
        List<int> _stripped_kcs = new List<int>();
        if (this.nominations.Count > 0 && this.killcounts.Count > 0)
        {
          for (int index = 0; index < this.nominations.Count; ++index)
          {
            if (MobileParty.MainParty.MemberRoster.Contains(this.nominations[index]) && this.nominations[index] != null && this.nominations[index].HitPoints > 0)
            {
              _stripped_noms.Add(this.nominations[index]);
              _stripped_kcs.Add(this.killcounts[index]);
            }
          }
          num1 = this.combat_perf_nomination_chance_increase_per_kill * (float) this.killcounts.Max();
        }
        if (this.rand.NextDouble() > this.nomination_chance * (double) num2 + (double) num1)
          return;
        if (_stripped_noms.Count == 0)
        {
          foreach (CharacterObject characterObject in new List<CharacterObject>(MobileParty.MainParty.MemberRoster.Troops).OrderBy<CharacterObject, int>((Func<CharacterObject, int>) (o => o.Tier)).Reverse<CharacterObject>().ToList<CharacterObject>())
          {
            CharacterObject co = characterObject;
            if (co != null && co.HitPoints > 0 && (!co.IsHero && co.Tier >= this.tier_threshold))
            {
              InformationManager.ShowInquiry(new InquiryData(new TextObject("获得勋章的士兵", (Dictionary<string, object>) null).ToString(), new TextObject("一位默默无闻的战士 " + co.Name?.ToString() + " 因为在战斗中表现突出，他们受到了战友的赞扬，您也亲自对他们授予了表彰。现在您可以为他们提供一个在您身边战斗的机会，将他们晋升为您的追随者，您愿意吗？", (Dictionary<string, object>) null).ToString(), true, true, new TextObject("十分荣幸", (Dictionary<string, object>) null).ToString(), new TextObject("下次再说", (Dictionary<string, object>) null).ToString(), (Action) (() =>
              {
                try
                {
                  this.giveNewGuy(co);
                  if (!MobileParty.MainParty.MemberRoster.Contains(co))
                    return;
                  MobileParty.MainParty.MemberRoster.RemoveTroop(co);
                }
                catch (Exception ex)
                {
                  InformationManager.ShowInquiry(new InquiryData(new TextObject("战斗结算引发了异常", (Dictionary<string, object>) null).ToString(), new TextObject(ex.ToString() + "\n\n请截图报告给作者.", (Dictionary<string, object>) null).ToString(), true, false, new TextObject("好的", (Dictionary<string, object>) null).ToString(), (string) null, (Action) null, (Action) null));
                }
              }), (Action) null), true);
              break;
            }
          }
        }
        else
        {
          List<CharacterObject> list = new List<CharacterObject>((IEnumerable<CharacterObject>) _stripped_noms).OrderBy<CharacterObject, int>((Func<CharacterObject, int>) (o => _stripped_kcs[_stripped_noms.IndexOf(o)])).Reverse<CharacterObject>().ToList<CharacterObject>();
          _stripped_kcs = new List<int>((IEnumerable<int>) _stripped_kcs).OrderBy<int, int>((Func<int, int>) (o => _stripped_kcs[_stripped_kcs.IndexOf(o)])).Reverse<int>().ToList<int>();
          this.mod_nominations = !this.respect_companion_limit || list.Count + Clan.PlayerClan.Companions.Count <= Clan.PlayerClan.CompanionLimit ? this.max_nominations : Clan.PlayerClan.CompanionLimit - Clan.PlayerClan.Companions.Count;
          InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("最佳服务勋章", (Dictionary<string, object>) null).ToString(), new TextObject("在这场战斗中，有几名士兵为战功卓越。 您最多可以选择 " + this.mod_nominations.ToString() + " 名(或不选) 作为跟随者在你身边战斗。", (Dictionary<string, object>) null).ToString(), this.gen_inquiryelements(list, _stripped_kcs), true, this.mod_nominations, new TextObject("晋升他们", (Dictionary<string, object>) null).ToString(), new TextObject("下次再说", (Dictionary<string, object>) null).ToString(), new Action<List<InquiryElement>>(this.OnNomineeSelect), (Action<List<InquiryElement>>) null), true);
        }
      }
    }

    public List<InquiryElement> gen_inquiryelements(
      List<CharacterObject> _cos,
      List<int> _kills)
    {
      List<InquiryElement> inquiryElementList = new List<InquiryElement>();
      for (int index = 0; index < _cos.Count; ++index)
      {
        if (MobileParty.MainParty.MemberRoster.Contains(_cos[index]))
          inquiryElementList.Add(new InquiryElement((object) _cos[index], _cos[index].Name.ToString(), new ImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject) _cos[index])), true, _kills[index].ToString() + " kills"));
      }
      return inquiryElementList;
    }

    public static bool is_soldier_qualified(CharacterObject co)
    {
      if (co == null)
        return false;
      InformationManager.DisplayMessage(new InformationMessage(new TextObject("Testing " + co.Name?.ToString(), (Dictionary<string, object>) null).ToString(), Colors.Blue));
      if (AddNewGuy.__instance.tier_threshold < 0)
      {
        if (co.UpgradeTargets == null || co.UpgradeTargets.Length == 0)
          return true;
      }
      else if (co.Tier >= AddNewGuy.__instance.tier_threshold)
        return true;
      return false;
    }

    public List<InquiryElement> gen_transfer_list(List<CharacterObject> _cos)
    {
      List<InquiryElement> inquiryElementList = new List<InquiryElement>();
      foreach (CharacterObject co in _cos)
      {
        if (MobileParty.MainParty.MemberRoster.Contains(co))
          inquiryElementList.Add(new InquiryElement((object) co, co.Name.ToString(), new ImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject) co)), true, " kills"));
      }
      return inquiryElementList;
    }

    public void OnNomineeSelect(List<InquiryElement> ies)
    {
      foreach (InquiryElement y in ies)
      {
        CharacterObject identifier = (CharacterObject) y.Identifier;
        try
        {
          this.giveNewGuy(identifier);
          MobileParty.MainParty.MemberRoster.RemoveTroop(identifier);
        }
        catch (Exception ex)
        {
          InformationManager.ShowInquiry(new InquiryData(new TextObject("提名时发生错误", (Dictionary<string, object>) null).ToString(), new TextObject(ex.ToString() + "\n\n请截图报告给作者", (Dictionary<string, object>) null).ToString(), true, false, new TextObject("好的", (Dictionary<string, object>) null).ToString(), (string) null, (Action) null, (Action) null));
        }
      }
    }

    public int getCharTier(CharacterObject co) => co.Tier;

    public string get_ds_template(CharacterObject co)
    {
      if (co.IsFemale)
      {
        switch (co.Culture.GetCultureCode())
        {
          case CultureCode.Sturgia:
            return "ds_template_inf_fe_sturgia";
          case CultureCode.Aserai:
            return "ds_template_inf_fe_aserai";
          case CultureCode.Vlandia:
            return "ds_template_inf_fe_vlandia";
          case CultureCode.Khuzait:
            return "ds_template_inf_fe_khuzait";
          case CultureCode.Battania:
            return "ds_template_inf_fe_battania";
          case CultureCode.Nord:
            return "ds_template_inf_fe_sturgia";
          case CultureCode.Darshi:
            return "ds_template_inf_fe_aserai";
          case CultureCode.Vakken:
            return "ds_template_inf_fe_sturgia";
          default:
            return "ds_template_inf_fe";
        }
      }
      else
      {
        switch (co.Culture.GetCultureCode())
        {
          case CultureCode.Sturgia:
            return "ds_template_inf_sturgia";
          case CultureCode.Aserai:
            return "ds_template_inf_aserai";
          case CultureCode.Vlandia:
            return "ds_template_inf_vlandia";
          case CultureCode.Khuzait:
            return "ds_template_inf_khuzait";
          case CultureCode.Battania:
            return "ds_template_inf_battania";
          case CultureCode.Nord:
            return "ds_template_inf_sturgia";
          case CultureCode.Darshi:
            return "ds_template_inf_aserai";
          case CultureCode.Vakken:
            return "ds_template_inf_sturgia";
          default:
            return "ds_template_inf";
        }
      }
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("give_squad", "dservice")]
    public static string GiveSquad(List<string> strings)
    {
      int result = 10;
      int num = 0;
      if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
        result = 10;
      else if (!int.TryParse(strings[0], out result))
        return "Incorrect number given.\nUsage: give_squad [num]";
      CultureObject culture = Hero.MainHero.Culture;
      while (num < result)
      {
        foreach (PartyTemplateStack stack in culture.EliteCaravanPartyTemplate.Stacks)
        {
          CharacterObject from = CharacterObject.CreateFrom(stack.Character, false);
          if ((double) MBRandom.RandomFloat < 0.5)
            from.TrySetPropertyValue("IsFemale", (object) true);
          AddNewGuy.__instance.giveNewGuy(from);
          ++num;
          if (num == result)
            break;
        }
      }
      return "小队 " + result.ToString() + "被授予。";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("uplift_soldier", "dservice")]
    public static string NewGuyCheat(List<string> strings)
    {
      int result = -1;
      if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
        return "Usage: uplift_soldier [tier threshold = 0]";
      if (!int.TryParse(strings[0], out result))
        result = 0;
      List<CharacterObject> characterObjectList = new List<CharacterObject>(MobileParty.MainParty.MemberRoster.Troops);
      AddNewGuy.__instance.Shuffle<CharacterObject>((IList<CharacterObject>) characterObjectList);
      foreach (CharacterObject characterObject in characterObjectList)
      {
        if (!characterObject.IsHero && characterObject.Tier >= result)
        {
          AddNewGuy.__instance.giveNewGuy(characterObject);
          MobileParty.MainParty.MemberRoster.RemoveTroop(characterObject);
          return "Created new companion!";
        }
      }
      return "无人晋升！";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("convert_party_to_heroes", "dservice")]
    public static string PartyToHeroes(List<string> strings)
    {
      bool flag = true;
      while (flag)
      {
        flag = false;
        foreach (CharacterObject troop in MobileParty.MainParty.MemberRoster.Troops)
        {
          if (!troop.IsHero)
          {
            AddNewGuy.__instance.giveNewGuy(troop);
            MobileParty.MainParty.MemberRoster.RemoveTroop(troop);
          }
        }
        foreach (BasicCharacterObject troop in MobileParty.MainParty.MemberRoster.Troops)
        {
          if (!troop.IsHero)
          {
            flag = true;
            break;
          }
        }
      }
      return "玩家部队已转移";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("give_party_heroes_perks", "dservice")]
    public static string HeroesTakePerks(List<string> strings)
    {
      foreach (CharacterObject troop in MobileParty.MainParty.MemberRoster.Troops)
      {
        if (troop.IsHero && troop.HeroObject != Hero.MainHero)
        {
          foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
            troop.HeroObject.HeroDeveloper.TakeAllPerks(allSkill);
        }
      }
      return "部队追随者技能点已添加！";
    }

    public void recruit_to_hero(CharacterObject troop, int amount)
    {
      if (troop.Tier < this.tier_threshold)
        return;
      for (int numberToRemove = 0; numberToRemove < amount; ++numberToRemove)
      {
        if (this.respect_companion_limit && Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit)
        {
          MobileParty.MainParty.MemberRoster.RemoveTroop(troop, numberToRemove);
          return;
        }
        this.giveNewGuy(troop);
      }
      MobileParty.MainParty.MemberRoster.RemoveTroop(troop, amount);
    }

    public void upgrade_to_hero(
      CharacterObject upgradeFromTroop,
      CharacterObject upgradeToTroop,
      int number)
    {
      if (upgradeToTroop.Tier < AddNewGuy.__instance.tier_threshold)
        return;
      for (int numberToRemove = 0; numberToRemove < number; ++numberToRemove)
      {
        if (this.respect_companion_limit && Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit)
        {
          MobileParty.MainParty.MemberRoster.RemoveTroop(upgradeToTroop, numberToRemove);
          return;
        }
        AddNewGuy.__instance.giveNewGuy(upgradeToTroop);
      }
      MobileParty.MainParty.MemberRoster.RemoveTroop(upgradeToTroop, number);
    }

    public string getNameSuffix(CharacterObject co)
    {
      List<string> list1 = new List<string>();
      if (co.IsArcher)
        list1.AppendList<string>(NameList.archer_suff);
      else if (!co.FirstBattleEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
        list1.AppendList<string>(NameList.cavalry_suff);
      else
        list1.AppendList<string>(NameList.infantry_suff);
      switch (co.Culture.GetCultureCode())
      {
        case CultureCode.Sturgia:
          list1.AppendList<string>(NameList.sturgian_suff);
          break;
        case CultureCode.Aserai:
          list1.AppendList<string>(NameList.aserai_suff);
          break;
        case CultureCode.Vlandia:
          list1.AppendList<string>(NameList.vlandian_suff);
          break;
        case CultureCode.Khuzait:
          list1.AppendList<string>(NameList.khuzait_suff);
          break;
        case CultureCode.Battania:
          list1.AppendList<string>(NameList.battanian_suff);
          break;
        case CultureCode.Nord:
          list1.AppendList<string>(NameList.sturgian_suff);
          break;
        case CultureCode.Darshi:
          list1.AppendList<string>(NameList.aserai_suff);
          break;
        case CultureCode.Vakken:
          list1.AppendList<string>(NameList.sturgian_suff);
          break;
      }
      if (co.Culture.StringId == "criminals")
      {
        list1.Clear();
        list1.AppendList<string>(NameList.criminal_suff);
      }
      else if (co.Culture.StringId == "criminals2")
      {
        list1.Clear();
        list1.AppendList<string>(NameList.criminal2_suff);
      }
      else if (co.Culture.StringId == "criminals3")
      {
        list1.Clear();
        list1.AppendList<string>(NameList.criminal3_suff);
      }
      else if (co.Culture.StringId == "pirates")
      {
        list1.Clear();
        list1.AppendList<string>(NameList.pirates_suff);
      }
      else if (co.Culture.StringId == "shift_sand")
      {
        list1.Clear();
        list1.AppendList<string>(NameList.shiftingsands_suff);
      }
      return list1[MBRandom.RandomInt(list1.Count)];
    }

    public void giveNewGuy(CharacterObject co)
    {
      if (co == null)
        return;
      CharacterObject randomElement = CharacterObject.Templates.Where<CharacterObject>((Func<CharacterObject, bool>) (x => x.StringId == this.get_ds_template(co))).GetRandomElement<CharacterObject>();
      randomElement.Culture = co.Culture;
      Hero specialHero = HeroCreator.CreateSpecialHero(randomElement, age: this.rand.Next(20, 50));
      specialHero.Name = new TextObject(specialHero.FirstName.ToString() + this.getNameSuffix(co), (Dictionary<string, object>) null);
      try
      {
        specialHero.CharacterObject.TrySetPropertyValue("Occupation", (object) Occupation.Wanderer);
        specialHero.CharacterObject.TrySetPropertyValue("DefaultFormationClass", (object) co.DefaultFormationClass);
        specialHero.CharacterObject.TrySetPropertyValue("DefaultFormationGroup", (object) co.DefaultFormationGroup);
      }
      catch (Exception ex)
      {
        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=EWD4Op6d}Exception Thrown in GiveNewGuy", (Dictionary<string, object>) null).ToString(), new TextObject("Fasterflect exception setting occupation to wanderer:\n\n" + ex.ToString(), (Dictionary<string, object>) null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", (Dictionary<string, object>) null).ToString(), (string) null, (Action) null, (Action) null));
      }
      specialHero.ChangeState(Hero.CharacterStates.Active);
      AddCompanionAction.Apply(Clan.PlayerClan, specialHero);
      AddHeroToPartyAction.Apply(specialHero, MobileParty.MainParty);
      CampaignEventDispatcher.Instance.OnHeroCreated(specialHero, false);
      this.addTraitVariance(specialHero);
      GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, specialHero, this.up_front_cost);
      specialHero.HasMet = true;
      specialHero.BattleEquipment.FillFrom(co.FirstBattleEquipment);
      this.AdjustEquipment(specialHero);
      if (co.IsMounted)
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 1 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 4 + this.rand.Next(3), false);
      }
      else if (co.IsArcher)
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 4 + this.rand.Next(3), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 1 + this.rand.Next(2), false);
      }
      else
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 3 + this.rand.Next(3), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 2 + this.rand.Next(2), false);
      }
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Cunning, 1 + this.rand.Next(3), false);
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Social, 1 + this.rand.Next(3), false);
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Intelligence, 1 + this.rand.Next(3), false);
      foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
        specialHero.HeroDeveloper.ChangeSkillLevel(allSkill, co.GetSkillValue(allSkill), false);
      List<SkillObject> skillObjectList = new List<SkillObject>(DefaultSkills.GetAllSkills());
      this.Shuffle<SkillObject>((IList<SkillObject>) skillObjectList);
      int num = this.base_additional_skill_points + 50 * Hero.MainHero.GetSkillValue(DefaultSkills.Leadership) / this.leadership_points_per_50_extra_skill_points;
      specialHero.HeroDeveloper.SetInitialLevel(co.Level);
      foreach (SkillObject skill in skillObjectList)
      {
        int changeAmount = skill == DefaultSkills.OneHanded || skill == DefaultSkills.TwoHanded || (skill == DefaultSkills.Polearm || skill == DefaultSkills.Bow) || (skill == DefaultSkills.Crossbow || skill == DefaultSkills.Throwing) ? this.rand.Next(10) + this.rand.Next(15) : this.rand.Next(10) + this.rand.Next(15) + this.rand.Next(25);
        num -= changeAmount;
        if (num < 0)
          changeAmount += num;
        specialHero.HeroDeveloper.ChangeSkillLevel(skill, changeAmount, false);
        if (num <= 0)
          break;
      }
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("选择技能专精", specialHero.Name?.ToString() + " (前身是一个 " + co.Name?.ToString() + ")的士兵最终赢得了战场上的荣耀. 在此之前, 除了训练战斗，他们...", new List<InquiryElement>()
      {
        new InquiryElement((object) "scout_bonus", "和侦查兵并一起巡逻", (ImageIdentifier) null, true, "+100 侦查"),
        new InquiryElement((object) "smithing_bonus", "修理部队的武器", (ImageIdentifier) null, true, "+100 锻造"),
        new InquiryElement((object) "athletics_bonus", "为战斗训练身体素质", (ImageIdentifier) null, true, "+100 跑动"),
        new InquiryElement((object) "riding_bonus", "练习骑马", (ImageIdentifier) null, true, "+100 骑术"),
        new InquiryElement((object) "tactics_bonus", "研究过去的战斗", (ImageIdentifier) null, true, "+100 战术"),
        new InquiryElement((object) "roguery_bonus", "在黑市贩卖战利品", (ImageIdentifier) null, true, "+100 流氓习气"),
        new InquiryElement((object) "charm_bonus", "和所有人谈笑风生", (ImageIdentifier) null, true, "+100 魅力"),
        new InquiryElement((object) "leadership_bonus", "在部队中组织各种值守", (ImageIdentifier) null, true, "+100 统帅"),
        new InquiryElement((object) "trade_bonus", "从访问过的城市买卖军需", (ImageIdentifier) null, true, "+100 交易"),
        new InquiryElement((object) "steward_bonus", "帮助部队处理账目", (ImageIdentifier) null, true, "+100 管理"),
        new InquiryElement((object) "medicine_bonus", "作为医生帮助战友", (ImageIdentifier) null, true, "+100 医术"),
        new InquiryElement((object) "engineering_bonus", "帮助建造和撤下营地", (ImageIdentifier) null, true, "+100 工程")
      }, false, this.num_skill_bonuses, "同意", "拒绝", (Action<List<InquiryElement>>) (ies =>
      {
        foreach (InquiryElement y in ies)
        {
          switch ((string) y.Identifier)
          {
            case "athletics_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Athletics, 100);
              continue;
            case "charm_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Charm, 100);
              continue;
            case "engineering_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Engineering, 100);
              continue;
            case "leadership_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Leadership, 100);
              continue;
            case "medicine_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Medicine, 100);
              continue;
            case "riding_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Riding, 100);
              continue;
            case "roguery_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Roguery, 100);
              continue;
            case "scout_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Scouting, 100);
              continue;
            case "smithing_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Crafting, 100);
              continue;
            case "steward_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Steward, 100);
              continue;
            case "tactics_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Tactics, 100);
              continue;
            case "trade_bonus":
              specialHero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Trade, 100);
              continue;
            default:
              continue;
          }
        }
      }), (Action<List<InquiryElement>>) null), true);
      if (!this.fill_perks)
        return;
      foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
        specialHero.HeroDeveloper.TakeAllPerks(allSkill);
    }

    public void AddNewGuyToParty(CharacterObject co, MobileParty party)
    {
      if (co == null || party == null)
        return;
      Hero leaderHero = party?.LeaderHero;
      if (leaderHero == null)
        return;
      CharacterObject randomElement = CharacterObject.Templates.Where<CharacterObject>((Func<CharacterObject, bool>) (x => x.StringId == this.get_ds_template(co))).GetRandomElement<CharacterObject>();
      randomElement.Culture = co.Culture;
      Hero specialHero = HeroCreator.CreateSpecialHero(randomElement, age: this.rand.Next(20, 50));
      specialHero.Name = new TextObject(specialHero.FirstName.ToString() + this.getNameSuffix(co), (Dictionary<string, object>) null);
      try
      {
        specialHero.CharacterObject.TrySetPropertyValue("Occupation", (object) Occupation.Wanderer);
      }
      catch (Exception ex)
      {
        InformationManager.ShowInquiry(new InquiryData(new TextObject("获得追随者发生错误", (Dictionary<string, object>) null).ToString(), new TextObject("将职业设为流浪者:\n\n" + ex.ToString(), (Dictionary<string, object>) null).ToString(), true, false, new TextObject("确定", (Dictionary<string, object>) null).ToString(), (string) null, (Action) null, (Action) null));
      }
      specialHero.ChangeState(Hero.CharacterStates.Active);
      AddCompanionAction.Apply(leaderHero.Clan, specialHero);
      AddHeroToPartyAction.Apply(specialHero, party);
      CampaignEventDispatcher.Instance.OnHeroCreated(specialHero, false);
      this.addTraitVariance(specialHero);
      GiveGoldAction.ApplyBetweenCharacters(leaderHero, specialHero, this.up_front_cost, true);
      specialHero.HasMet = true;
      specialHero.BattleEquipment.FillFrom(co.FirstBattleEquipment);
      this.AdjustEquipment(specialHero);
      if (co.IsMounted)
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 1 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 4 + this.rand.Next(3), false);
      }
      else if (co.IsArcher)
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 4 + this.rand.Next(3), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 1 + this.rand.Next(2), false);
      }
      else
      {
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, 3 + this.rand.Next(3), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, 2 + this.rand.Next(2), false);
        specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, 2 + this.rand.Next(2), false);
      }
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Cunning, 1 + this.rand.Next(3), false);
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Social, 1 + this.rand.Next(3), false);
      specialHero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Intelligence, 1 + this.rand.Next(3), false);
      foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
        specialHero.HeroDeveloper.ChangeSkillLevel(allSkill, co.GetSkillValue(allSkill), false);
      List<SkillObject> skillObjectList = new List<SkillObject>(DefaultSkills.GetAllSkills());
      this.Shuffle<SkillObject>((IList<SkillObject>) skillObjectList);
      int num = this.base_additional_skill_points + 50 * Hero.MainHero.GetSkillValue(DefaultSkills.Leadership) / this.leadership_points_per_50_extra_skill_points;
      specialHero.HeroDeveloper.SetInitialLevel(co.Level);
      foreach (SkillObject skill in skillObjectList)
      {
        int changeAmount = skill == DefaultSkills.OneHanded || skill == DefaultSkills.TwoHanded || (skill == DefaultSkills.Polearm || skill == DefaultSkills.Bow) || (skill == DefaultSkills.Crossbow || skill == DefaultSkills.Throwing) ? this.rand.Next(10) + this.rand.Next(15) : this.rand.Next(10) + this.rand.Next(15) + this.rand.Next(25);
        num -= changeAmount;
        if (num < 0)
          changeAmount += num;
        specialHero.HeroDeveloper.ChangeSkillLevel(skill, changeAmount, false);
        if (num <= 0)
          break;
      }
      if (leaderHero == Hero.MainHero && !this.fill_perks)
        return;
      foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
        specialHero.HeroDeveloper.TakeAllPerks(allSkill);
    }

    public void AdjustEquipment(Hero _h)
    {
      Equipment battleEquipment = _h.BattleEquipment;
      ItemModifier itemModifier1 = MBObjectManager.Instance.GetObject<ItemModifier>("ds_armor");
      ItemModifier itemModifier2 = MBObjectManager.Instance.GetObject<ItemModifier>("ds_weapon");
      ItemModifier itemModifier3 = MBObjectManager.Instance.GetObject<ItemModifier>("ds_horse");
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumEquipmentSetSlots; ++index)
      {
        EquipmentElement equipmentElement = battleEquipment[index];
        if (equipmentElement.Item != null)
        {
          if (equipmentElement.Item.ArmorComponent != null)
            battleEquipment[index] = new EquipmentElement(equipmentElement.Item, itemModifier1);
          else if (equipmentElement.Item.HorseComponent != null)
            battleEquipment[index] = new EquipmentElement(equipmentElement.Item, itemModifier3);
          else if (equipmentElement.Item.WeaponComponent != null)
            battleEquipment[index] = new EquipmentElement(equipmentElement.Item, itemModifier2);
        }
      }
    }

    public void addTraitVariance(Hero hero)
    {
      foreach (TraitObject trait in DefaultTraits.All)
      {
        if (trait == DefaultTraits.Honor || trait == DefaultTraits.Mercy || (trait == DefaultTraits.Generosity || trait == DefaultTraits.Valor) || trait == DefaultTraits.Calculating)
        {
          int num1 = hero.CharacterObject.GetTraitLevel(trait);
          float randomFloat = MBRandom.RandomFloat;
          if ((double) Hero.MainHero.GetTraitLevel(trait) >= 0.9)
            randomFloat *= 1.2f;
          if ((double) randomFloat < 0.1)
          {
            --num1;
            if (num1 < -1)
              num1 = -1;
          }
          if ((double) randomFloat > 0.9)
          {
            ++num1;
            if (num1 > 1)
              num1 = 1;
          }
          int num2 = MBMath.ClampInt(num1, trait.MinValue, trait.MaxValue);
          hero.SetTraitLevel(trait, num2);
        }
      }
    }

    public void Shuffle<T>(IList<T> list)
    {
      int count = list.Count;
      while (count > 1)
      {
        --count;
        int index = this.rand.Next(count + 1);
        T obj = list[index];
        list[index] = list[count];
        list[count] = obj;
      }
    }

    public void addDialogs(CampaignGameStarter campaignGameStarter)
    {
      campaignGameStarter.AddPlayerLine("companion_change_name_start", "hero_main_options", "companion_change_name_confirm", "我将赐予你新的姓名...", new ConversationSentence.OnConditionDelegate(this.namechangecondition), new ConversationSentence.OnConsequenceDelegate(this.namechanceconsequence));
      campaignGameStarter.AddDialogLine("companion_change_name_confirm", "companion_change_name_confirm", "hero_main_options", "就这样吧", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
      campaignGameStarter.AddPlayerLine("companion_transfer_start", "hero_main_options", "companion_transfer_confirm", "带上这些人加入你的队伍...", new ConversationSentence.OnConditionDelegate(this.givecomptoclanpartycondition), new ConversationSentence.OnConsequenceDelegate(this.givecomptoclanpartyconsequence));
      campaignGameStarter.AddDialogLine("companion_transfer_confirm", "companion_transfer_confirm", "hero_main_options", "就这样吧", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
      campaignGameStarter.AddPlayerLine("companion_takeback_start", "hero_main_options", "companion_takeback_confirm", "我要给你重新分配队伍里的英雄...", new ConversationSentence.OnConditionDelegate(this.takecompfromclanpartycondition), new ConversationSentence.OnConsequenceDelegate(this.takecompfromclanpartyconsequence));
      campaignGameStarter.AddDialogLine("companion_takeback_confirm", "companion_takeback_confirm", "hero_main_options", "就这样吧", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
    }

    private bool namechangecondition() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.IsPlayerCompanion;

    private void namechanceconsequence() => InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("新的名字: ", (Dictionary<string, object>) null).ToString(), string.Empty, true, false, GameTexts.FindText("str_done").ToString(), (string) null, new Action<string>(this.change_hero_name), (Action) null));

    private void change_hero_name(string s) => Hero.OneToOneConversationHero.Name = new TextObject("{=6dvryoMH}" + s, (Dictionary<string, object>) null);

    private bool givecomptoclanpartycondition() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.IsPlayerCompanion && Hero.OneToOneConversationHero.IsPartyLeader;

    private void givecomptoclanpartyconsequence() => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("转移英雄", (Dictionary<string, object>) null).ToString(), new TextObject("你可以选择谁要加入 " + Hero.OneToOneConversationHero.Name?.ToString() + "的部队.", (Dictionary<string, object>) null).ToString(), this.gen_transfer_list(this.player_party_heroes()), true, PartyBase.MainParty.MemberRoster.Count, new TextObject("选好了", (Dictionary<string, object>) null).ToString(), new TextObject("没有合适的", (Dictionary<string, object>) null).ToString(), new Action<List<InquiryElement>>(this.transfer_characters_to_conversation), (Action<List<InquiryElement>>) null), true);

    private List<CharacterObject> player_party_heroes()
    {
      List<CharacterObject> characterObjectList = new List<CharacterObject>();
      foreach (CharacterObject troop in PartyBase.MainParty.MemberRoster.Troops)
      {
        if (troop.IsHero && !troop.IsPlayerCharacter)
          characterObjectList.Add(troop);
      }
      return characterObjectList;
    }

    private void transfer_characters_to_conversation(List<InquiryElement> ies)
    {
      MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
      foreach (InquiryElement y in ies)
        AddHeroToPartyAction.Apply(((CharacterObject) y.Identifier).HeroObject, partyBelongedTo);
    }

    private bool takecompfromclanpartycondition() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.IsPlayerCompanion && Hero.OneToOneConversationHero.IsPartyLeader;

    private void takecompfromclanpartyconsequence() => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("转移英雄", (Dictionary<string, object>) null).ToString(), new TextObject("您可以选择谁要加入 " + Hero.OneToOneConversationHero.Name?.ToString() + "的部队。", (Dictionary<string, object>) null).ToString(), this.gen_transfer_list(this.conversation_party_heroes()), true, PartyBase.MainParty.MemberRoster.Count, new TextObject("选好了", (Dictionary<string, object>) null).ToString(), new TextObject("没有合适的", (Dictionary<string, object>) null).ToString(), new Action<List<InquiryElement>>(this.transfer_characters_from_conversation), (Action<List<InquiryElement>>) null), true);

    private List<CharacterObject> conversation_party_heroes()
    {
      PartyBase party = Hero.OneToOneConversationHero.PartyBelongedTo.Party;
      List<CharacterObject> characterObjectList = new List<CharacterObject>();
      foreach (CharacterObject troop in party.MemberRoster.Troops)
      {
        if (troop.IsHero && troop.HeroObject != Hero.OneToOneConversationHero)
          characterObjectList.Add(troop);
      }
      return characterObjectList;
    }

    private void transfer_characters_from_conversation(List<InquiryElement> ies)
    {
      foreach (InquiryElement y in ies)
        AddHeroToPartyAction.Apply(((CharacterObject) y.Identifier).HeroObject, MobileParty.MainParty);
    }
  }
}
