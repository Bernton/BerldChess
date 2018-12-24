using System;
using System.Collections.Generic;

namespace BerldChess.Model
{
    [Serializable]
    public class EngineList
    {
        public bool SettingAvailable => Settings.Count > 0;
        public int SelectedIndex { get; set; }
        public int SelectedOpponentIndex { get; set; }
        public List<EngineSetting> Settings { get; set; }
        public List<string> LastPaths { get; set; } = new List<string>();

        public EngineSetting SelectedSetting
        {
            get
            {
                if (SelectedIndex == -1 || Settings.Count == 0)
                    return null;

                return Settings[SelectedIndex];
            }
        }

        public EngineSetting SelectedOpponentSetting
        {
            get
            {
                if (SelectedOpponentIndex == -1 || Settings.Count == 0)
                    return null;

                return Settings[SelectedOpponentIndex];
            }
        }

        public EngineList()
        {
            Settings = new List<EngineSetting>();
            SelectedIndex = -1;
        }

        public void TryAddPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || LastPaths.Contains(path))
                return;

            LastPaths.Add(path);

            if (LastPaths.Count > 8)
            {
                LastPaths.RemoveRange(8, LastPaths.Count - 8);
            }
        }
    }
}
