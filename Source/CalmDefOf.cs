using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;


namespace Calm_Down
{
    //References to our defs
    public static class CalmDefOf
    {
        public static JobDef CalmDown = DefDatabase<JobDef>.GetNamed("CDCalmDown");
        public static JobDef SnappingOut = DefDatabase<JobDef>.GetNamed("CDSnappingOut");
        public static InteractionDef CalmDownInt = DefDatabase<InteractionDef>.GetNamed("CDCalmDownInt");
        public static ThoughtDef CDGaveCareThought = DefDatabase<ThoughtDef>.GetNamed("CDHelpedFriend");
    }
}
