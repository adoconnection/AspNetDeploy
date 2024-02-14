using System.Text;
using Ionic.Zip;
using Ionic.Zlib;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Configuration;
using System.Reflection;
using AspNetDeploy.Contracts;

namespace MachineServices
{
    public class MachineInstanceFactory
    {
        private readonly CertificateManager _certificateManager;

        public MachineInstanceFactory(IPathServices pathServices)
        {
            this._certificateManager = new CertificateManager(pathServices);
        }
        public ZipFile Create(MachineConfigModel machineConfigModel)
        {
            MachineConfigFactory configFactory = new MachineConfigFactory();

            string templatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"], "MachineAgent", "Template");

            XDocument machineConfig = configFactory.CreateConfig(machineConfigModel);
            machineConfig.Save(Path.Combine(templatePath, "App.Config"));

            this._certificateManager.CreateAndSaveCertificateForMachine();

            ZipFile zipFile = new ZipFile(Encoding.UTF8);

            zipFile.AlternateEncoding = Encoding.UTF8;
            zipFile.AlternateEncodingUsage = ZipOption.Always;
            zipFile.CompressionLevel = CompressionLevel.BestCompression;

            zipFile.AddDirectory(templatePath);

            return zipFile;
        }
    }
}
