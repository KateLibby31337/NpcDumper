// Written by https://github.com/KateLibby31337 https://wrobot.eu/profile/84021-katelibby/


// Local
using NpcDumper;
// WRobot
using robotManager.Helpful;
using wManager.Plugin;

public class Main : IPlugin
{
    public void Initialize()
    {
        PluginSettings.Load();
        Plugin.Start();
    }

    public void Dispose()
    {
        Plugin.Stop();
        PluginSettings.CurrentSetting.Save();
    }

    public void Settings()
    {
        PluginSettings.Load();
        PluginSettings.CurrentSetting.ToForm();
        PluginSettings.CurrentSetting.Save();
    }
}