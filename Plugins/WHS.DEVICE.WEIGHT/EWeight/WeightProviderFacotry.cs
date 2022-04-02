using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    public class WeightProviderFacotry
    {
        private static ConcurrentDictionary<string, IEWeight> fEWeighbriges = new ConcurrentDictionary<string, IEWeight>(StringComparer.OrdinalIgnoreCase);

        //理论上一台电脑只会接一个称，此处不作类型缓存
        public static IEWeight CreateEWeight(string key)
        {
            IEWeight result;
            if (fEWeighbriges.TryGetValue(key, out result))
            {
                return result;
            }

            var findTypes = from item in Assembly.GetAssembly(typeof(WeightAttribute)).GetExportedTypes()
                            where !item.IsAbstract && item.IsClass && item.IsPublic && typeof(IEWeight).IsAssignableFrom(item)
                            select item;
            foreach (var finder in findTypes)
            {
                var attrs = finder.GetCustomAttributes(typeof(WeightAttribute), false) as WeightAttribute[];
                if (attrs != null)
                {
                    foreach (var attr in attrs)
                    {
                        if (attr.Value.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            IEWeight oWeight = (IEWeight)Activator.CreateInstance(finder);
                            fEWeighbriges.TryAdd(key, oWeight);

                            return oWeight;
                        }
                    }
                }
            }
            return null;
        }


        public static Dictionary<string, string> LoadWeightType()
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var providers = from item in Assembly.GetAssembly(typeof(WeightAttribute)).GetExportedTypes()
                            where !item.IsAbstract && item.IsClass && item.IsPublic && typeof(IEWeight).IsAssignableFrom(item)
                            select item;
            foreach (var provider in providers)
            {
                var attrs = provider.GetCustomAttributes(typeof(WeightAttribute), false) as WeightAttribute[];
                if (attrs != null)
                {
                    foreach (var attr in attrs)
                    {
                        result.Add(attr.Name, attr.Value);
                    }
                }
            }
            return result;
        }
    }
}
