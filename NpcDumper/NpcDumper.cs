// Written by https://github.com/KateLibby31337 https://wrobot.eu/profile/84021-katelibby/

/*
MIT License

Copyright (c) 2021 KateLibby31337

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using robotManager.Helpful;
using System;
using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager;

namespace NpcDumper
{
    public class Vars
    {
        public static Npc.FactionType AllianceNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Alliance"); // (cast)Enum.Parse(typeof(cast), "value");
        public static Npc.FactionType HordeNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Horde");
        public static Npc.FactionType NeutralNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Neutral");
        public static string MeClassNpcType = ObjectManager.Me.WowClass.ToString() + "Trainer";  // NpcDB Type
        public static string MeClassTrainerNpcType = ObjectManager.Me.WowClass.ToString() + " Trainer"; // Tooltip Scanner Text under Npc Name <>
    }

    public class SettingsOverride
    {
        public static wManagerSetting wRobotSettings = wManagerSetting.CurrentSetting;
        public static void TrainerSettingsOverride()
        {
            if (PluginSettings.CurrentSetting.ClearDBonStart)
            {
                Logging.Write($"[{Plugin.Name}]: Clearing NPCDB");
                NpcDB.ListNpc.Clear();
                
            }
            wRobotSettings.NpcMailboxSearchRadius = 0;
            wRobotSettings.NpcScanVendor = false;
            wRobotSettings.NpcScanMailboxes = false;
            wRobotSettings.NpcScanAuctioneer = false;
            wRobotSettings.NpcScanRepair = false;
        }

        public static void PersonalSettingsOverride()
        {
            NpcDB.AcceptOnlyProfileNpc = true;
            wRobotSettings.TrainNewSkills = (ObjectManager.Me.Level % 2 == 0); // Train ever 2x Levels
            wRobotSettings.AcceptOnlyProfileNpc = true;
            wRobotSettings.Selling = true;
            wRobotSettings.RandomJumping = true;
            wRobotSettings.Repair = true;
            wRobotSettings.SellPurple = false;
            wRobotSettings.SellWhite = false;
            wRobotSettings.SellBlue = false;
            wRobotSettings.SellGreen = false;
            wRobotSettings.LatencyMax = 200;
            wRobotSettings.LatencyMin = 100;
        }
    }

    public class NpcDumper
    {

        private static Npc CreateNewNPC(string NpcName, int NpcEntry, Npc.FactionType NpcFaction, ContinentId NpcContinentId, Vector3 NpcPosition, bool NpcCanFlyTo, Npc.NpcType NpcType, Npc.NpcVendorItemClass VendorItem)
        {
            return new Npc
            {
                Name = NpcName,
                Entry = NpcEntry,
                Faction = NpcFaction,
                ContinentId = NpcContinentId,
                Position = NpcPosition,
                CanFlyTo = NpcCanFlyTo,
                Type = NpcType,
                VendorItemClass = VendorItem,
                Save = Settings.SaveNpc,
                CurrentProfileNpc = Settings.AddNpcAsProfile,
            };
        }

        private static PluginSettings Settings = PluginSettings.CurrentSetting;

        private static void ScanForNearbyNpcs()
        {            
            List<WoWUnit> NpcList = ObjectManager.GetObjectWoWUnit();
            foreach (WoWUnit NpcUnit in NpcList)
            {
                // VendorItemClass and NpcType Logic
                string NpcTypeText = NpcUnit.GetTypeText();
                List<string> NpcTypes = new List<string> { };
                List<string> NpcVendorClasses = new List<string> { };                
                if (NpcUnit.HasNpcFlag("MailInfo"))  { NpcTypes.Add("Mailbox"); } // If Mailbox
                if (NpcUnit.HasNpcFlag("CanRepair")) { NpcTypes.Add("Repair"); } // If npc is Repair or Vendor
                if (NpcUnit.HasNpcFlag("SellsAmmo")) { NpcVendorClasses.Add("Arrow"); NpcVendorClasses.Add("Bullet"); }
                if (NpcUnit.HasNpcFlag("CanTrain"))  { if (NpcTypeText == Vars.MeClassTrainerNpcType) { NpcTypes.Add(Vars.MeClassNpcType); } }// If NPC is class Trainer            
                if (NpcUnit.HasNpcFlag("SellsFood")) { NpcVendorClasses.Add("Food"); } // If npc sells food
                if (NpcUnit.HasNpcFlag("CanSell")) { if (!NpcTypes.Contains("Repair")) { NpcTypes.Add("Vendor"); } };
                if (NpcUnit.HasNpcFlag("SellsReagents")) { NpcVendorClasses.Add("Reagent"); }; // Reagent / SellsReagents
                if (NpcTypeText.Contains("Trade Supplies")) { NpcVendorClasses.Add("TradeGoods"); }; // TradeGoods / Npc type <Trade Supplies>
                if (NpcTypeText.Contains("Trainer")) { NpcTypes.Add(NpcTypeText.TrimSubString("Trainer").Trim() + "Trainer");  }

                // Faction Logic
                Npc.FactionType PlyFactionNpcType = Vars.NeutralNPC;
                string FactionStatus = NpcUnit.Reaction.ToString();
                if (FactionStatus == "Friendly" || FactionStatus == "Honored" || FactionStatus == "Revered" || FactionStatus == "Exalted")
                {
                    if (ObjectManager.Me.IsAlliance)
                    { PlyFactionNpcType = Vars.AllianceNPC; }
                    else if (ObjectManager.Me.IsHorde)
                    { PlyFactionNpcType = Vars.HordeNPC; }
                }

                // Create combinations of NpcType/VendorItemClass until both are "None"
                List<Dictionary<string,string>> NpcEntryToAdd = new List<Dictionary<string,string>> {}; // Key:VendorItemClass Value:NpcType  Ex. "Arrow":"Vendor"
                bool CreateNpcEntry = false;
                int EntryCounter = 0;
                while (true)
                {
                    string FinalNpcType = "None";
                    string FinalVendorClass = "None";
                    if (NpcTypes.TryGetElement(EntryCounter, out string varNpcType))
                    { FinalNpcType = varNpcType; }
                    if (NpcVendorClasses.TryGetElement(EntryCounter, out string varVendorClass))
                    { FinalVendorClass = varVendorClass; }
                    if (!(FinalNpcType == "None" && FinalVendorClass == "None"))
                    {
                        var NpcEntry = new Dictionary<string,string> {};
                        NpcEntry.Add(FinalNpcType, FinalVendorClass);
                        NpcEntryToAdd.Add(NpcEntry);
                        CreateNpcEntry = true;
                    }
                    else { break; }
                    EntryCounter++;
                }

                // Foreach combination of NpcType/VendorItemClass Add an NPC entry for that NPC // (Workaround that wRobot only allows one of each per npc)
                if (CreateNpcEntry && !NpcDB.NpcSimilarExist((ContinentId)Usefuls.ContinentId, NpcUnit.Entry, NpcUnit.Position, PlyFactionNpcType))
                {
                    List<Npc> NpcEntryList = new List<Npc> { };
                    foreach (Dictionary<string,string> NpcEntry in NpcEntryToAdd)
                    {
                        foreach (KeyValuePair<string,string> VendorData in NpcEntry)
                        {
                            Logging.Write(Plugin.LogName + $"Adding Npc {NpcUnit.Name} of Type:{VendorData.Key}, ItemClass:{VendorData.Value} to NpcDB.");
                            Npc NewNpc = CreateNewNPC(NpcUnit.Name, NpcUnit.Entry, PlyFactionNpcType, (ContinentId)Usefuls.ContinentId, NpcUnit.Position, NpcUnit.IsFlying, (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), VendorData.Key), (Npc.NpcVendorItemClass)Enum.Parse(typeof(Npc.NpcVendorItemClass), VendorData.Value));
                            NpcEntryList.Add(NewNpc);
                        }
                    }

                    //NpcDB.AddNpcRange(NpcEntryList, Settings.SaveNpc, Settings.AddNpcAsProfile);
                    NpcDB.ListNpc.AddRange(NpcEntryList);
                    //NpcDB.

                    //Npc NewNpc = CreateNewNPC(NpcUnit.Name, NpcUnit.Entry, PlyFactionNpcType, (ContinentId)Usefuls.ContinentId, NpcUnit.Position, NpcUnit.IsFlying, (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), NpcType), (Npc.NpcVendorItemClass)Enum.Parse(typeof(Npc.NpcVendorItemClass), VendorItemClass));
                    //NpcDB.AddNpc(NewNpc, Settings.SaveNpc, Settings.AddNpcAsProfile);
                }                
            }
        }

        public static void Pulse()
        {
            ScanForNearbyNpcs();
        }
    }
}

