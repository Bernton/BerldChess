using System;
using System.Collections.Generic;

namespace BerldChess.Model
{
    [Serializable]
    public class EngineList
    {
        public int SelectedIndex1 { get; set; }
        public int SelectedIndex2 { get; set; }
        public List<EngineSetting> Settings { get; set; }
        public List<string> LastPaths { get; set; } = new List<string>();

        public bool SettingAvailable
        {
            get
            {
                return Settings.Count > 0;
            }
        }

        public EngineSetting SelectedSetting1
        {
            get
            {
                if(SelectedIndex1 == -1 || Settings.Count == 0)
                {
                    return null;
                }

                return Settings[SelectedIndex1];
            }
        }

        public EngineSetting SelectedSetting2
        {
            get
            {
                if (SelectedIndex2 == -1 || Settings.Count == 0)
                {
                    return null;
                }

                return Settings[SelectedIndex2];
            }
        }


        public EngineList()
        {
            Settings = new List<EngineSetting>();
            SelectedIndex1 = -1;
        }


        public void AddLastPath(string path)
        {
            if (!LastPaths.Contains(path))
            {
                LastPaths.Add(path);

                if (LastPaths.Count > 8)
                {
                    LastPaths.RemoveRange(8, LastPaths.Count - 8);
                }
            }
        }
    }
}
