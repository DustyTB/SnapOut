
using RimWorld;
using Verse;
using Verse.AI;

namespace Calm_Down
{
    class WorkGiver_CalmPawnDown : WorkGiver_Warden_Chat
    {

        //Lone boolean 
        #region lonebool
        bool aggroCalmEnabled = CDMod.settings.CDAggroCalmEnabled;
        #endregion


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
            bool shouldDo = false;
            if (pawn.InAggroMentalState && aggroCalmEnabled) { shouldDo = true; };
            if (!pawn.InAggroMentalState) { shouldDo = true; };
            
            bool recent = Find.TickManager.TicksGame < pawn2.mindState.lastAssignedInteractTime + 15000;
            if (pawn2.InMentalState && shouldDo && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && !recent && pawn2.RaceProps.Humanlike && pawn.CanReserve(pawn2)) //Only on mental state pawns
            {
                    return new Job(CalmDefOf.CalmDown, pawn2);
            }
            return null;
        }
        #endregion 
    }


}
