using CertificateBuilder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MachineServices
{
    public class CertificateManager
    {
        private X509Certificate2 GetOrCreateRootCertificate()
        {
            CertificateFactory certificateFactory = new CertificateFactory();
            string rootCertificatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"], "Certificates", "Root.pem");

            try
            {
                return new X509Certificate2(rootCertificatePath, "sdfsd");
            }
            catch (FileNotFoundException ex)
            {
                X509Certificate2 rootCertificate = certificateFactory.CreateRootCertificate();
                byte[] certificateData = rootCertificate.Export(X509ContentType.Pfx, "aspnetdeploy");
                File.WriteAllBytes(rootCertificatePath, certificateData);
                return rootCertificate;
            }
        }

        public void CreateAndSaveCertificateForMachine()
        {
            string machineCertificatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"], "MachineAgent", "Template", "Certificates", "machineCertificate.pfx");
            this.CreateAndSaveCertificateInternal(machineCertificatePath);
        }

        public void CreateAndSaveCertificateForBase()
        {
            string baseCertificatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"], "Certificates", "machineCertificate.pfx");
            this.CreateAndSaveCertificateInternal(baseCertificatePath);
        }

        private void CreateAndSaveCertificateInternal(string certPath)
        {
            X509Certificate2 rootCertificate = this.GetOrCreateRootCertificate();
            CertificateFactory certificateFactory = new CertificateFactory();

            X509Certificate2 machineCertificate = certificateFactory.CreateCertificate(rootCertificate);

            string machineCertificatePath = Path.Combine(ConfigurationManager.AppSettings["Settings.WorkingFolder"],
                "MachineAgent", "Template", "Certificates", "machineCertificate.pfx");

            byte[] certificateData = rootCertificate.Export(X509ContentType.Pfx, "aspnetdeploy");
            File.WriteAllBytes(certPath, certificateData);
        }
    }
}
