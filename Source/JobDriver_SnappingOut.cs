using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace  Calm_Down
{
    class JobDriver_SnappingOut : JobDriver
    {


        //Toil Reservations
        #region toilreservations
        public override bool TryMakePreToilReservations()
        {
            return true;
        }
        #endregion


        //Toil stuffs
        #region toilstuffs
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn rpawn = this.pawn;
            this.TargetThingB = this.pawn;
            Building ownedbed = this.pawn.ownership.OwnedBed;
            if (ownedbed != null)
            {
                yield return Toils_Goto.GotoCell(ownedbed.Position, PathEndMode.OnCell);
            }
            else
            {
                IntVec3 c = RCellFinder.RandomWanderDestFor(rpawn, rpawn.Position, 2f, null, Danger.None);
                yield return Toils_Goto.GotoCell(c, PathEndMode.OnCell);
            }               
            Toil waitonspot = Toils_General.Wait(500);
            
            waitonspot.socialMode = RandomSocialMode.Off;
            yield return waitonspot;
            Toil snappingout = Toils_General.Do(delegate
            {
                rpawn.MentalState.RecoverFromState();
                CalmUtils.logThis("Removed recovery thought from " + rpawn.NameStringShort);
                if (CDMod.settings.CDNoCath) rpawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(rpawn.MentalState.def.moodRecoveryThought);
                CalmUtils.logThis(rpawn.NameStringShort + " snapped out and finished job");
                rpawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            });
            snappingout.socialMode = RandomSocialMode.Off;
            yield return snappingout;
        }
        #endregion


    }
}
