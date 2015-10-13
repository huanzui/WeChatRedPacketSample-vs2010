using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WeChatRedPacketSample
{
    public class WeChatClient : IWeChatClient
    {
        static readonly string _WeChatRedPacketApiEndpoint = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
        ICertificateFinder m_CertificateFinder;

        public WeChatClient(ICertificateFinder finder)
        {
            if (finder == null)
                throw new ArgumentNullException("finder");

            m_CertificateFinder = finder;
        }

        public string PostAsync(string data)
        {
            var certHandler = new WebRequestHandler();
            certHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            certHandler.UseDefaultCredentials = false;
            var cert = m_CertificateFinder.Find();
            certHandler.ClientCertificates.Add(cert);
            using (var client = new HttpClient(certHandler, true))
            {
                var response =  client.PostAsync(_WeChatRedPacketApiEndpoint, new StringContent(data, Encoding.UTF8, "application/xml"));
                var responseData = response.Result.Content.ReadAsByteArrayAsync();

                var result = Encoding.UTF8.GetString(responseData.Result);
                return result;
            }
        }
    }
}
