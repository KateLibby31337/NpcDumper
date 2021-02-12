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
using System.Windows.Forms;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace NpcDumper
{
    public class Vars
    {
        public static Npc.FactionType AllianceNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Alliance");
        public static Npc.FactionType HordeNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Horde");
        public static Npc.FactionType NeutralNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Neutral");
        public static Npc.NpcType MeClassNpcType = (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), ObjectManager.Me.WowClass.ToString() + "Trainer");
        public static string MeClassNpcTypeString = ObjectManager.Me.WowClass.ToString() + " Trainer";
        public static string HomeFolder = Application.StartupPath;
        public static string SettingsFolder = HomeFolder + @"\Settings\";
        public static string SVarEnd = "." + ObjectManager.Me.Name + "." + Usefuls.RealmName + ".svar";
        public static string SVarBegin = "NpcDumper-";
        public static string NpcDB = SVarBegin + "NpcDB" + SVarEnd;
        public static string NpcDBPath = SettingsFolder + NpcDB;
        public static string SessionFile = SVarBegin + "Session" + SVarEnd;
        public static string SessionPath = SettingsFolder + SessionFile;
        public static List<string> SessionUnits;
        public static bool SaveToNpcDB = false;
        public static bool AddToProfileNpc = false;
    }

    public class NpcDumper
    {
        private static Npc.FactionType PlyFactionNpcType;

        private static string GetNpcTypeText(ulong UnitGUID)
        {
            var text = Lua.LuaDoString<string>(@"
            local tooltip = _G[""TooltipScanner""] or CreateFrame(""GameTooltip"", ""TooltipScanner"", nil, ""GameTooltipTemplate"");
            tooltip:SetOwner(UIParent,""ANCHOR_NONE"");
            tooltip:ClearLines();
            tooltip:SetHyperlink(""unit:" + UnitGUID.ToString("X") + @""")
            text = _G[""TooltipScannerTextLeft""..2]:GetText();
            tooltip:Hide();
            return text
            ");
            return text;
        }

        private static Npc CreateNewNPC(string NpcName, int NpcEntry, Npc.FactionType NpcFaction, wManager.Wow.Enums.ContinentId NpcContinentId, Vector3 NpcPosition, bool NpcCanFlyTo, Npc.NpcType NpcType)
        {
            Npc NewNPC = new Npc
            {
                Name = NpcName,
                Entry = NpcEntry,
                Faction = NpcFaction,
                ContinentId = NpcContinentId,
                Position = NpcPosition,
                CanFlyTo = NpcCanFlyTo,
                Type = NpcType,
            };
            return NewNPC;
        }

        private static void ScanForNearbyTrainers()
        {
            List<WoWUnit> TrainerList = ObjectManager.GetWoWUnitTrainer();
            foreach (WoWUnit Trainer in TrainerList)
            {
                PlyFactionNpcType = Vars.NeutralNPC;
                string FactionStatus = Trainer.Reaction.ToString();
                
                if (FactionStatus == "Friendly" || FactionStatus == "Honored" || FactionStatus == "Revered" || FactionStatus == "Exalted")
                {                    
                    if (ObjectManager.Me.Faction == 1)
                    {
                        PlyFactionNpcType = Vars.AllianceNPC;
                    }
                    else
                    {
                        PlyFactionNpcType = Vars.HordeNPC;
                    }
                }
                string NpcTypeText = GetNpcTypeText(Trainer.Guid);
                if (NpcTypeText == Vars.MeClassNpcTypeString)
                {
                    if (!NpcDB.NpcSimilarExist((wManager.Wow.Enums.ContinentId)Usefuls.ContinentId,Trainer.Entry,Trainer.Position,PlyFactionNpcType))
                    {
                        Logging.Write(Plugin.LogName + "Adding trainer " + Trainer.Name + " of type " + NpcTypeText + " to NpcDB.");
                        Npc ClassTrainer = CreateNewNPC(Trainer.Name, Trainer.Entry, PlyFactionNpcType, (wManager.Wow.Enums.ContinentId)Usefuls.ContinentId, Trainer.Position, Trainer.IsFlying, Vars.MeClassNpcType);
                        NpcDB.AddNpc(ClassTrainer,false,true);
                    }                 
                }                    
            }
        }

        public static void Pulse()
        {
            ScanForNearbyTrainers();
        }
    }
}

