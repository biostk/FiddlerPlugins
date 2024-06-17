using System.Windows.Forms;
using Fiddler;
using Sunny.UI;

namespace FiddlerPlugins
{
    public class FiddlerInit : IAutoTamper
    {
        private TabPage gongNengTabPage ;


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

            //初始化功能视图
            UiGongNengTabPage uiGongNengTabPage = new UiGongNengTabPage();
            uiGongNengTabPage.Dock = DockStyle.Fill;

            //创建页面容器
            this.gongNengTabPage = new TabPage("Fiddler To ECode");
            this.gongNengTabPage.ToolTipText = "By:学无止境 QQ:97348461";
            this.gongNengTabPage.Controls.Add(uiGongNengTabPage);
            this.gongNengTabPage.ImageIndex =(int) Fiddler.SessionIcons.Builder ;
            
            //Fiddler显示插件页、设置
            FiddlerApplication.UI.tabsViews.TabPages.Insert(0,this.gongNengTabPage);
            FiddlerApplication.UI.tabsViews.ShowToolTips = true;



        }
    }
}