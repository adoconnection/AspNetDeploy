using System.Text;
using Ionic.Zip;
using Ionic.Zlib;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace MachineServices
{
    public class MachineInstanceFactory
    {
        public ZipFile Create(MachineConfigModel machineConfigModel)
        {
            MachineConfigFactory configFactory = new MachineConfigFactory();
            CertificateManager certificateManager = new CertificateManager();
            string templatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"], "MachineAgent", "Template");

            XDocument machineConfig = configFactory.CreateConfig(machineConfigModel);
            machineConfig.Save(Path.Combine(templatePath, "App.Config"));

            certificateManager.CreateAndSaveCertificateForMachine();

            ZipFile zipFile = new ZipFile(Encoding.UTF8);

            zipFile.AlternateEncoding = Encoding.UTF8;
            zipFile.AlternateEncodingUsage = ZipOption.Always;
            zipFile.CompressionLevel = CompressionLevel.BestCompression;

            zipFile.AddDirectory(templatePath);

            return zipFile;
        }
    }
}
