using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YukiDrive.CLI.Services
{
    public class SettingService
    {
        private const string settingPath = "settings.json";

        public Settings settings;
        /// <summary>
        /// 设置服务
        /// </summary>
        public SettingService()
        {
            Init();
        }

        public void Init()
        {
            if(!File.Exists("settings.json"))
            {
                this.settings = new Settings();
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText("settings.json",json);
            } else {
                string json = File.ReadAllText("settings.json");
                this.settings = JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveSettings(){
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText("settings.json",json);
        }

        public class Settings
        {
            public string ApiUrl { get; set; }
            public string UploadPassword { get; set; }
        }
    }
}