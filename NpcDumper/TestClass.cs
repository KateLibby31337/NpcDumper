using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;

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
