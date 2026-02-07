using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization {
    public class PluginLoader {
        public List<ITraceResultSerializer> LoadPlugins(string dirPath) {
            string[] files = Directory.GetFiles(dirPath, "*.dll", SearchOption.AllDirectories);
            List<ITraceResultSerializer> result = new();
            foreach (string file in files) {
                try {
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (Type type in assembly.GetTypes()) {
                        if (typeof(ITraceResultSerializer).IsAssignableFrom(type) && !type.IsInterface) {
                            ITraceResultSerializer? serializer = (ITraceResultSerializer?)Activator.CreateInstance(type);
                            if (serializer != null)
                                result.Add(serializer);
                        }
                    }
                }
                catch(Exception e) {
                    Console.WriteLine("Ошибка при загрузке плагина: " + e.Message);
                }
            }
            return result;
        }
    }
}
