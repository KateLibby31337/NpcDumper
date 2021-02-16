using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

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

namespace NpcDumper
{
    public static class Extensions
    {
        public static bool TryGetElement(this List<string> ListString, int Index, out string Element) // Extends lists to add a more robust way of blind checking indexes
        {
            if (ListString == null || Index < 0 || Index >= ListString.Count)
            {
                Element = default(string);
                return false;
            }
            Element = ListString[Index];
            return true;
        }

        public static bool HasNpcFlag(this WoWUnit Unit, string FlagString) // Extends WoWUnit to allow use of strings to check NpcUnit flags 
        {
            return Unit.UnitNPCFlags.HasFlag((UnitNPCFlags)Enum.Parse(typeof(UnitNPCFlags), FlagString));
        }

        public static bool HasUnitFlag(this WoWUnit Unit, string FlagString) // Extends WoWUnit to allow use of strings to check Unit flags
        {
            return Unit.UnitFlags.HasFlag((UnitFlags)Enum.Parse(typeof(UnitFlags), FlagString));
        }

        public static string GetTypeText(this WoWUnit Unit) // Extends WoWUnit to scan for the text under npc name <Blacksmithing Trainer>
        {
            ulong UnitGUID = Unit.Guid;
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

        public static string TrimSubString(this string MainString, string SubString) // Extends string to easily remove a sub-string from a string
        {
            return MainString.Remove(MainString.IndexOf(SubString), SubString.Length);
        }

    }
}
