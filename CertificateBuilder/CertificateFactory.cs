using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace CertificateBuilder
{
    public class CertificateFactory
    {
        public Tuple<X509Certificate2, RSA> CreateRootCertificate()
        {
            RSA rsaKey = RSA.Create(2048);
            string subject = "CN=myauthority.ru";
            CertificateRequest certReq = new CertificateRequest(subject, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));

            DateTimeOffset expirate = DateTimeOffset.Now.AddYears(5);
            X509Certificate2 caCert = certReq.CreateSelfSigned(DateTimeOffset.Now, expirate);

            return new Tuple<X509Certificate2, RSA>(caCert, rsaKey);
        }

        public X509Certificate2 CreateCertificate(X509Certificate2 rootCertificate)
        {
            var certificateKey = RSA.Create(2048);

            string certificateSubject = "CN=10.10.10.*";
            var expirate = DateTimeOffset.Now.AddYears(4);
            byte[] certificateSerialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());

            var certificateRequest = new CertificateRequest(certificateSubject, certificateKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            X509Certificate2 newCertificate = certificateRequest.Create(rootCertificate, DateTimeOffset.Now, expirate, certificateSerialNumber);
            
            return newCertificate.CopyWithPrivateKey(certificateKey);
        }

        
        private static AsymmetricCipherKeyPair GetAsymmetricCipherKeyPair()
        {
            var keyPairGen = new RsaKeyPairGenerator();
            var keyParams = new KeyGenerationParameters(
                new SecureRandom(new CryptoApiRandomGenerator()), 2048);
            keyPairGen.Init(keyParams);
            var keyPair = keyPairGen.GenerateKeyPair();
            return keyPair;
        }
    }
}