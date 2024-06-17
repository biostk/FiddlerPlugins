using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Fiddler;

namespace FiddlerPlugins
{
    public partial class UiGongNengTabPage : UserControl
    {
        private List<Session> _sessionList = new List<Session>();

        public bool _module = true; //true = WinHttpR / False = WinHttpW


        public UiGongNengTabPage()
        {
            InitializeComponent();
            this.uiRadioButton1WinHttpR.Checked = true;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 接收拖放数据处理
        /// </summary>
        /// <param name="sessions"></param>
        private void ProcessSessions(Session[] sessions)
        {
            // 在这里处理会话数据，例如显示在自定义页面上
            foreach (var session in sessions)
            {
#if DEBUG
                this.uiTextBox1.Text = session.ToString(false);
#endif
                this._sessionList.Add(session);
                this.uiListBox1.Items.Add(session.url);
            }
        }

        private void uiListBox1_ItemsAdd(object sender, EventArgs e)
        {
        }

        private void uiListBox1_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖放数据是否包含会话信息
            if (e.Data.GetDataPresent(typeof(SessionData[])))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void uiListBox1_DragDrop(object sender, DragEventArgs e)
        {
            // 获取拖放的会话数据
            Session[] sessions = (Session[])e.Data.GetData(typeof(Session[]));
            // 处理会话数据
            ProcessSessions(sessions);
        }

        private void uiListBox1_DoubleClick(object sender, EventArgs e)
        {
            int index = uiListBox1.SelectedIndex;
            if (index == -1)
            {
                return;
            }

            uiListBox1.Items.RemoveAt(index);
            _sessionList.RemoveAt(index);
        }
        //一键生成代码
        private void uiButton1_Click(object sender, EventArgs e)
        {

        }
        //一键复制至剪辑板
        private void uiButton2_Click(object sender, EventArgs e)
        {

        }
    }
}