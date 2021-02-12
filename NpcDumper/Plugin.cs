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
using System.Threading;

namespace NpcDumper
{
    public class Plugin
    {
        public static readonly string Name = "TrainerScanner";
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

