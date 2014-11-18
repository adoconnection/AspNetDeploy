using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SatelliteService.Helpers
{
    public class XmlMerger
    {
        public IDictionary<string, object> MacroCollection { get; set; }
        private readonly XmlDocument xmlDocument;

        public XmlMerger(XmlDocument xmlDocument, IDictionary<string, object> macroCollection)
        {
            this.MacroCollection = macroCollection;
            this.xmlDocument = xmlDocument;
        }

        public XmlMerger(XmlDocument xmlDocument) : this(xmlDocument, new Dictionary<string, object>())
        {
        }

        public void ApplyChanges(XmlNode xmlNode)
        {
            this.MergeNodes(this.xmlDocument, xmlNode);
        }

        private void MergeNodes(XmlNode initialNode, XmlNode changesNode)
        {
            this.MergeNodeAttributes(initialNode, changesNode);

            string childKey = changesNode.Attributes != null && changesNode.Attributes["childNodesKeyName"] != null
                                  ? changesNode.Attributes["childNodesKeyName"].Value
                                  : null;

            string collectionKey = changesNode.Attributes != null && changesNode.Attributes["collectionKeyName"] != null
                                  ? changesNode.Attributes["collectionKeyName"].Value
                                  : string.Empty;

            IDictionary<string, bool> collectionMarkers = this.GetCollectionMarkers(initialNode, changesNode, collectionKey);

            foreach (XmlElement changesChildNode in changesNode.ChildNodes.OfType<XmlElement>())
            {
                this.MergeNode(initialNode, childKey, changesChildNode, collectionMarkers);
            }
        }

        private void MergeNode(XmlNode initialParentNode, string childKey, XmlElement changesChildNode, IDictionary<string, bool> collectionMarkers)
        {
            string nodeName = changesChildNode.Name;
            bool emptyKey = string.IsNullOrEmpty(childKey) || changesChildNode.Attributes[childKey] == null || string.IsNullOrEmpty(changesChildNode.Attributes[childKey].Value);
            bool isCollection = collectionMarkers.ContainsKey(nodeName) && collectionMarkers[nodeName];

            if (emptyKey)
            {
                if (isCollection)
                {
                    this.AppendNewNode(initialParentNode, changesChildNode);
                    return;
                }

                XmlNode firstChild = GetFirstChild(initialParentNode, nodeName);

                if (firstChild == null)
                {
                    this.AppendNewNode(initialParentNode, changesChildNode);
                }
                else
                {
                    this.MergeNodes(firstChild, changesChildNode);
                }

                return;
            }
            else
            {
                string value = changesChildNode.Attributes[childKey].Value;
                XmlNode firstChild = GetFirstChild(initialParentNode, nodeName, childKey, value);

                if (firstChild != null)
                {
                    this.MergeNodes(firstChild, changesChildNode);
                    return;
                }

                this.AppendNewNode(initialParentNode, changesChildNode);
            }
        }

        private IDictionary<string, bool> GetCollectionMarkers(XmlNode initialParentNode, XmlNode changesChildNode, string collectionKey)
        {
            IDictionary<string, bool> initialNodeData = initialParentNode.ChildNodes
                .OfType<XmlNode>()
                .GroupBy(node => node.Name)
                .Select(
                    nodeGroup => new
                    {
                        name = nodeGroup.Key,
                        count = nodeGroup.Count()
                    })
                .ToDictionary(item => item.name, item => item.count > 1 || collectionKey.Contains(item.name));

            IDictionary<string, bool> changesNodeData = changesChildNode.ChildNodes
                .OfType<XmlNode>()
                .GroupBy(node => node.Name)
                .Select(
                    nodeGroup => new
                    {
                        name = nodeGroup.Key,
                        count = nodeGroup.Count()
                    })
                .ToDictionary(item => item.name, item => item.count > 1 || collectionKey.Contains(item.name));

            IDictionary<string, bool> result = new Dictionary<string, bool>();

            foreach (string key in initialNodeData.Keys)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, initialNodeData[key]);
                }
                else
                {
                    result[key] |= initialNodeData[key];
                }
            }

            foreach (string key in changesNodeData.Keys)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, changesNodeData[key]);
                }
                else
                {
                    result[key] |= changesNodeData[key];
                }
            }

            return result;
        }

        private XmlNode GetFirstChild(XmlNode parentElement, string name)
        {
            XmlNodeList xmlNodeList = parentElement.SelectNodes(name + "[1]");
            return xmlNodeList == null || xmlNodeList.Count == 0 ? null : xmlNodeList[0];
        }

        private XmlNode GetFirstChild(XmlNode parentElement, string name, string key, string value)
        {
            XmlNodeList xmlNodeList = parentElement.SelectNodes(string.Format("{0}[@{1}='{2}'][1]", name, key, value));
            return xmlNodeList == null || xmlNodeList.Count == 0 ? null : xmlNodeList[0];
        }

        private void MergeToFirst(XmlElement changesChildNode, XmlNodeList initialChildNodes)
        {
            this.MergeNodes(initialChildNodes[0], changesChildNode);
        }

        private void AppendNewNode(XmlNode initialNode, XmlElement changesChildNode)
        {
            XmlElement initialChildNode = initialNode.OwnerDocument.CreateElement(changesChildNode.Name);
            initialNode.AppendChild(initialChildNode);

            this.MergeNodes(initialChildNode, changesChildNode);
        }

        private void MergeNodeAttributes(XmlNode initialNode, XmlNode changesNode)
        {
            if (changesNode.Attributes == null)
            {
                return;
            }

            foreach (XmlAttribute changeAttribute in changesNode.Attributes)
            {
                if (changeAttribute.Name.ToLower() == "childnodeskeyname")
                {
                    continue;
                }
                if (changeAttribute.Name.ToLower() == "collectionkeyname")
                {
                    continue;
                }

                if (initialNode.Attributes != null)
                {
                    if (initialNode.Attributes[changeAttribute.Name] == null)
                    {
                        XmlAttribute xmlAttribute = initialNode.OwnerDocument.CreateAttribute(changeAttribute.Name);
                        xmlAttribute.Value = ApplyMacro(changeAttribute.Value);
                        initialNode.Attributes.Append(xmlAttribute);
                    }
                    else
                    {
                        initialNode.Attributes[changeAttribute.Name].Value = ApplyMacro(changeAttribute.Value);
                    }
                }
            }
        }

        private string ApplyMacro(string value)
        {
            value = value ?? string.Empty;

            foreach (KeyValuePair<string, object> macros in this.MacroCollection)
            {
                value = value.Replace("{var:" + macros.Key + "}", macros.Value == null ? string.Empty : macros.Value.ToString());
            }

            return value;
        }
    }
}