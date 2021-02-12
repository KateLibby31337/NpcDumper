# NpcDumper


This is a small utility plugin that adds nearby trainers of the current character class to the NPCDB as current profile NPC or saved NPC with the correct "Npc.Type". The benefit of this is that the wRobot train state can be used more dynamic/automatically without needing to setup other things first. Additionally plugins/code that want to search the NpcDB for Trainers can now do so and use code like this:

```
    public static void GoToNearestTrainerForClass()
    {    
        Npc.NpcType PlyClassNpcType = (Npc.NpcType)Enum.Parse(typeof(Npc.NpcType), ObjectManager.Me.WowClass.ToString() + "Trainer");
        Npc NearestTrainer = NpcDB.GetNpcNearby(PlyClassNpcType);
        if (NearestTrainer.Type == PlyClassNpcType)
        {
            GoToNpcAndInteract(NearestTrainer.Position, NearestTrainer.Entry, 1);
        }
    }
```
