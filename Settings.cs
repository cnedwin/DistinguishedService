// Decompiled with JetBrains decompiler
// Type: DistinguishedService.Settings
// Assembly: DistinguishedService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E45570B9-AC89-4B06-BD9D-0737C1CD1CD6
// Assembly location: F:\Downloads\Distinguished Service for 1.5.7b-1101-4-2-0-1611086804\DistinguishedService\bin\Win64_Shipping_Client\DistinguishedService.dll

namespace DistinguishedService
{
  public class Settings
  {
    public int up_front_cost { get; set; }

    public int tier_threshold { get; set; }

    public int battle_size_scale { get; set; }

    public double combat_perf_nomination_chance_increase_per_kill { get; set; }

    public double nomination_chance { get; set; }

    public int base_additional_skill_points { get; set; }

    public int leadership_points_per_50_extra_skill_points { get; set; }

    public int inf_kill_threshold { get; set; }

    public int cav_kill_threshold { get; set; }

    public int ran_kill_threshold { get; set; }

    public float outperform_percentile { get; set; }

    public int max_nominations { get; set; }

    public bool upgrade_to_hero { get; set; }

    public bool fill_in_perks { get; set; }

    public bool respect_companion_limit { get; set; }

    public int bonus_companion_slots_base { get; set; }

    public int bonus_companion_slots_per_clan_tier { get; set; }

    public float companion_extra_lethality { get; set; }

    public float ai_promotion_chance { get; set; }

    public int number_of_skill_bonuses { get; set; }

    public bool remove_tavern_companions { get; set; }

    public Settings()
    {
      this.up_front_cost = 0;
      this.tier_threshold = 4;
      this.battle_size_scale = 50;
      this.combat_perf_nomination_chance_increase_per_kill = 0.1;
      this.nomination_chance = 0.1;
      this.base_additional_skill_points = 100;
      this.leadership_points_per_50_extra_skill_points = 80;
      this.number_of_skill_bonuses = 3;
      this.inf_kill_threshold = 4;
      this.cav_kill_threshold = 5;
      this.ran_kill_threshold = 6;
      this.outperform_percentile = 0.68f;
      this.max_nominations = 2;
      this.upgrade_to_hero = false;
      this.fill_in_perks = false;
      this.respect_companion_limit = false;
      this.bonus_companion_slots_base = 10;
      this.bonus_companion_slots_per_clan_tier = 10;
      this.companion_extra_lethality = 0.0f;
      this.ai_promotion_chance = 1f / 1000f;
      this.remove_tavern_companions = true;
    }
  }
}
