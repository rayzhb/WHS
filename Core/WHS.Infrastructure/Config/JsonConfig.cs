using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WHS.Infrastructure.NlogEx;

namespace WHS.Infrastructure.Config
{
    public class JsonConfig : IDisposable
    {
        public JObject User { get; set; }

        public delegate void UserConfigFileChangedHandler();
        public event UserConfigFileChangedHandler OnUserConfigFileChanged;

        private string _file;
        private Module _module;
        [Obsolete("即将移除")]
        public JsonConfig(Assembly assembly)
        {
            _module = assembly.ManifestModule;
            var path = assembly.Location.Replace(_module.Name, "");
            var dir = new DirectoryInfo(path);
            var user_config_filename = "settings";

            _file = path + user_config_filename + ".conf";

            if (File.Exists(_file))
            {
                try
                {
                    User = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(_file));
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex, $"{_module.Name}:获取配置异常");
                }
                WatchUserConfig(new FileInfo(_file));
            }
            else
            {
                User = new JObject();
            }
        }

        public JsonConfig(string filename)
        {
            var dir = Directory.GetParent(filename);
            var user_config_filename = "settings";

            _file = dir.FullName + "\\" + user_config_filename + ".conf";

            if (File.Exists(_file))
            {
                try
                {
                    User = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(_file));
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex, $"{_module.Name}:获取配置异常");
                }
                WatchUserConfig(new FileInfo(_file));
            }
            else
            {
                User = new JObject();
            }
        }

        private FileSystemWatcher userConfigWatcher;
        public void WatchUserConfig(FileInfo info)
        {
            var lastRead = File.GetLastWriteTime(info.FullName);
            userConfigWatcher = new FileSystemWatcher(info.Directory.FullName, info.Name);
            userConfigWatcher.NotifyFilter = NotifyFilters.LastWrite;
            userConfigWatcher.Changed += delegate
            {
                DateTime lastWriteTime = File.GetLastWriteTime(info.FullName);
                if (lastWriteTime.Subtract(lastRead).TotalMilliseconds > 100)
                {
                    LogUtil.Info($"{_module.Name}:配置文件发送改变");
                    try
                    {
                        User = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                    }
                    catch (IOException)
                    {
                        System.Threading.Thread.Sleep(100); //Sleep shortly, and try again.
                        try
                        {
                            User = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                        }
                        catch (Exception)
                        {
                            LogUtil.Error($"{_module.Name}:获取配置异常，检查配置文件是否正确");
                            throw;
                        }
                    }

                    if (OnUserConfigFileChanged != null)
                        OnUserConfigFileChanged();
                }
                lastRead = lastWriteTime;
            };
            userConfigWatcher.EnableRaisingEvents = true;
        }

        private void SaveUserConfig()
        {
            var str = JsonConvert.SerializeObject(User);
            if (!File.Exists(_file))
            {
                File.WriteAllText(_file, str);
                WatchUserConfig(new FileInfo(_file));
            }
            else
            {
                userConfigWatcher.EnableRaisingEvents = false;
                File.WriteAllText(_file, str);
                userConfigWatcher.EnableRaisingEvents = true;
            }

        }

        public void SaveUserConfig(object data)
        {
            User = JObject.Parse(JsonConvert.SerializeObject(data));
            this.SaveUserConfig();

        }

        public void Dispose()
        {
            if (userConfigWatcher != null)
            {
                userConfigWatcher.EnableRaisingEvents = false;
                userConfigWatcher.Dispose();
                userConfigWatcher = null;
            }
        }
    }
}
