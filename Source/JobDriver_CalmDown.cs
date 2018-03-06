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
        protected bool messageEnabled = CDMod.settings.CDmessagesEnabled;
        protected bool opnOnlyEnabled = CDMod.settings.CDOpnOnly;
        string[] calmingmessages = new string[]
                {
                    "It will be all fine.",
                    "Don't worry, Ive got your back.",
                    "Just hold on with me, Ill be with you all the way.",
                    "Its just a table, we will be fine.",
                    "We will be with you.",
                    "I'm here for you, tell me whats bothering you - I'm listening.",
                    "We can solve this together.",
                    "Take deep breaths, you can do it. Relax..",
                    "I know what you're going through, I'd really like to help.. ",
                    "What are you worried about?",
                    "Not this again, come on..",
                    "Tell me your concerns.",
                    "This too will pass.",
                    "It's like a storm, It's just temporary.",
                    "Lets solve this together.",
                    "You're safe, I'm here for you.",
                    "I'm sorry you're going through that.",
                    "You're not on your own, we are all here for you.",
                    "It's not your fault.",
                    "You can vent to me, I'll listen.",
                    "We have a great recreation room, how about we play some chess together?"
                };
        //Translations for the success/failure messages
        private static readonly string FCalm = "FailCalm".Translate();
        private static readonly string SCalm = "SuccessCalm".Translate();
        private static readonly string AFCalm = "AggroFailCalm".Translate();
        private const int CalmDuration = 1250; 
        //Formula weight variables. Thanks Mehni and XeoNovaDan, you guys are epic!
        private const float DiplomacyWeight = 0.2f; 
        private const float OpinionWeight = 0.0014f;
        private const float OOpinionWeight = 0.006f;
        private const TargetIndex pieceofshit = TargetIndex.A; 
        Job recoverjob = new Job(CalmDefOf.SnappingOut);
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
                    float num = pawn.GetStatValue(StatDefOf.DiplomacyPower, true); 
                    int opinion = pieceofs.relations.OpinionOf(pawn); 
                    num = num * DiplomacyWeight + (float)opinion * OpinionWeight; //Formula
                    if (opnOnlyEnabled)
                    {
                        num = (float)opinion * OOpinionWeight;
                    }
                    num = Mathf.Clamp01(num); 
                    string debugNum = num.ToString();
                    string debugRand = rand.ToString();
                    Log.Message("chance " + debugNum + " |rand " + debugRand);
                    if (rand > num) 
                    {
                        #region failcondition
                        if (messageEnabled) 
                        {
                            MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, calmingmessages.RandomElement<string>(), Color.red, 3.85f);
                        }
                        if (pieceofs.InAggroMentalState) 
                        {
                            pieceofs.TryStartAttack(pawn);
                            Messages.Message(string.Format(JobDriver_CalmDown.AFCalm, new object[]
                                    {
                                    pawn.NameStringShort,
                                    pieceofs.NameStringShort,
                                    }), MessageTypeDefOf.TaskCompletion);
                            pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame; 
                            return;

                        }
                        
                        Messages.Message(string.Format(JobDriver_CalmDown.FCalm, new object[]
                                {
                                    pawn.NameStringShort,
                                    pieceofs.NameStringShort,
                                }), MessageTypeDefOf.TaskCompletion);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame; 
                        return;
                        #endregion
                    }
                        #region successcondition
                    if (messageEnabled) 
                    {
                        MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, calmingmessages.RandomElement<string>(), Color.green, 3.85f);
                    }
                    pawn.needs.mood.thoughts.memories.TryGainMemory(CalmDefOf.CDGaveCareThought, null);
                    Messages.Message(string.Format(JobDriver_CalmDown.SCalm, new object[]
                    {
                                    pawn.NameStringShort,
                                    pieceofs.NameStringShort,
                    }), MessageTypeDefOf.TaskCompletion);
                    pieceofs.jobs.StartJob(recoverjob);
                    pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                    #endregion
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Instant,
                defaultDuration = CalmDuration
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
            var cdown = CalmDown(pieceofshit, CalmDuration);
            
            this.FailOnDowned(pieceofshit);
            this.FailOnDespawnedOrNull(pieceofshit);
            this.FailOnNotAwake(pieceofshit);


            yield return Toils_Goto.GotoThing(pieceofshit, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(pieceofshit);
            yield return Toils_General.WaitWith(pieceofshit, CalmDuration, true, true);
            yield return cdown;
            yield return Toils_Interpersonal.SetLastInteractTime(pieceofshit);


        }
        #endregion


    }
}
