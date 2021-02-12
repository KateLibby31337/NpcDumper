using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NpcDumper
{
    public class Plugin
    {
        public static readonly string Name = "NpcDumper";
        public static readonly string LogName = "[" + Plugin.Name + "]: ";
        public static bool IsRunning;

        public static void Start()
        {
            IsRunning = true;
            Plugin.Tick();
        }

        public static void Stop()
        {
            IsRunning = false;            
        }

        private static void Tick()
        {
            do 
            {
                try
                {
                    if (!robotManager.Products.Products.InPause)
                    {
                        Plugin.Execute();
                    }
                }
                catch (Exception error)
                {
                    Logging.WriteError(Plugin.LogName + "Plugin.Tick(): " + (object)error, true);
                }
                Thread.Sleep(PluginSettings.CurrentSetting.TickSpeed);

            } while (IsRunning);
        }

        private static void Execute()
        {
            NpcDumper.Pulse();
        }

    }
}
