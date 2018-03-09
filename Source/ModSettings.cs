using Verse;
using UnityEngine;
using SettingsHelper;

namespace Calm_Down
{
    class CDSettings : ModSettings
    {
        //Vars
        #region vars
        public bool CDmessagesEnabled = true;
        public bool CDAggroCalmEnabled = false;
        public bool CDOpnOnly = false;
        public bool CDNonFaction = true;
        public bool CDTraderCalm = true;
        public bool CDNoCath = false;
        public bool CDAdvanced = false;
        public bool CDDebugChances = false;
        public float CDDipWeight = 0.2f;
        public float CDOpnWeight = 0.0014f;
        public float CDOOpnWeight = 0.006f;
        public float CDStunWeight = 0.55f;
        public int CDCalmDuration = 1250;
        public int CDCooldown = 15000;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.CDmessagesEnabled, "CDMessagesEnabled", true);
            Scribe_Values.Look<bool>(ref this.CDAggroCalmEnabled, "CDAggroCalmEnabled", false);
            Scribe_Values.Look<bool>(ref this.CDOpnOnly, "CDOpnOnly", false);
            Scribe_Values.Look<bool>(ref this.CDNonFaction, "CDNonFaction", true);
            Scribe_Values.Look<bool>(ref this.CDTraderCalm, "CDTraderCalm", true);
            Scribe_Values.Look<bool>(ref this.CDNoCath, "CDNoCath", true);
            Scribe_Values.Look<bool>(ref this.CDAdvanced, "CDAdvancedMenu", false);
            Scribe_Values.Look<bool>(ref this.CDDebugChances, "CDDebugChances", false);
            Scribe_Values.Look<float>(ref this.CDStunWeight, "CDStunWeight", 0.45f);
            Scribe_Values.Look<float>(ref this.CDDipWeight, "CDDipWeight", 0.2f);
            Scribe_Values.Look<float>(ref this.CDOpnWeight, "CDOpnWeight", 0.0014f);
            Scribe_Values.Look<float>(ref this.CDOOpnWeight, "CDOOpnWeight", 0.006f);
            Scribe_Values.Look<int>(ref this.CDCalmDuration, "CDCalmDuration", 1250);
            Scribe_Values.Look<int>(ref this.CDCooldown, "CDCoolDown", 15000);
        }
        #endregion
    }

    class CDMod : Mod
    {
        //CDSettings
        #region cdsettings
        public static CDSettings settings;

        public CDMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<CDSettings>();
        }
   
        public override string SettingsCategory() => "SettingsCategoryLabel".Translate();
        #endregion

        //Main window thing
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.AddLabeledCheckbox("MessagesEnabledLabel".Translate() +" ", ref settings.CDmessagesEnabled);
            listing_Standard.AddLabeledCheckbox("NoCathEnabledLabel".Translate() + " ", ref settings.CDNoCath);
            listing_Standard.AddLabeledCheckbox("AggroCalmEnabledLabel".Translate() + " ", ref settings.CDAggroCalmEnabled);
            listing_Standard.AddLabeledCheckbox("OpinionOnlyEnabledLabel".Translate() + " ", ref settings.CDOpnOnly);
            listing_Standard.AddLabeledCheckbox("NonFactionEnabledLabel".Translate() + " ", ref settings.CDNonFaction);
            listing_Standard.AddLabeledCheckbox("TraderCalmEnabledLabel".Translate() + " ", ref settings.CDTraderCalm);
            listing_Standard.AddLabeledCheckbox("AdvancedMenu".Translate() + "  ", ref settings.CDAdvanced);
            if (CDMod.settings.CDAdvanced)
            {
                listing_Standard.AddLabelLine("Formula = diplomacy skill * social multiplier + opinion * opinion multiplier");
                listing_Standard.AddLabeledSlider("SMultLabel".Translate() + " - " + CDMod.settings.CDDipWeight, ref settings.CDDipWeight, 0f, 1f);
                listing_Standard.AddLabeledSlider("OMultLabel".Translate() + " - " + CDMod.settings.CDOpnWeight, ref settings.CDOpnWeight, 0f, 1f);
                listing_Standard.AddLabeledSlider("OOMultLabel".Translate() + " - " + CDMod.settings.CDOOpnWeight, ref settings.CDOOpnWeight, 0f, 1f);
                listing_Standard.AddLabeledSlider("StunWeight".Translate() + " - " + CDMod.settings.CDStunWeight, ref settings.CDStunWeight, 0f, 1f);
                listing_Standard.AddLabeledNumericalTextField("CalmDuration".Translate(), ref CDMod.settings.CDCalmDuration);
                listing_Standard.AddLabeledNumericalTextField("Cooldown".Translate(), ref CDMod.settings.CDCooldown);
                listing_Standard.AddLabeledCheckbox("DebugChanceSetting".Translate() + " ", ref settings.CDDebugChances);
                if (listing_Standard.ButtonText("Default"))
                {
                    CalmUtils.logThis("Reset advanced settings to defaults");
                    CDMod.settings.CDDipWeight = 0.2f;
                    CDMod.settings.CDOpnWeight = 0.0014f;
                    CDMod.settings.CDOOpnWeight = 0.006f;
                    CDMod.settings.CDStunWeight = 0.55f;
                    CDMod.settings.CDCalmDuration = 1250;
                    CDMod.settings.CDDebugChances = false;
                    CDMod.settings.CDCooldown = 15000;
                }
            }
            listing_Standard.End();
            settings.Write();
        }

    }
}
