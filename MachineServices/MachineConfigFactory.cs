using System.Xml.Linq;

namespace MachineServices
{
    public class MachineConfigFactory
    {
        public XDocument CreateConfig(MachineConfigModel model)
        {
            return new XDocument(
                new XElement("configuration",
                    new XElement("appSettings",
                        new XElement("add", new XAttribute("key", "PackagesPath"), new XAttribute("value", model.PackagesPath)),
                        new XElement("add", new XAttribute("key", "BackupsPath"), new XAttribute("value", model.BackupsPath)),
                        new XElement("add", new XAttribute("key", "Service.URI"), new XAttribute("value", model.Uri)),
                        new XElement("add", new XAttribute("key", "Authorization.Enabled"), new XAttribute("value", model.IsAuthorizationEnabled))
                    )
                )
            );
        }
    }
}