﻿using Reclaimer.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExamplePlugin
{
    public class ExamplePlugin : Plugin
    {
        public override string Name => "Example Plugin";

        private PluginSettings Settings { get; set; }

        public override void Initialise()
        {
            Settings = LoadSettings<PluginSettings>();
            LogOutput("Loaded example settings");
        }

        public override IEnumerable<PluginMenuItem> MenuItems
        {
            get
            {
                yield return new PluginMenuItem("Key1", "Example\\Item 1");
                yield return new PluginMenuItem("Key2", "Example\\Item 2");
            }
        }

        public override void OnMenuItemClick(string key)
        {
            if (key == "Key1")
                MessageBox.Show("Item 1 Click");
            else if (key == "Key2")
                MessageBox.Show("Item 2 Click");
        }

        public override bool CanOpenFile(object file, string fileTypeKey)
        {
            return true;
        }

        public override void OpenFile(OpenFileArgs args)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open {args.FileName}");
                LogError("Not Implemented!", ex);
            }
        }

        public override void Suspend()
        {
            SaveSettings(Settings);
            LogOutput("Saved example settings");
        }

        private class PluginSettings
        {
            public string ExampleSetting { get; set; }

            public PluginSettings()
            {
                ExampleSetting = "<insert value here>";
            }
        }
    }
}
