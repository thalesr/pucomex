using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoRFB
{
    public class SecurityWebClient : System.Net.WebClient
    {

        System.Net.HttpWebRequest request = null;
        System.Security.Cryptography.X509Certificates.X509CertificateCollection certificates = null;

        protected override System.Net.WebRequest GetWebRequest(System.Uri address)
        {
            request = (System.Net.HttpWebRequest)base.GetWebRequest(address);
            if (certificates != null)
            {
                request.ClientCertificates.AddRange(certificates);
                request.PreAuthenticate = true;
            }
            return request;
        }

        public void AddCerts(System.Security.Cryptography.X509Certificates.X509Certificate[] certs)
        {
            if (certificates == null)
            {
                certificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
            }
            if (request != null)
            {
                request.ClientCertificates.AddRange(certs);
            }
            certificates.AddRange(certs);
        }

    }
}
