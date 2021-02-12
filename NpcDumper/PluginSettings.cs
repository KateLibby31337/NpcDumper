using wManager.Wow.Helpers;
using robotManager.Helpful;
using System;
using System.IO;
using wManager.Wow.ObjectManager;
using robotManager;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;

namespace NpcDumper
{
    [Serializable]
    public class PluginSettings : Settings
    {

        [Setting]
        [DefaultValue(5000)]
        [Category("Settings")]
        [DisplayName("TickSpeed")]
        [Description("MS Between NPC Scanning. (Set higher if experiencing lag from this plugin.)")]
        public int TickSpeed { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Settings")]
        [DisplayName("SaveNpc")]
        [Description("If set to true save to npcdb file for future bot usage.")]
        public bool SaveNpc { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Settings")]
        [DisplayName("AddNpcAsProfile")]
        [Description("If set to true it is profile NPC (used by option AcceptOnlyProfileNpc).")]
        public bool AddNpcAsProfile { get; set; }

        private PluginSettings()
        {
            TickSpeed = 5000;
            SaveNpc = false;
            AddNpcAsProfile = true;

            ConfigWinForm(new System.Drawing.Point(600, 300), Plugin.Name + Translate.Get("Settings"));
        }

        public static PluginSettings CurrentSetting { get; set; }

        public bool Save()
        {
            try
            {
                return Save(AdviserFilePathAndName(Plugin.Name, ObjectManager.Me.Name + "." + Usefuls.RealmName));
            }
            catch (Exception e)
            {
                Logging.WriteError(Plugin.LogName + "PluginSettings > Save(): " + e);
                return false;
            }
        }

        public static bool Load()
        {
            try
            {
                if (File.Exists(AdviserFilePathAndName(Plugin.Name, ObjectManager.Me.Name + "." + Usefuls.RealmName)))
                {
                    CurrentSetting =
                        Load<PluginSettings>(AdviserFilePathAndName(Plugin.Name, ObjectManager.Me.Name + "." + Usefuls.RealmName));
                    return true;
                }
                CurrentSetting = new PluginSettings();
            }
            catch (Exception e)
            {
                Logging.WriteError(Plugin.LogName + "PluginSettings > Load(): " + e);
            }
            return false;
        }
    }
}