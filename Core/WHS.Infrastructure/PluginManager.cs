using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Caliburn.Micro;
using WHS.Infrastructure.Events;
using WHS.Infrastructure.NlogEx;

namespace WHS.Infrastructure
{
    public class PluginManager
    {
        private static ArrayList _list;

        private static AppDomain _domain;

        private static List<string> _folderList;

        private static Dictionary<Guid, HotPlugins.PluginLoader> _loaderList;

        private static Hashtable _alreadyUnresolvedAndNotFoundDlls;

        private static bool _pluginInitCalled;

        private static List<string> _allPuliginCustomDlls;

        private static List<string> _languages;

        static PluginManager()
        {

            _languages = WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.DefaultProvider.AvailableCultures
                .Where(w=>!string.IsNullOrEmpty(w.Name))
                .Select(s => s.Name).ToList();

            PluginManager._folderList = new List<string>();
            PluginManager._alreadyUnresolvedAndNotFoundDlls = new Hashtable();
            _loaderList = new Dictionary<Guid, HotPlugins.PluginLoader>();
            PluginManager._pluginInitCalled = false;
            PluginManager._list = new ArrayList();
            _allPuliginCustomDlls = new List<string>();
            //初始化加载主程序已经加载的DLL，防止插件重复加载
            _allPuliginCustomDlls.AddRange(new string[]
                { "Caliburn.Micro.Core.dll",
                "Caliburn.Micro.Platform.dll",
                "ControlzEx.dll",
                "MahApps.Metro.dll",
                "MahApps.Metro.IconPacks.Core.dll",
                "MahApps.Metro.IconPacks.FontAwesome.dll",
                "Microsoft.DotNet.PlatformAbstractions.dll",
                "Microsoft.Extensions.DependencyModel.dll",
                "Microsoft.Xaml.Behaviors.dll",
                "Newtonsoft.Json.Bson.dll",
                "Newtonsoft.Json.dll",
                "NLog.dll",
                "Polly.dll",
                "System.IO.Ports.dll",
                "System.Management.dll",
                "System.Net.Http.Formatting.dll",
                "WHS.Infrastructure.dll",
                "WPFLocalizeExtension.dll",
                "XAMLMarkupExtensions.dll",
                });
        }

        public static void Initialize()
        {
            PluginManager._domain = AppDomain.CurrentDomain;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(PluginManager.CurrentDomain_AssemblyResolve);
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var addPlugins = PluginManager.LoadFromFolder(Path.Combine(directoryName, "Plugins"), typeof(PluginDefinition), "plugin.def");
            PluginManager._list.AddRange(addPlugins);
            foreach (PluginDefinition pd in PluginManager._list)
            {
                PluginManager.LogPluginPath(pd);
                //发布主界面RELOAD的异步事件
                GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
                {
                    HandleType = HandleType.Add,
                    PluginID = pd.Id
                }, (action) => { return action(); });
            }

            string fullName = Assembly.GetExecutingAssembly().FullName;
            int num = fullName.LastIndexOf('\\');
            if (num > 0)
            {
                string item = fullName.Substring(0, num + 1);
                if (PluginManager._folderList != null && PluginManager._folderList.Contains(item))
                    PluginManager._folderList.Add(item);
            }
        }

        public static void AddEmbeddedPlugins(List<PluginDefinition> plugins)
        {
            PluginManager._list.AddRange(plugins);
        }

        public static void InitializeMore(string path)
        {
            if (path == null)
            {
                return;
            }
            PluginManager._domain = AppDomain.CurrentDomain;
            try
            {
                List<object> list = PluginManager.LoadFromFolder(path, typeof(PluginDefinition), "plugin.def");
                using (List<object>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        PluginDefinition pluginDefinition = (PluginDefinition)enumerator.Current;
                        bool flag = false;
                        foreach (PluginDefinition pluginDefinition2 in PluginManager._list)
                        {
                            if (pluginDefinition2.Id == pluginDefinition.Id)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            PluginManager._list.Add(pluginDefinition);
                            //发布主界面RELOAD的异步事件
                            GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
                            {
                                HandleType = HandleType.Add,
                                PluginID = pluginDefinition.Id
                            }, (action) => { return action(); });
                            PluginManager.LogPluginPath(pluginDefinition);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Warn("Unable to load plugins from path:" + path + "  -  " + ex.Message);
            }
        }

        public static void InitializeOnly(string path)
        {
            PluginManager._domain = AppDomain.CurrentDomain;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(PluginManager.CurrentDomain_AssemblyResolve);
            PluginManager._list = new ArrayList();
            var addPlugins = PluginManager.LoadFromFolder(path, typeof(PluginDefinition), "plugin.def");

            PluginManager._list.AddRange(addPlugins);

            foreach (PluginDefinition pd in addPlugins)
            {
                //发布主界面RELOAD的异步事件
                GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
                {
                    HandleType = HandleType.Add,
                    PluginID = pd.Id
                }, (action) => { return action(); });
                PluginManager.LogPluginPath(pd);
            }
        }

        public static void PluginInitCheckForStandalone(Caliburn.Micro.SimpleContainer container)
        {
            if (!PluginManager._pluginInitCalled)
            {
                PluginManager._pluginInitCalled = true;
                foreach (PluginDefinition pluginDefinition in PluginManager._list)
                {
                    try
                    {
                        pluginDefinition.Init();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public static void PluginCloseCheckForStandalone()
        {
            if (PluginManager._pluginInitCalled)
            {
                PluginManager._pluginInitCalled = false;
                foreach (PluginDefinition pluginDefinition in PluginManager._list)
                {
                    try
                    {
                        pluginDefinition.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private static void LogPluginPath(PluginDefinition pd)
        {
            LogUtil.Info(pd.Name + ", loaded from:" + pd.GetType().Assembly.Location);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string text = args.Name.Split(new char[]
            {
                ','
            })[0];
            if (PluginManager._alreadyUnresolvedAndNotFoundDlls[text] == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly[] array = assemblies;
                for (int i = 0; i < array.Length; i++)
                {
                    Assembly assembly = array[i];
                    if (text == assembly.FullName.Split(new char[]
                    {
                        ','
                    })[0])
                    {
                        break;
                    }
                }
                foreach (string current in PluginManager._folderList)
                {
                    Assembly assembly2 = PluginManager.CheckDirectory(current, text);
                    if (assembly2 != null)
                    {
                        return assembly2;
                    }
                }
                PluginManager._alreadyUnresolvedAndNotFoundDlls[text] = "NotFound";
            }
            return null;
        }

        private static Assembly CheckDirectory(string dir, string assemblyName)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir);
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i];
                    string fileName = Path.GetFileName(path);
                    if (assemblyName.ToLower() + ".dll" == fileName.ToLower())
                    {
                        return Assembly.LoadFile(path);
                    }
                }
            }
            return null;
        }

        public static void UnloadPluginDefinition(PluginDefinition pluginDefinition)
        {
            lock (PluginManager._list)
            {
                if (PluginManager._list.Contains(pluginDefinition))
                {
                    if (_loaderList.ContainsKey(pluginDefinition.Id))
                    {
                        _loaderList[pluginDefinition.Id].Dispose();
                        _loaderList.Remove(pluginDefinition.Id);
                    }
                    PluginManager._list.Remove(pluginDefinition);
                }
            }
        }

        public static void UnloadAllPluginDefinition()
        {
            lock (PluginManager._list)
            {
                foreach (var loader in _loaderList)
                {
                    loader.Value.Dispose();
                }
                foreach (PluginDefinition plugin in _list)
                {
                    plugin.Close();
                }
                PluginManager._loaderList.Clear();
                PluginManager._list.Clear();
                PluginManager._folderList.Clear();
            }
        }

        public static void DeepReload()
        {
            lock (PluginManager._list)
            {
                foreach (PluginDefinition item in PluginManager._list)
                {
                    item.Close();
                }
                PluginManager._list.Clear();
                PluginManager._folderList.Clear();
                Initialize();
            }
        }

        public static ArrayList GetPluginDefinitions()
        {
            return PluginManager._list;
        }

        public static PluginDefinition GetPluginDefinition(Guid id)
        {
            foreach (PluginDefinition pluginDefinition in PluginManager._list)
            {
                if (pluginDefinition.Id == id)
                {
                    return pluginDefinition;
                }
            }
            return null;
        }

        public static bool PlatformDefinitionIdExists(Guid id)
        {
            foreach (PluginDefinition pluginDefinition in PluginManager._list)
            {
                if (pluginDefinition.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<object> LoadFromFolder(string folderPath, Type pluginInterfaceType, string pluginDefName = "plugin.def")
        {
            List<object> list = new List<object>();
            List<PluginDefModel> list2 = new List<PluginDefModel>();
            try
            {
                if (Directory.Exists(folderPath))
                {
                    string text = Path.Combine(folderPath, pluginDefName);
                    if (File.Exists(text))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(text);

                        XmlNode xmlNode = xmlDocument.SelectSingleNode("plugin/runPlatform/@target");
                        if (xmlNode != null && !string.IsNullOrWhiteSpace(xmlNode.Value))
                        {
                            if (xmlNode.Value.ToLower() != "anycpu")
                            {
                                if (GlobalContext.OSArchitecture.ToString().ToLower() != xmlNode.Value.ToLower())
                                {
                                    LogUtil.Warn($"无法加载插件({pluginInterfaceType.Name})，因为插件所属的平台({xmlNode.Value.ToLower()})和运行的平台({GlobalContext.OSArchitecture.ToString().ToLower()})不一致");
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            LogUtil.Warn($"无法加载插件({pluginInterfaceType.Name})，请定义runPlatform xml标签如:<runPlatform target=\"anycpu\"/> 其中target:anycpu,x64,x86,arm,arm64,wasm");

                            return null;
                        }
                        bool enableHotReload = false;
                        XmlNode enableHotReloadNode = xmlDocument.SelectSingleNode("plugin/enableHotReload");
                        if (enableHotReloadNode != null && !string.IsNullOrWhiteSpace(enableHotReloadNode.InnerText))
                        {
                            bool result = false;
                            bool.TryParse(enableHotReloadNode.InnerText, out result);
                            enableHotReload = result;
                        }

                        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("plugin/file/@name");
                        foreach (XmlNode xmlNode2 in xmlNodeList)
                        {
                            PluginDefModel pluginDef = new PluginDefModel();
                            pluginDef.file = Path.Combine(folderPath, xmlNode2.Value);
                            pluginDef.runPlatform = GlobalContext.OSArchitecture.ToString().ToLower();
                            pluginDef.enableHotReload = enableHotReload;
                            list2.Add(pluginDef);
                        }

                    }
                    foreach (PluginDefModel current in list2)
                    {
                        bool isexist = false;
                        foreach (PluginDefinition item in PluginManager._list)
                        {
                            if (item.FileName == current.file)
                            {
                                isexist = true;
                                break;
                            }

                        }
                        if (!isexist)//防止插件已经加载
                        {
                            List<object> list3 = PluginManager.FindPlugins(current.file, pluginInterfaceType, current.enableHotReload);
                            if (list3 != null)
                            {
                                list.AddRange(list3);
                            }
                        }
                    }
                    string[] directories = Directory.GetDirectories(folderPath);
                    for (int i = 0; i < directories.Length; i++)
                    {
                        string folderPath2 = directories[i];
                        List<object> list4 = PluginManager.LoadFromFolder(folderPath2, pluginInterfaceType, pluginDefName);
                        if (list4 != null)
                        {
                            list.AddRange(list4);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            return list;
        }



        internal static List<object> FindPlugins(string fileName, Type typeToLoad, bool enableHotReload)
        {
            List<object> list = new List<object>();
            string nameOfClass = fileName;
            try
            {
                var dir = Directory.GetParent(fileName);
                var allDlls = dir.GetFiles("*.dll");
                var privateDlls = new List<string>();
                var loader = HotPlugins.PluginLoader.CreateFromAssemblyFile(
                       fileName,
                       config =>
                       {
                           config.AdditionalProbingPaths.Add(dir.FullName);//配置加载资源文件夹
                           foreach (var language in _languages)
                           {
                               config.ResourceProbingSubpaths.Add(language);
                           }
                           config.EnableHotReload = enableHotReload;
                       });
                foreach (var item in allDlls)
                {
                    if (item.FullName == fileName)
                        continue;
                    var customFileName = Path.GetFileName(item.FullName);
                    if (!_allPuliginCustomDlls.Contains(customFileName))
                    {
                        _allPuliginCustomDlls.Add(customFileName);
                        loader.LoadAssemblyFromPath(item.FullName);
                    }
                }

                var plugins = loader.LoadDefaultAssembly() .GetTypes()
.Where(t => typeof(PluginDefinition).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass).ToList();

                foreach (var pluginType in plugins)
                {
                    #region 内存加载，可以实现热加载
                    var obj = pluginType.GetConstructors()[0].Invoke(new object[] { }) as PluginDefinition;
                    if (obj != null)
                    {
                        obj.EnableHotReload = enableHotReload;
                        obj.FileName = fileName;
                        list.Add(obj);
                        if (!_loaderList.ContainsKey(obj.Id))
                        {
                            loader.Reloaded += Loader_Reloaded;
                            _loaderList.Add(obj.Id, loader);
                        }
                    }
                    #endregion
                    #region 常规加载
                    //ObjectHandle objectHandle = Activator.CreateInstanceFrom(fileName, pluginType.ToString());
                    //if (objectHandle != null)
                    //{
                    //    PluginDefinition obj = objectHandle.Unwrap() as PluginDefinition;
                    //    if (obj != null)
                    //    {
                    //        obj.FileName = fileName;
                    //        obj.PluginLoader = loader;
                    //        list.Add(obj);
                    //    }
                    //} 
                    #endregion
                }
            }
            catch (Exception ex2)
            {
                if (fileName != null)
                {
                    int num2 = fileName.LastIndexOf("\\");
                    if (num2 < 0)
                    {
                        nameOfClass = fileName;
                    }
                    else
                    {
                        nameOfClass = fileName.Substring(num2 + 1);
                    }
                }
                ReflectionTypeLoadException ex = ex2 as ReflectionTypeLoadException;
                LogUtil.Error(ex2.Message);
                if (ex != null && ex.LoaderExceptions != null)
                {
                    Exception[] loaderExceptions = ex.LoaderExceptions;
                    for (int j = 0; j < loaderExceptions.Length; j++)
                    {
                        Exception ex3 = loaderExceptions[j];
                        LogUtil.Error(ex3.Message);
                    }
                }
            }
            return list;
        }

        private static void Loader_Reloaded(object sender, HotPlugins.PluginReloadedEventArgs eventArgs)
        {
            try
            {
                foreach (var pluginType in eventArgs.Loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(PluginDefinition).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass))
                {
                    //初始化插件
                    var obj = pluginType.GetConstructors()[0].Invoke(new object[] { }) as PluginDefinition;

                    if (obj != null)
                    {
                        //获取老的插件，删除然后释放资源
                        var origin = GetPluginDefinition(obj.Id);
                        if (origin != null)
                        {
                            obj.EnableHotReload = origin.EnableHotReload;
                            obj.FileName = origin.FileName;
                            origin.Close();
                            if (PluginManager._list.Contains(origin))
                            {
                                PluginManager._list.Remove(origin);
                            }


                        }
                        //将新插件加入到集合
                        PluginManager._list.Add(obj);
                        //发布主界面RELOAD的异步事件
                        GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
                        {
                            HandleType = HandleType.Reload,
                            PluginID = obj.Id
                        }, (action) => { return action(); });

                    }
                }
            }
            catch
            {

            }
        }
    }

    public class PluginDefModel
    {
        public string file
        {
            get; set;
        }

        public string runPlatform
        {
            get; set;
        }

        public bool enableHotReload
        {
            get; set;
        }
    }
}
