using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Utils
{
    public static class AssemblyUtil
    {
        public static T CreateInstance<T>(string type)
        {
            return AssemblyUtil.CreateInstance<T>(type, new object[0]);
        }

        public static T CreateInstance<T>(string type, object[] parameters)
        {
            T t = default(T);
            Type type2 = Type.GetType(type, true);
            if (type2 == null)
            {
                throw new Exception(string.Format("The type '{0}' was not found!", type));
            }
            object obj = Activator.CreateInstance(type2, parameters);
            return (T)((object)obj);
        }

        public static Type GetType(string fullTypeName, bool throwOnError, bool ignoreCase)
        {
            Type type = Type.GetType(fullTypeName, false, ignoreCase);
            Type result;
            if (type != null)
            {
                result = type;
            }
            else
            {
                string[] array = fullTypeName.Split(new char[]
                {
                    ','
                });
                string assemblyString = array[1].Trim();
                try
                {
                    Assembly assembly = Assembly.Load(assemblyString);
                    string typeNamePrefix = array[0].Trim() + "`";
                    Type[] array2 = (from t in assembly.GetExportedTypes()
                                     where t.IsGenericType && t.FullName.StartsWith(typeNamePrefix, ignoreCase, CultureInfo.InvariantCulture)
                                     select t).ToArray<Type>();
                    if (array2.Length != 1)
                    {
                        result = null;
                    }
                    else
                    {
                        result = array2[0];
                    }
                }
                catch (Exception ex)
                {
                    if (throwOnError)
                    {
                        throw ex;
                    }
                    result = null;
                }
            }
            return result;
        }

        public static IEnumerable<Type> GetImplementTypes<TBaseType>(this Assembly assembly)
        {
            return from t in assembly.GetExportedTypes()
                   where t.IsSubclassOf(typeof(TBaseType)) && t.IsClass && !t.IsAbstract
                   select t;
        }

        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly, Type targetType) where TBaseInterface : class
        {
            Type[] exportedTypes = assembly.GetExportedTypes();
            List<TBaseInterface> list = new List<TBaseInterface>();
            for (int i = 0; i < exportedTypes.Length; i++)
            {
                Type type = exportedTypes[i];
                if (!type.IsAbstract)
                {
                    if (targetType.IsAssignableFrom(type))
                    {
                        list.Add((TBaseInterface)((object)Activator.CreateInstance(type)));
                    }
                }
            }
            return list;
        }

        public static T BinaryClone<T>(this T target)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, target);
                memoryStream.Position = 0L;
                result = (T)((object)binaryFormatter.Deserialize(memoryStream));
            }
            return result;
        }

        public static T CopyPropertiesTo<T>(this T source, T target)
        {
            return source.CopyPropertiesTo((PropertyInfo p) => true, target);
        }

        public static T CopyPropertiesTo<T>(this T source, Predicate<PropertyInfo> predict, T target)
        {
            PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Dictionary<string, PropertyInfo> dictionary = properties.ToDictionary((PropertyInfo p) => p.Name);
            PropertyInfo[] array = (from p in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                                    where predict(p)
                                    select p).ToArray<PropertyInfo>();
            int i = 0;
            while (i < array.Length)
            {
                PropertyInfo propertyInfo = array[i];
                PropertyInfo propertyInfo2;
                if (dictionary.TryGetValue(propertyInfo.Name, out propertyInfo2))
                {
                    if (!(propertyInfo2.PropertyType != propertyInfo.PropertyType))
                    {
                        if (propertyInfo2.PropertyType.IsSerializable)
                        {
                            propertyInfo.SetValue(target, propertyInfo2.GetValue(source, null), null);
                        }
                    }
                }
                i++;
                continue;

            }
            return target;
        }

        public static IEnumerable<Assembly> GetAssembliesFromString(string assemblyDef)
        {
            return AssemblyUtil.GetAssembliesFromStrings(assemblyDef.Split(new char[]
            {
                ',',
                ';'
            }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static IEnumerable<Assembly> GetAssembliesFromStrings(string[] assemblies)
        {
            List<Assembly> list = new List<Assembly>(assemblies.Length);
            for (int i = 0; i < assemblies.Length; i++)
            {
                string assemblyString = assemblies[i];
                list.Add(Assembly.Load(assemblyString));
            }
            return list;
        }
    }
}
