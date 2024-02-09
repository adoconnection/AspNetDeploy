using CertificateBuilder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;
using System.Xml.Linq;

namespace MachineServices
{
    public class CertificateManager
    {
        private readonly IPathServices _pathServices;
        public CertificateManager(IPathServices pathServices)
        {
            this._pathServices = pathServices;
        }

        public X509Certificate2 GetRootCertificate(bool isPfx)
        {
            string path = this._pathServices.GetRootCertificatePath(isPfx);

            if (!File.Exists(path))
            {
                return null;
            }

            if (isPfx)
            {
                return new X509Certificate2(path, "aspnetdeploy");
            }

            return new X509Certificate2(X509Certificate.CreateFromCertFile(path));
        }

        public X509Certificate2 GetClientCertificate()
        {
            return new X509Certificate2(this._pathServices.GetClientCertificatePath(), "aspnetdeploy");
        }

        public void CreateAndSaveCertificateForMachine()
        {
            X509Certificate2 rootPublicKey = this.GetRootCertificate(false);

            this.CreateAndSavePfxCertificate(this._pathServices.GetMachineCertificatePath());
            this.SaveCrt(rootPublicKey, this._pathServices.GetMachineCertificatePath(true));
        }

        public void CreateAndSaveClientCertificate()
        {
            this.CreateAndSavePfxCertificate(this._pathServices.GetClientCertificatePath());
        }

        private void CreateAndSavePfxCertificate(string certPath)
        {
            X509Certificate2 rootCertificate = this.GetRootCertificate(true);
            CertificateFactory certificateFactory = new CertificateFactory();

            X509Certificate2 newCertificate = certificateFactory.CreateCertificate(rootCertificate);

            byte[] certificateData = newCertificate.Export(X509ContentType.Pfx, "aspnetdeploy");
            File.WriteAllBytes(certPath, certificateData);
        }

        public void CreateAndSaveRootCertificate()
        {
            CertificateFactory certificateFactory = new CertificateFactory();

            Tuple<X509Certificate2, RSA> generatedData = certificateFactory.CreateRootCertificate();

            X509Certificate2 rootCertificate = generatedData.Item1;
            RSA rootCertPrivateKey = generatedData.Item2;

            byte[] certificateData = rootCertificate.Export(X509ContentType.Pfx, "aspnetdeploy");
            File.WriteAllBytes(this._pathServices.GetRootCertificatePath(true), certificateData);

            this.SaveCrt(rootCertificate, this._pathServices.GetRootCertificatePath(false));
        }

        private void SaveCrt(X509Certificate2 certificate, string path)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");
            File.WriteAllText(path, builder.ToString());
        }
    }
}
