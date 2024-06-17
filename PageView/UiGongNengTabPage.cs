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

        /// <summary>
        /// 会话列表增加触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiListBox1_ItemsAdd(object sender, EventArgs e)
        {
            //todo 增加会话列表事件处理
        }


        /// <summary>
        /// 拖放过程中事件，赋值允许拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiListBox1_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖放数据是否包含会话信息
            if (e.Data.GetDataPresent(typeof(SessionData[])))
            {
                e.Effect = DragDropEffects.All;
            }
        }


        /// <summary>
        /// 获取到拖放会话列表处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiListBox1_DragDrop(object sender, DragEventArgs e)
        {
            // 获取拖放的会话数据
            Session[] sessions = (Session[])e.Data.GetData(typeof(Session[]));
            // 处理会话数据
            ProcessSessions(sessions);
        }


        /// <summary>
        /// 双击删除指定会话列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiListBox1_DoubleClick(object sender, EventArgs e)
        {
            int index = uiListBox1.SelectedIndex;
            if (index == -1)
            {
                return;
            }

            //会话列表同步删除
            uiListBox1.Items.RemoveAt(index);
            _sessionList.RemoveAt(index);
        }

        /// <summary>
        /// 一键生成代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            //todo 一键生成代码
        }


        //一键复制至剪辑板
        private void uiButton2_Click(object sender, EventArgs e)
        {
            //todo 一键复制到剪辑板
        }



    }
}