using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CertificateHelpers
{
    public static class X509Certificate2CollectionExtender
    {
        public static X509Certificate2 FindByFriendlyName(this X509Store store, string friendlyName)
        {
            return store.Certificates
                .Cast<X509Certificate2>()
                .FirstOrDefault(certificate => certificate.FriendlyName.ToLowerInvariant() == friendlyName.ToLowerInvariant());
        }

        public static X509Certificate2 FirstOrDefault(this X509Certificate2Collection collection, Func<X509Certificate2, bool> predicate)
        {
            return collection
                .Cast<X509Certificate2>()
                .FirstOrDefault(predicate);
        }
    }
}
