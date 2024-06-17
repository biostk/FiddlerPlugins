using Fiddler;

namespace FiddlerPlugins
{
    public class FiddlerInit :IAutoTamper
    {
         
        void IAutoTamper.AutoTamperRequestAfter(Session oSession)
        {
            //在请求发送后，你可以记录请求信息或进行其他处理。

        }

        void IAutoTamper.AutoTamperRequestBefore(Session oSession)
        {
            //在请求发送前，你可以修改请求头或请求内容。

        }

        void IAutoTamper.AutoTamperResponseAfter(Session oSession)
        {
            //在响应接收后，你可以记录响应信息或进行其他处理。

        }

        void IAutoTamper.AutoTamperResponseBefore(Session oSession)
        {
            //在响应接收前，你可以修改响应头或响应内容。

        }

        void IAutoTamper.OnBeforeReturningError(Session oSession)
        {
            //在返回错误前，你可以记录错误信息或进行其他处理。

        }

        void IFiddlerExtension.OnBeforeUnload()
        {
            //在插件卸载前，你可以进行清理工作，如注销事件处理程序。

        }

        void IFiddlerExtension.OnLoad()
        {
            //在插件加载时，你可以进行初始化工作，如注册事件处理程序或进行日志记录。

        }
    }
}
