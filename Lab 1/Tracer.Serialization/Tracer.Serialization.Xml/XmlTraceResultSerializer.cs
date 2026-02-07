using Tracer.Core;
using Tracer.Serialization.Abstractions;
using System.Xml;

namespace Tracer.Serialization.Xml {
    public class XmlTraceResultSerializer : ITraceResultSerializer {
        public string Format => "xml";

        private XmlElement CreateMethodElement(XmlDocument xml, MethodInfo method) {
            XmlElement methodXml = xml.CreateElement("Method");
            methodXml.SetAttribute("Name", method.Name);
            methodXml.SetAttribute("Class", method.Class);
            methodXml.SetAttribute("Time", method.Time.ToString());
            foreach (MethodInfo childMethod in method.Methods) {
                methodXml.AppendChild(CreateMethodElement(xml, childMethod));
            }
            return methodXml;
        }

        private XmlElement CreateThreadElement(XmlDocument xml, ThreadInfo thread) {
            XmlElement threadXml = xml.CreateElement("Thread");
            threadXml.SetAttribute("Id", thread.Id.ToString());
            threadXml.SetAttribute("Time", thread.Time.ToString());
            foreach (MethodInfo method in thread.Methods) {
                threadXml.AppendChild(CreateMethodElement(xml, method));
            }
            return threadXml;
        }

        public void Serialize(TraceResult traceResult, Stream to) {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement(typeof(TraceResult).Name);
            xml.AppendChild(root);
            foreach (ThreadInfo thread in traceResult.Threads) {
                root.AppendChild(CreateThreadElement(xml, thread));
            }
            xml.Save(to);
        }
    }
}
