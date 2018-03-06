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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.CDmessagesEnabled, "CDMessagesEnabled", true);
            Scribe_Values.Look<bool>(ref this.CDAggroCalmEnabled, "CDAggroCalmEnabled", false);
            Scribe_Values.Look<bool>(ref this.CDOpnOnly, "CDOpnOnly", false);
            Scribe_Values.Look<bool>(ref this.CDNonFaction, "CDNonFaction", true);
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
            listing_Standard.AddLabeledCheckbox("MessagesEnabledLabel".Translate() +": ", ref settings.CDmessagesEnabled);
            listing_Standard.AddLabeledCheckbox("AggroCalmEnabledLabel".Translate() + ": ", ref settings.CDAggroCalmEnabled);
            listing_Standard.AddLabeledCheckbox("OpinionOnlyEnabledLabel".Translate() + ": ", ref settings.CDOpnOnly);
            listing_Standard.AddLabeledCheckbox("NonFactionEnabledLabel".Translate() + ": ", ref settings.CDNonFaction);
            listing_Standard.End();
            settings.Write();
        }

    }
}
