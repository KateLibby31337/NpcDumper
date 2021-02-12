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
        public static string MeClassNpcType = ObjectManager.Me.WowClass.ToString() + "Trainer";  // NpcDB Type
        public static string MeClassTrainerNpcType = ObjectManager.Me.WowClass.ToString() + " Trainer"; // Tooltip Scanner Text under Npc Name <>
    }

    public class NpcDumper
    {
        public static void AddVendorItemToBuy()
        {
            var itemToBuy = new wManager.Wow.Class.Buy
            {
                ItemName = "Instant Poison",
                VendorItemClass = wManager.Wow.Class.Npc.NpcVendorItemClass.Consumable,
                GoToVendorIfQuantityLessOrEqual = 5,
                Quantity = 20,
                ScriptCanCondition = "return ObjectManager.Me.Level >20 && ObjectManager.Me.WowClass == WoWClass.Rogue;"
            };
            wManager.wManagerSetting.CurrentSetting.BuyList.Add(itemToBuy);
        }

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

        private static bool UnitHasNpcFlag(WoWUnit Unit,string FlagString)
        {
            return Unit.UnitNPCFlags.HasFlag((UnitNPCFlags)Enum.Parse(typeof(UnitNPCFlags), FlagString));
        }

        private static void ScanForNearbyNpcs()
        {
            PluginSettings Settings = PluginSettings.CurrentSetting;

            Npc.FactionType PlyFactionNpcType;
            List<WoWUnit> NpcList = ObjectManager.GetObjectWoWUnit();
            foreach (WoWUnit NpcUnit in NpcList)
            {
                string VendorItemClass = "None";
                string NpcType = "None";
                string NpcTypeText = GetNpcTypeText(NpcUnit.Guid);
                PlyFactionNpcType = Vars.NeutralNPC;
                string FactionStatus = NpcUnit.Reaction.ToString();
                bool CreateNpcEntry = false;

                // VendorItemClass and NpcType Logic
                if (UnitHasNpcFlag(NpcUnit, "SellsAmmo"))
                {
                    NpcType = "Vendor";
                    VendorItemClass = "Arrow";
                    CreateNpcEntry = true;
                }
                if (UnitHasNpcFlag(NpcUnit, "CanTrain"))
                {
                    if (NpcTypeText == Vars.MeClassTrainerNpcType)
                    {
                        NpcType = Vars.MeClassNpcType;
                        CreateNpcEntry = true;
                    }
                }
                
                // Faction Logic
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

                if (!NpcDB.NpcSimilarExist((ContinentId)Usefuls.ContinentId, NpcUnit.Entry, NpcUnit.Position, PlyFactionNpcType) && CreateNpcEntry)
                {
                    Logging.Write(Plugin.LogName + "Adding Npc " + NpcUnit.Name + " of type " + NpcTypeText + " to NpcDB.");
                    Npc NewNpc = CreateNewNPC(NpcUnit.Name, NpcUnit.Entry, PlyFactionNpcType, (ContinentId)Usefuls.ContinentId, NpcUnit.Position, NpcUnit.IsFlying, (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), NpcType), (Npc.NpcVendorItemClass)Enum.Parse(typeof(Npc.NpcVendorItemClass), VendorItemClass));
                    NpcDB.AddNpc(NewNpc, Settings.SaveNpc, Settings.AddNpcAsProfile);
                }                
            }
        }

        public static void Pulse()
        {
            ScanForNearbyNpcs();
        }
    }
}

