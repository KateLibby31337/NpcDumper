using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace NpcDumper
{
    public static class Extensions
    {
        public static bool TryGetElement(this List<string> ListString, int Index, out string Element)
        {
            if (ListString == null || Index < 0 || Index >= ListString.Count)
            {
                Element = default(string);
                return false;
            }
            Element = ListString[Index];
            return true;
        }

        public static bool HasNpcFlag(this WoWUnit Unit, string FlagString)
        {
            return Unit.UnitNPCFlags.HasFlag((UnitNPCFlags)Enum.Parse(typeof(UnitNPCFlags), FlagString));
        }

        public static string GetTypeText(this WoWUnit Unit, ulong UnitGUID = 0)
        {
            if (UnitGUID == 0) { UnitGUID = Unit.Guid; }
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

        public static string TrimSubString(this string MainString, string SubString)
        {
            return MainString.Remove(MainString.IndexOf(SubString), SubString.Length);
        }

    }
}
