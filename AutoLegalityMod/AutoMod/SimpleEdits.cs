﻿using System;
using System.Collections.Generic;
using System.Linq;
using PKHeX.Core;
using static PKHeX.Core.LegalityCheckStrings;

namespace AutoLegalityMod
{
    public static class SimpleEdits
    {
        /// <summary>
        /// Set Encryption Constant based on PKM GenNumber
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        public static void SetEncryptionConstant(this PKM pk)
        {
            if (pk.GenNumber > 5 || pk.VC)
            {
                int wIndex = Array.IndexOf(Legal.WurmpleEvolutions, pk.Species);
                uint EC = wIndex < 0 ? Util.Rand32() : PKX.GetWurmpleEC(wIndex / 2);
                if (!(pk.Species == 658 && pk.AltForm == 1))
                    pk.EncryptionConstant = EC;
            }
            else
            {
                pk.EncryptionConstant = pk.PID; // Generations 3 to 5
            }
        }

        /// <summary>
        /// Sets shiny value to whatever boolean is specified
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="isShiny">Shiny value that needs to be set</param>
        public static void SetShinyBoolean(this PKM pk, bool isShiny)
        {
            if (!isShiny)
            {
                pk.SetUnshiny();
            }
            else
            {
                if (pk.GenNumber > 5)
                    pk.SetShiny();
                else if (pk.VC)
                    pk.SetIsShiny(true);
                else
                    pk.SetShinySID();
            }
        }

        /// <summary>
        /// Set a valid Pokeball incase of an incorrect ball issue arising with GeneratePKM
        /// </summary>
        /// <param name="pk"></param>
        public static void SetSpeciesBall(this PKM pk)
        {
            if (!new LegalityAnalysis(pk).Report().Contains(LBallEncMismatch))
                return;
            if (pk.GenNumber == 5 && pk.Met_Location == 75)
                pk.Ball = (int)Ball.Dream;
            else
                pk.Ball = 4;
        }

        public static void ClearRelearnMoves(this PKM Set)
        {
            Set.RelearnMove1 = 0;
            Set.RelearnMove2 = 0;
            Set.RelearnMove3 = 0;
            Set.RelearnMove4 = 0;
        }

        public static void SetMarkings(this PKM pk)
        {
            if (pk.Format >= 7)
            {
                if (pk.IV_HP == 30 || pk.IV_HP == 29) pk.MarkCircle = 2;
                if (pk.IV_ATK == 30 || pk.IV_ATK == 29) pk.MarkTriangle = 2;
                if (pk.IV_DEF == 30 || pk.IV_DEF == 29) pk.MarkSquare = 2;
                if (pk.IV_SPA == 30 || pk.IV_SPA == 29) pk.MarkHeart = 2;
                if (pk.IV_SPD == 30 || pk.IV_SPD == 29) pk.MarkStar = 2;
                if (pk.IV_SPE == 30 || pk.IV_SPE == 29) pk.MarkDiamond = 2;
            }
            if (pk.IV_HP == 31) pk.MarkCircle = 1;
            if (pk.IV_ATK == 31) pk.MarkTriangle = 1;
            if (pk.IV_DEF == 31) pk.MarkSquare = 1;
            if (pk.IV_SPA == 31) pk.MarkHeart = 1;
            if (pk.IV_SPD == 31) pk.MarkStar = 1;
            if (pk.IV_SPE == 31) pk.MarkDiamond = 1;
        }

        public static void ClearHyperTraining(this PKM pk)
        {
            if (pk is IHyperTrain h)
            {
                h.HT_HP = false;
                h.HT_ATK = false;
                h.HT_DEF = false;
                h.HT_SPA = false;
                h.HT_SPD = false;
                h.HT_SPE = false;
            }
        }

        public static void SetHappiness(this PKM pk)
        {
            pk.CurrentFriendship = pk.Moves.Contains(218) ? 0 : 255;
        }

        public static void SetBelugaValues(this PKM pk)
        {
            if (pk is PB7 pb7)
                pb7.ResetCalculatedValues();
        }

        public static void RestoreIVs(this PKM pk, int[] IVs)
        {
            pk.IVs = IVs;
            pk.ClearHyperTraining();
        }

        public static bool NeedsHyperTraining(this PKM pk)
        {
            int flawless = 0;
            int minIVs = 0;
            foreach (int i in pk.IVs)
            {
                if (i == 31) flawless++;
                if (i == 0 || i == 1) minIVs++; //ignore IV value = 0/1 for intentional IV values (1 for hidden power cases)
            }
            return flawless + minIVs != 6;
        }

        public static void HyperTrain(this PKM pk)
        {
            if (!(pk is IHyperTrain h) || !NeedsHyperTraining(pk))
                return;

            pk.CurrentLevel = 100; // Set level for HT before doing HT

            h.HT_HP = (pk.IV_HP != 0 && pk.IV_HP != 1 && pk.IV_HP != 31);
            h.HT_ATK = (pk.IV_ATK != 0 && pk.IV_ATK != 1 && pk.IV_ATK != 31);
            h.HT_DEF = (pk.IV_DEF != 0 && pk.IV_DEF != 1 && pk.IV_DEF != 31);
            h.HT_SPA = (pk.IV_SPA != 0 && pk.IV_SPA != 1 && pk.IV_SPA != 31);
            h.HT_SPD = (pk.IV_SPD != 0 && pk.IV_SPD != 1 && pk.IV_SPD != 31);
            h.HT_SPE = (pk.IV_SPE != 0 && pk.IV_SPE != 1 && pk.IV_SPE != 31);
        }

        public static void ClearHyperTrainedPerfectIVs(this PKM pk)
        {
            if (!(pk is IHyperTrain h))
                return;
            if (pk.IV_HP == 31) h.HT_HP = false;
            if (pk.IV_ATK == 31) h.HT_ATK = false;
            if (pk.IV_DEF == 31) h.HT_DEF = false;
            if (pk.IV_SPA == 31) h.HT_SPA = false;
            if (pk.IV_SPD == 31) h.HT_SPD = false;
            if (pk.IV_SPE == 31) h.HT_SPE = false;
        }

        public static void SetSuggestedMemories(this PKM pk)
        {
            switch (pk)
            {
                case PK7 pk7:
                    if (!pk.IsUntraded)
                        pk7.TradeMemory(true);
                    pk7.FixMemories();
                    break;
                case PK6 pk6:
                    if (!pk.IsUntraded)
                        pk6.TradeMemory(true);
                    pk6.FixMemories();
                    break;
            }
        }

        public static void ClearOTMemory(this PKM pk)
        {
            pk.OT_Memory = 0;
            pk.OT_TextVar = 0;
            pk.OT_Intensity = 0;
            pk.OT_Feeling = 0;
        }

        /// <summary>
        /// Set TID, SID and OT
        /// </summary>
        /// <param name="pk">PKM to set trainer data to</param>
        /// <param name="trainer">Trainer data</param>
        /// <param name="APILegalized">Was the <see cref="pk"/> legalized by the API</param>
        public static void SetTrainerData(this PKM pk, SimpleTrainerInfo trainer, bool APILegalized = false)
        {
            if (APILegalized)
            {
                if ((pk.TID == 12345 && pk.OT_Name == "PKHeX") || (pk.TID == 34567 && pk.SID == 0 && pk.OT_Name == "TCD"))
                {
                    bool Shiny = pk.IsShiny;
                    pk.TID = trainer.TID;
                    pk.SID = trainer.SID;
                    pk.OT_Name = trainer.OT;
                    pk.OT_Gender = trainer.Gender;
                    pk.SetShinyBoolean(Shiny);
                }
                return;
            }
            pk.TID = trainer.TID;
            pk.SID = trainer.SID;
            pk.OT_Name = trainer.OT;
        }

        /// <summary>
        /// Set Trainer data (TID, SID, OT) for a given PKM
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        public static void SetTrainerDataAndMemories(this PKM pk)
        {
            if (pk.WasEvent || pk.WasIngameTrade)
                return;

            // Hardcoded a generic one for now, trainerdata.json implementation here later
            pk.CurrentHandler = 1;
            pk.HT_Name = "ARCH";
            pk.HT_Gender = 0; // Male for Colo/XD Cases
            pk.TID = 34567;
            pk.SID = 0;
            pk.OT_Name = "TCD";
            pk.SetSuggestedMemories();
        }

        /// <summary>
        /// Set trainer data for a legal PKM
        /// </summary>
        /// <param name="pk">Legal PKM for setting the data</param>
        /// <param name="trainer"></param>
        /// <returns>PKM with the necessary values modified to reflect trainerdata changes</returns>
        public static void SetAllTrainerData(this PKM pk, SimpleTrainerInfo trainer)
        {
            pk.SetTrainerData(trainer, true);
            pk.ConsoleRegion = trainer.ConsoleRegion;
            pk.Country = trainer.Country;
            pk.Region = trainer.SubRegion;
        }

        /// <summary>
        /// Fix invalid and missing ribbons. (V600 and V601)
        /// </summary>
        /// <param name="pk">PKM whose ribbons need to be fixed</param>
        public static void SetSuggestedRibbons(this PKM pk)
        {
            string Report = new LegalityAnalysis(pk).Report();
            if (Report.Contains(string.Format(LRibbonFMissing_0, "")))
            {
                var val = string.Format(LRibbonFMissing_0, "");
                var ribbonList = GetRequiredRibbons(Report, val);
                var missingRibbons = GetRibbonsRequired(pk, ribbonList);
                SetRibbonValues(pk, missingRibbons, 0, true);
            }
            if (Report.Contains(string.Format(LRibbonFInvalid_0, "")))
            {
                var val = string.Format(LRibbonFInvalid_0, "");
                string[] ribbonList = GetRequiredRibbons(Report, val);
                var invalidRibbons = GetRibbonsRequired(pk, ribbonList);
                SetRibbonValues(pk, invalidRibbons, 0, false);
            }
        }

        public static void SetSuggestedRelearnMoves(this PKM pk)
        {
            if (pk.Format < 6)
                return;
            pk.ClearRelearnMoves();
            var Legality = new LegalityAnalysis(pk);

            int[] m = Legality.GetSuggestedRelearn();
            if (m.All(z => z == 0))
            {
                if (!pk.WasEgg && !pk.WasEvent && !pk.WasEventEgg && !pk.WasLink)
                {
                    if (pk.Version != (int)GameVersion.CXD)
                    {
                        var encounter = Legality.GetSuggestedMetInfo();
                        if (encounter != null)
                            m = encounter.Relearn;
                    }
                }
            }

            if (pk.RelearnMoves.SequenceEqual(m))
                return;
            if (m.Length > 3)
                pk.RelearnMoves = m;
        }

        public static void SetSuggestedMetLocation(this PKM pk)
        {
            var Legality = new LegalityAnalysis(pk);

            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || (pk.Format >= 3 && encounter.Location < 0))
                return;

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(pk, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (pk.CurrentLevel >= minlvl && pk.Met_Level == level && pk.Met_Location == location)
                return;
            if (minlvl < level)
                level = minlvl;
            pk.Met_Location = location;
            pk.Met_Level = level;
        }

        private static IEnumerable<string> GetRibbonsRequired(PKM pk, string[] ribbonList)
        {
            foreach (var RibbonName in GetRibbonNames(pk))
            {
                string v = RibbonStrings.GetName(RibbonName).Replace("Ribbon", "");
                if (ribbonList.Contains(v))
                    yield return RibbonName;
            }
        }

        private static string[] GetRequiredRibbons(string Report, string val)
        {
            return Report.Split(new[] { val }, StringSplitOptions.None)[1].Split(new[] { "\r\n" }, StringSplitOptions.None)[0].Split(new[] { ", " }, StringSplitOptions.None);
        }

        private static void SetRibbonValues(this PKM pk, IEnumerable<string> ribNames, int vRib, bool bRib)
        {
            foreach (string rName in ribNames)
            {
                bool intRib = rName == nameof(PK6.RibbonCountMemoryBattle) || rName == nameof(PK6.RibbonCountMemoryContest);
                ReflectUtil.SetValue(pk, rName, intRib ? (object)vRib : bRib);
            }
        }

        public static void ClearAllRibbons(this PKM pk) => pk.SetRibbonValues(GetRibbonNames(pk), 0, false);
        private static IEnumerable<string> GetRibbonNames(PKM pk) => ReflectUtil.GetPropertiesStartWithPrefix(pk.GetType(), "Ribbon").Distinct();
    }
}