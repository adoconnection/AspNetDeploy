using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateBuilder
{
    public class CertificateFactory
    {
        public X509Certificate2 CreateRootCertificate()
        {
            RSA rsaKey = RSA.Create(2048);
            string subject = "CN=myauthority.ru";
            CertificateRequest certReq = new CertificateRequest(subject, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));

            DateTimeOffset expirate = DateTimeOffset.Now.AddYears(5);
            X509Certificate2 caCert = certReq.CreateSelfSigned(DateTimeOffset.Now, expirate);

            return caCert;
        }

        public X509Certificate2 CreateCertificate(X509Certificate2 rootCertificate)
        {
            var certificateKey = RSA.Create(2048);
            string certificateSubject = "CN=10.10.10.*";
            var expirate = DateTimeOffset.Now.AddYears(5);
            byte[] certificateSerialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());

            var certificateRequest = new CertificateRequest(certificateSubject, certificateKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
            certificateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

            return certificateRequest.Create(rootCertificate, DateTimeOffset.Now, expirate, certificateSerialNumber);
        }
    }
}