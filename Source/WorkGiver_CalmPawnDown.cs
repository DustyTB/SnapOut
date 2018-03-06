
using RimWorld;
using Verse;
using Verse.AI;

namespace Calm_Down
{
    class WorkGiver_CalmPawnDown : WorkGiver_Warden_Chat
    {

        //ShouldSkip Thingy
        #region shouldskip
        public override bool ShouldSkip(Pawn pawn)
        {
            return base.ShouldSkip(pawn) && (pawn.Faction != Faction.OfPlayer) && (!pawn.RaceProps.Humanlike);
        }
        #endregion


        //Main thing
        #region jobonthing
        public override Job JobOnThing(Pawn pawn, Thing thang, bool forced = true)
        {
            Pawn pawn2 = (Pawn)thang;
            if (pawn2.RaceProps.Humanlike)
            {
                bool ShouldDoFac = (pawn2.IsPrisonerOfColony == CDMod.settings.CDNonFaction);
                if (pawn2.Faction == Faction.OfPlayer) ShouldDoFac = true;

                bool ShouldDoAgg = (pawn2.InAggroMentalState == CDMod.settings.CDAggroCalmEnabled);
                if (!pawn2.InAggroMentalState) ShouldDoAgg = true;


                bool recent = Find.TickManager.TicksGame < pawn2.mindState.lastAssignedInteractTime + 15000;
                if (pawn2.InMentalState && ShouldDoFac && ShouldDoAgg && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && !recent && pawn2.RaceProps.Humanlike && pawn.CanReserve(pawn2)) //Only on non-aggressive mental state pawns
                {
                    return new Job(CalmDefOf.CalmDown, pawn2);
                }
                return null;
            }
            return null;
        }
        #endregion 
    }


}
