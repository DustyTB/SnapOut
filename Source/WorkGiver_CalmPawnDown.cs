
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
                if (pawn2.InMentalState) 
                {
                    CalmUtils.logThis(pawn2.NameStringShort + " has met HumanLike and InMentalState conditions.");
                    bool recent = Find.TickManager.TicksGame < pawn2.mindState.lastAssignedInteractTime + 15000;
                    if (CalmUtils.canDo(pawn2) && CalmUtils.IsCapableOf(pawn) && !recent && pawn.CanReserve(pawn2)) //Only on non-aggressive mental state pawns
                    {
                        CalmUtils.logThis("Calming job initiated on " + pawn2.NameStringShort + " by " + pawn.NameStringShort);
                        return new Job(CalmDefOf.CalmDown, pawn2);
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
        #endregion 
    }


}
