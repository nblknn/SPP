using Tracer.Core;
using Tracer.Serialization.Abstractions;
using Newtonsoft.Json;
using System.Text;

namespace Tracer.Serialization.Json {
    public class JsonTraceResultSerializer : ITraceResultSerializer {
        public string Format => "json";

        public void Serialize(TraceResult traceResult, Stream to) {
            var settings = new JsonSerializerSettings() {
                Formatting = Formatting.Indented,
            };
            to.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(traceResult, settings)));
        }
    }
}
