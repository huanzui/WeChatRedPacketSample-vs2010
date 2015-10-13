using System.Threading.Tasks;

namespace WeChatRedPacketSample
{
    public interface IWeChatClient
    {
       string PostAsync(string data);
    }
}
