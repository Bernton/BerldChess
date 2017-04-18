using System;
using System.Collections.Generic;

namespace BerldChess.Model
{
    [Serializable]
    public class EngineList
    {
        public List<EngineSetting> Settings { get; set; }
        public int SelectedIndex { get; set; }

        public List<string> LastPaths { get; set; } = new List<string>();

        public bool SettingAvailable
        {
            get
            {
                return Settings.Count > 0;
            }
        }


        public EngineSetting SelectedSetting
        {
            get
            {
                if(SelectedIndex == -1)
                {
                    return null;
                }

                return Settings[SelectedIndex];
            }
        }

        public EngineList()
        {
            Settings = new List<EngineSetting>();
            SelectedIndex = -1;
        }

        public void AddLastPath(string path)
        {
            LastPaths.Add(path);

            if(LastPaths.Count > 8)
            {
                LastPaths.RemoveRange(8, LastPaths.Count - 8);
            }
        }
    }
}
