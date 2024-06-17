using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Fiddler;

namespace FiddlerPlugins
{
    public partial class UiGongNengTabPage : UserControl
    {
        private List<Session> sessionList = new List<Session>();




        public UiGongNengTabPage()
        {
            InitializeComponent();

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void uiTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            // 获取拖放的会话数据
            Session[] sessions = (Session[])e.Data.GetData(typeof(Session[]));
            // 处理会话数据
            ProcessSessions(sessions);
        }

        private void uiTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖放数据是否包含会话信息
            if (e.Data.GetDataPresent(typeof(SessionData[])))
            {
                e.Effect = DragDropEffects.All;
            }
        }
        private void ProcessSessions(Session[] sessions)
        {
            // 在这里处理会话数据，例如显示在自定义页面上
            foreach (var session in sessions)
            {
                this.uiTextBox1.Text = session.ToString(false);

            }
        }
    }
}
