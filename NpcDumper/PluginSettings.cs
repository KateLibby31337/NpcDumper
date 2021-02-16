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

using robotManager;
using robotManager.Helpful;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

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

        [Setting]
        [DefaultValue(false)]
        [Category("Settings")]
        [DisplayName("NPCDB Scan Override")]
        [Description("True to disable wRobot NPC scanning.")]
        public bool DisableWrobotScanning { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Settings")]
        [DisplayName("Clear DB on start:")]
        [Description("True to clear the NPCDB every wRobot play.")]
        public bool ClearDBonStart { get; set; }



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

