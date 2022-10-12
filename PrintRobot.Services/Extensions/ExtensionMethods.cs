using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace PrintRobot.Services.Extensions
{
    public static class ExtensionMethods
    {
        private readonly static object _lock = new object();
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T Clone<T>(T source)
        {

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(ms);
            }
        }

        public static T cloneObject<T>(T original, List<string> propertyExcludeList)
        {
            try
            {
                Monitor.Enter(_lock);
                T copy = Activator.CreateInstance<T>();
                PropertyInfo[] piList = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo pi in piList)
                {
                    if (!propertyExcludeList.Contains(pi.Name))
                    {
                        if (pi.GetValue(copy, null) != pi.GetValue(original, null))
                        {
                            pi.SetValue(copy, pi.GetValue(original, null), null);
                        }
                    }
                }
                return copy;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
