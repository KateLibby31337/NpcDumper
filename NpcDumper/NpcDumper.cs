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

namespace NpcDumper
{
    public class Vars
    {
        public static Npc.FactionType AllianceNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Alliance"); // (cast)Enum.Parse(typeof(cast), "value");
        public static Npc.FactionType HordeNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Horde");
        public static Npc.FactionType NeutralNPC = (Npc.FactionType)Enum.Parse(typeof(Npc.FactionType), "Neutral");
        public static Npc.NpcType MeClassNpcType = (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), ObjectManager.Me.WowClass.ToString() + "Trainer");  // NpcDB Type
        public static string MeClassTrainerNpcType = ObjectManager.Me.WowClass.ToString() + " Trainer"; // Tooltip Scanner Text under Npc Name <>
    }

    public class NpcDumper
    {
        private static string GetNpcTypeText(ulong UnitGUID)
        {
            return Lua.LuaDoString<string>(@"
            local tooltip = _G[""TooltipScanner""] or CreateFrame(""GameTooltip"", ""TooltipScanner"", nil, ""GameTooltipTemplate"");
            tooltip:SetOwner(UIParent,""ANCHOR_NONE"");
            tooltip:ClearLines();
            tooltip:SetHyperlink(""unit:" + UnitGUID.ToString("X") + @""")
            text = _G[""TooltipScannerTextLeft""..2]:GetText();
            tooltip:Hide();
            return text
            ");
        }

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
            };  
        }

        private static void ScanForNearbyTrainers()
        {
            Npc.FactionType PlyFactionNpcType;
            List<WoWUnit> TrainerList = ObjectManager.GetWoWUnitTrainer();
            foreach (WoWUnit Trainer in TrainerList)
            {

                string VendorItem = "Book";
                PlyFactionNpcType = Vars.NeutralNPC;
                string FactionStatus = Trainer.Reaction.ToString();

                if (FactionStatus == "Friendly" || FactionStatus == "Honored" || FactionStatus == "Revered" || FactionStatus == "Exalted")
                {
                    if (ObjectManager.Me.PlayerFaction == "Alliance")

                    {
                        PlyFactionNpcType = Vars.AllianceNPC;
                    }
                    else if (ObjectManager.Me.PlayerFaction == "Horde")
                    {
                        PlyFactionNpcType = Vars.HordeNPC;
                    }
                }
                string NpcTypeText = GetNpcTypeText(Trainer.Guid);
                if (NpcTypeText == Vars.MeClassTrainerNpcType)
                {
                    if (!NpcDB.NpcSimilarExist((ContinentId)Usefuls.ContinentId, Trainer.Entry, Trainer.Position, PlyFactionNpcType))
                    {
                        Logging.Write(Plugin.LogName + "Adding trainer " + Trainer.Name + " of type " + NpcTypeText + " to NpcDB.");
                        Npc ClassTrainer = CreateNewNPC(Trainer.Name, Trainer.Entry, PlyFactionNpcType, (ContinentId)Usefuls.ContinentId, Trainer.Position, Trainer.IsFlying, Vars.MeClassNpcType, (Npc.NpcVendorItemClass)Enum.Parse(typeof(Npc.NpcVendorItemClass), VendorItem));
                        NpcDB.AddNpc(ClassTrainer, PluginSettings.CurrentSetting.SaveNpc, PluginSettings.CurrentSetting.AddNpcAsProfile);
                    }
                }
            }
        }
        private static void ScanForNearbyNpcs()
        {
            var Settings = PluginSettings.CurrentSetting;
            string VendorItem;
            Npc.FactionType PlyFactionNpcType;
            List<WoWUnit> NpcList = ObjectManager.GetObjectWoWUnit();
            foreach (WoWUnit Npc in NpcList)
            {
                if (Npc.UnitNPCFlags.HasFlag((UnitNPCFlags)Enum.Parse(typeof(UnitNPCFlags), "SellsAmmo")))
                {
                    VendorItem = "Arrow";
                }
                else
                {
                    VendorItem = "None";
                }
                PlyFactionNpcType = Vars.NeutralNPC;
                string FactionStatus = Npc.Reaction.ToString();

                if (FactionStatus == "Friendly" || FactionStatus == "Honored" || FactionStatus == "Revered" || FactionStatus == "Exalted")
                {
                    if (ObjectManager.Me.IsAlliance)
                    {
                        PlyFactionNpcType = Vars.AllianceNPC;
                    }
                    else if (ObjectManager.Me.IsHorde)
                    {
                        PlyFactionNpcType = Vars.HordeNPC;
                    }
                }
                string NpcTypeText = GetNpcTypeText(Npc.Guid);
                if (NpcTypeText == Vars.MeClassTrainerNpcType)
                {
                    if (!NpcDB.NpcSimilarExist((ContinentId)Usefuls.ContinentId, Npc.Entry, Npc.Position, PlyFactionNpcType))
                    {
                        Logging.Write(Plugin.LogName + "Adding Npc " + Npc.Name + " of type " + NpcTypeText + " to NpcDB.");
                        Npc NewNpc = CreateNewNPC(Npc.Name, Npc.Entry, PlyFactionNpcType, (ContinentId)Usefuls.ContinentId, Npc.Position, Npc.IsFlying, Vars.MeClassNpcType, (Npc.NpcVendorItemClass)Enum.Parse(typeof(Npc.NpcVendorItemClass), VendorItem));
                        NpcDB.AddNpc(NewNpc, Settings.SaveNpc, Settings.AddNpcAsProfile);
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

