using Tracer.Core;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;

namespace Tracer.Serialization.Yaml {
    public class YamlTraceResultSerializer : ITraceResultSerializer {
        public string Format => "yaml";

        public void Serialize(TraceResult traceResult, Stream to) {
            var serializer = new SerializerBuilder().Build();
            using (var writer = new StreamWriter(to)) {
                serializer.Serialize(writer, traceResult);
                writer.Flush();
            }
        }
    }
}
