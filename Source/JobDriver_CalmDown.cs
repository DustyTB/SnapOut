using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Calm_Down

{
    public class JobDriver_CalmDown : JobDriver
    {


        //Our variables b here
        #region vars
        //Formula weight variables. Thanks Mehni and XeoNovaDan, you guys are epic!
        private const TargetIndex pieceofshit = TargetIndex.A; 
        Job recoverjob = new Job(CalmDefOf.SnappingOut);
        Job gotosafetyjob = new Job(CalmDefOf.GoToSafety);
        #endregion


        //Our main toil that does the calculations and stuff like that
        #region MainToil
        protected Toil CalmDown(TargetIndex CTrg, int dur)
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var toil = new Toil
            {
                initAction = () =>
                {
                    pieceofs.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    this.TargetThingB = this.pawn; //Defining our initiator pawn
                    float rand = UnityEngine.Random.Range(0f, 0.70f);
                    pawn.interactions.TryInteractWith(pieceofs, CalmDefOf.CalmDownInt);
                    float num = CalmUtils.doFormula(pawn, pieceofs);
                    CalmUtils.logThis("Calm chance was " + num.ToString() + " versus random of " + rand.ToString());
                    if (rand > num)
                    {
                        #region failcondition
                        if (CDMod.settings.CDmessagesEnabled)
                        {
                            MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, CalmUtils.GetCalmingMessage, Color.red, 3.85f);
                        }
                        if (pieceofs.InAggroMentalState)
                        {
                            pieceofs.TryStartAttack(pawn);
                            CalmUtils.doStatusMessage(3, pawn, pieceofs);
                            pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;                            
                            return;

                        }

                        CalmUtils.doStatusMessage(2, pawn, pieceofs);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        Room bedroom = pieceofs.ownership.OwnedRoom;
                        if (bedroom != null)
                        {
                            
                            int srand = Random.Range(0, 100);
                            CalmUtils.logThis(pieceofs.NameStringShort + " has a bedroom. Chance to get job is.. " + srand);
                            if (srand <= 65) //65% chance
                            {
                                CalmUtils.logThis(pieceofs.NameStringShort + " received gotosafety job!");
                                pieceofs.jobs.EndCurrentJob(JobCondition.InterruptForced);
                                gotosafetyjob.playerForced = true;
                                gotosafetyjob.locomotionUrgency = LocomotionUrgency.Jog;
                                pieceofs.jobs.StartJob(gotosafetyjob);
                            }
                            else
                            {
                                CalmUtils.logThis(pieceofs.NameStringShort + " didnt receive gotosafety job!");
                            }
                        }
                            return;
                        #endregion
                    }
                    #region successcondition
                    if (CDMod.settings.CDmessagesEnabled)
                    {
                        MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, CalmUtils.GetCalmingMessage, Color.green, 3.85f);
                    }
                    pawn.needs.mood.thoughts.memories.TryGainMemory(CalmDefOf.CDGaveCareThought, null);
                    pawn.skills.Learn(SkillDefOf.Social, Rand.Range(50,125));
                    CalmUtils.doStatusMessage(1, pawn, pieceofs);
                    if (pieceofs.InAggroMentalState) { pieceofs.MentalState.RecoverFromState(); pieceofs.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.WanderSad); }
                    pieceofs.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    recoverjob.playerForced = true;               
                    pieceofs.jobs.StartJob(recoverjob);
                    pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                    #endregion
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Instant,
                defaultDuration = CDMod.settings.CDCalmDuration
            };
            return toil.WithProgressBarToilDelay(TargetIndex.B);
        }
        #endregion


        //Toil reservation
        #region toilreservation
        public override bool TryMakePreToilReservations()
        {
            return this.pawn.Reserve(this.job.GetTarget(pieceofshit), this.job, 1, -1, null);         
        }
        #endregion


        //Toil stuffs
        #region toilstuffs
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var cdown = CalmDown(pieceofshit, CDMod.settings.CDCalmDuration);
            

            this.FailOnDowned(pieceofshit);
            this.FailOnDespawnedOrNull(pieceofshit);
            this.FailOnNotAwake(pieceofshit);

            
            yield return Toils_Goto.GotoThing(pieceofshit, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(pieceofshit);
            yield return Toils_General.Do(delegate
            {
                             
                
                pieceofs.rotationTracker.FaceCell(pawn.PositionHeld);
                if (pieceofs.InAggroMentalState)
                {
                    
                    float randa = UnityEngine.Random.Range(0f, 0.85f);
                    float numba = pawn.GetStatValue(StatDefOf.DiplomacyPower, true);
                    numba = numba * CDMod.settings.CDStunWeight;
                    CalmUtils.logThis("Aggressive stun chance was " + numba.ToString() + " versus random of " + randa.ToString());
                    if (randa > numba)
                    {
                        CalmUtils.doStatusMessage(3, pawn, pieceofs);    
                        pieceofs.TryStartAttack(pawn);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        EndJobWith(JobCondition.Incompletable);
                    }
                    if (numba > randa) pieceofs.stances.stunner.StunFor(CDMod.settings.CDCalmDuration);       
               }   
            });
            yield return Toils_Interpersonal.GotoInteractablePosition(pieceofshit);
            yield return Toils_General.WaitWith(pieceofshit, CDMod.settings.CDCalmDuration, true, true);
            yield return cdown;
            yield return Toils_Interpersonal.SetLastInteractTime(pieceofshit);


        }
        #endregion


    }
}
