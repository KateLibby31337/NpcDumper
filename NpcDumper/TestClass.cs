using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;

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
    class TestClass
    {
        public static void TestMethod()
        {
            NpcDB.ListNpc.Clear();
            var MyNPC1 = new Npc
            {
                Name = "Test",
                Entry = 5,
                Faction = Npc.FactionType.Alliance,
                ContinentId = ContinentId.Azeroth,
                Position = new Vector3(1337f, 1337f, 1337f),
                CanFlyTo = false,
                Type = Npc.NpcType.Vendor,
                VendorItemClass = Npc.NpcVendorItemClass.Flask,
            };
            var MyNPC2 = new Npc
            {
                Name = "Test",
                Entry = 5,
                Faction = Npc.FactionType.Alliance,
                ContinentId = ContinentId.Azeroth,
                Position = new Vector3(1337f, 1337f, 1337f),
                CanFlyTo = false,
                Type = Npc.NpcType.Repair,
                VendorItemClass = Npc.NpcVendorItemClass.Book,
            };
            List<Npc> MyNPCList = new List<Npc>();
            MyNPCList.Add(MyNPC1);
            MyNPCList.Add(MyNPC2);
            NpcDB.AddNpcRange(MyNPCList);
        }
    }

    public class BuyArrowsFix
    {
        public static void AddVendorItemToBuy(string name, Npc.NpcVendorItemClass VendorClass, int MinAmount, int BuyAmount, string ScriptCondition = "")
        {
            Buy NewItem = new Buy
            {
                ItemName = name,
                VendorItemClass = VendorClass,
                GoToVendorIfQuantityLessOrEqual = MinAmount,
                Quantity = BuyAmount,
                ScriptCanCondition = ScriptCondition,
            };
            SettingsOverride.wRobotSettings.BuyList.Add(NewItem);

            // Example Data
            //Buy itemToBuy = new Buy
            //{
            //    ItemName = "Rough Arrow",
            //    VendorItemClass = Npc.NpcVendorItemClass.Arrow,
            //    GoToVendorIfQuantityLessOrEqual = 100,
            //    Quantity = 200,
            //    ScriptCanCondition = "return ObjectManager.Me.Level <20 && ObjectManager.Me.WowClass == WoWClass.Hunter;"
            //};

        }
    }

}
