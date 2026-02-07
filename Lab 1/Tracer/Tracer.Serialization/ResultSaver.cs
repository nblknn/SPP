using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization {
    public class ResultSaver {
        public void SaveResult(TraceResult result) {
            PluginLoader pluginLoader = new PluginLoader();
            List<ITraceResultSerializer> serializers =
                pluginLoader.LoadPlugins("dlls");
            Directory.CreateDirectory("result");
            for (int i = 0; i < serializers.Count; i++) {
                Stream stream = File.Create($"result/{i.ToString()}.{serializers[i].Format}");
                serializers[i].Serialize(result, stream);
                stream.Close();
            }
        }
    }
}
