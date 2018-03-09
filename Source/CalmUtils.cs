using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Calm_Down
{
    class CalmUtils
    {
        //In an attempt to de-clutterify and remove excess lines
        private static readonly string FCalm = "FailCalm".Translate();
        private static readonly string SCalm = "SuccessCalm".Translate();
        private static readonly string AFCalm = "AggroFailCalm".Translate();

        public static bool canDo(Pawn subjectee)
        {
            bool PrisonerDo = false; bool TraderDo = false; bool StateTypeDo = false;

            //Aggro check
            if (subjectee.InAggroMentalState == CDMod.settings.CDAggroCalmEnabled) StateTypeDo = true;
            if (!subjectee.InAggroMentalState) StateTypeDo = true;

            //Prisoner check
            if (subjectee.guest.IsPrisoner == CDMod.settings.CDNonFaction) { PrisonerDo = true; TraderDo = true; }

            //Trader check
            if (!subjectee.Faction.IsPlayer && !subjectee.Faction.RelationWith(Faction.OfPlayer).hostile)
            {
                if (CDMod.settings.CDTraderCalm) { TraderDo = true; PrisonerDo = true; }
            }

            //Colonist check
            if (subjectee.Faction == Faction.OfPlayer) { PrisonerDo = true; TraderDo = true; }

            logThis("Can do check for " + subjectee.NameStringShort + " returns.. " + PrisonerDo + TraderDo + StateTypeDo);
            if (PrisonerDo && TraderDo && StateTypeDo) return true;
            return false;
        }

        public static bool IsCapableOf(Pawn doer)
        {
            if (doer.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                logThis("Capability check for " + doer.NameStringShort + " returns true");
                return true;
            }
            logThis("Capability check for " + doer.NameStringShort + " returns false");
            return false;
        }

        public static void logThis(string message)
        {
            if (CDMod.settings.CDDebugChances)
            {
                Log.Message("[SnapOut] " + message);
            }
        }

        public static float doFormula(Pawn doer, Pawn subjectee)
        {
            float num = doer.GetStatValue(StatDefOf.DiplomacyPower, true);
            int opinion = subjectee.relations.OpinionOf(doer);
            num = num * CDMod.settings.CDDipWeight + (float)opinion * CDMod.settings.CDOpnWeight; //Formula
            if (CDMod.settings.CDOpnOnly)
            {
                num = (float)opinion * CDMod.settings.CDOOpnWeight;
            }
            num = Mathf.Clamp01(num);
            return num;
        }

        public static void doStatusMessage(int type, Pawn doer, Pawn subjectee)
        {
            switch (type)
            {
                case 1: //Success
                    Messages.Message(string.Format(SCalm, new object[]
                    {
                                    doer.NameStringShort,
                                    subjectee.NameStringShort,
                    }), MessageTypeDefOf.TaskCompletion);
                    break;
                case 2: //Failure
                    Messages.Message(string.Format(FCalm, new object[]
                                {
                                    doer.NameStringShort,
                                    subjectee.NameStringShort,
                                }), MessageTypeDefOf.TaskCompletion);
                    break;
                case 3: //Critical Failure
                    Messages.Message(string.Format(AFCalm, new object[]
                                    {
                                    doer.NameStringShort,
                                    subjectee.NameStringShort,
                                    }), MessageTypeDefOf.TaskCompletion);
                    break;
            }
        }
    }
}
