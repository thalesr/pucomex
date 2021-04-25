using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoRFB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var x = new Program();
            await x.MakeRequestAsync();
        }

        public async Task MakeRequestAsync()
        {
            /*
             * 
             * AUTENTICAÇÃO
             * 
             */

            //Abre o certificado passando o path e a senha
            X509Certificate2 oCertificado = new X509Certificate2("ENDERECO_DO_CERTIFICADO", "SENHA_DO_CERTIFICADO");

            //Configurações de segurança
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AlwaysGoodCertificate);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            //Client para obter o token de autenticação
            SecurityWebClient client = new SecurityWebClient();

            //Adiciona o certificado ao client
            client.AddCerts(new X509Certificate[] { oCertificado });

            //Define a role de autenticação na RFB
            client.Headers.Add("Role-Type", "DEPOSIT");

            //Requisição
            var result = client.UploadString("https://val.portalunico.siscomex.gov.br/portal/api/autenticar", "POST", "");

            //Obtém os tokens retornados no header
            var authorization = client.ResponseHeaders.GetValues("Set-Token");
            var xCsrfToken = client.ResponseHeaders.GetValues("X-CSRF-Token");






            /*
             * 
             * ENVIO DE DADOS PARA A API DE RECINTOS
             * 
             */


            //JSON com os dados que serão enviados
            string jsonPost = File.ReadAllText(@"c:\Temp\data.json");

            //Endpoint da API de Recintos
            string requestUrl = "https://val.portalunico.siscomex.gov.br/recintos-ext/api/ext/atribuicao-troca-navio";

            //Objeto JSON retornado pela API de Recintos
            string retorno;

            using (var clientAuthenticated = new HttpClient())
            {

                //Adiciona os tokens no header da requisição
                clientAuthenticated.DefaultRequestHeaders.Add("Authorization", authorization);
                clientAuthenticated.DefaultRequestHeaders.Add("X-CSRF-Token", xCsrfToken);

                //Requisição
                var response = await clientAuthenticated.PostAsync(requestUrl, new StringContent(jsonPost, Encoding.UTF8, "application/json"));
                retorno = response.Content.ReadAsStringAsync().Result;

            }

            Console.WriteLine(retorno);
            Console.ReadLine();

        }

        private static bool AlwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

    }
}
