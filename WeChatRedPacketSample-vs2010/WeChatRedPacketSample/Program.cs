using System;

namespace WeChatRedPacketSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //微信支付商户号
            var merchantId = "10011111";
            //支付密钥，不是公众号/服务号的密钥，在账户设置-安全设置-API安全中配置。
            var payKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            //这一行是模拟发送
            var service = new RedPacketService(merchantId, payKey, new DebugWeChatClient());
            //这里是真正发起API调用。
            //subjectDistinguishedName是微信提供下载安装的X509证书的主题可分辨名称
            //格式严格类似： "SN=10000000, CN=深圳XXXX科技有限公司, OU=MMPay, O=Tencent, L=Shenzhen, S=Guangdong, C=CN"
            //注意，CertificateStoreFinder的实现要求证书必须安装在当前用户（应用程序启动的身份）下的个人类别中。
            //不了解的请启动命令行，输入certmgr回车，左边树形列表第一个就是。
            //当然你也可以安装在其他地方，修改CertificateStoreFinder的实现。
            //如果无法在托管系统中安装证书，可以采用微信官方Demo中文件形式访问，这里不再重复。
            //基于安全的考虑，墙裂建议把证书安装在证书容器中，并在安装时设置为私钥不可导出。
            var message = "异常";
            try
            {
                //var subjectDistinguishedName = "SN=10000000, CN=深圳XXXX科技有限公司, OU=MMPay, O=Tencent, L=Shenzhen, S=Guangdong, C=CN";
                //var service = new RedPacketService(merchantId, payKey, new WeChatClient(new CertificateStoreFinder(subjectDistinguishedName)));

                //string openid= Console.ReadLine();
                var redPacket = new RedPacket
                {
                    ActName = "obt发红包",
                    Amount = 100,
                    AppId = "wx15161852365122", //微信/服务公众号Id，通过这个公众/服务号下行红包通知，如果用户没关注，通过服务消息（特殊服务号）下行。 
                    //订单号格式： 商户号 + 4位年 + 2位月 + 2位日 + 10位自然日内唯一数字。
                    BillNumber = merchantId + DateTime.Now.ToString("yyyyMMdd") + GetUniqueId().PadLeft(10, '0'),
                    IpAddress = "127.0.0.1", //触发操作的客户的IP地址
                    OpenId = "ojlO7s6HPDoU5tOLPMDe8t", //触发红包的用户在微信号内的Id。
                    Remark = "好红包备注",
                    SendName = "大话O2O红包君",
                    Wishing = "恭喜发财！"
                };

                var result = service.SendAsync(redPacket); //控制台程序可以使用Result属性来访问异步Task结果，在Asp.Net和WinForm、WPF中不要这么做。
                message = string.Format("{0}, {1}", result.Succeeded ? "成功" : "失败", result.Error);
            }
            catch (Exception ex)
            {
                Yc.WeChat.O2O.Common.Log.WriteLog(ex, "异常：");
                message = ex.Message;
            }
            Console.WriteLine(message);
            Console.ReadKey();
        }

        static string GetUniqueId()
        {
            //可以通过数据库来生成唯一的ID，这个唯一ID不要求全局唯一，只要求在一个自然日内唯一。
            //这里用随机数模拟。
            var r = new Random();
            return r.Next(1, 1000000000).ToString();
        }
    }
}
