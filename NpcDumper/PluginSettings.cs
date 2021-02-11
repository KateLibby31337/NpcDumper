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
        [Description("MS Per Execution")]
        public int TickSpeed { get; set; }

        private PluginSettings()
        {
            TickSpeed = 5000;

            ConfigWinForm(new System.Drawing.Point(600, 300), "["+Plugin.Name+"] "+Translate.Get("Settings"));
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
                Logging.WriteError("[" + Plugin.Name + "] PluginSettings > Save(): " + e);
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
                Logging.WriteError("[" + Plugin.Name + "] PluginSettings > Load(): " + e);
            }
            return false;
        }
    }
}