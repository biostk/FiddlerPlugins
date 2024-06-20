﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Fiddler;
using Newtonsoft.Json.Linq;

namespace FiddlerPlugins
{
    public partial class UiGongNengTabPage : UserControl
    {
        private List<Session> _sessionList = new List<Session>();
        public bool _module = true; //true = WinHttpR / False = WinHttpW
        public StringBuilder _stringBuilderLs = new StringBuilder();

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
#if DEBUG
            FiddlerApplication.Log.LogFormat("会话数量：{0}", _sessionList.Count);
#endif
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
            Task.Run(async () => { uiTextBox1.Text = await AutoGenerateCode(); });
        }

        private async Task<string> AutoGenerateCode()
        {
            string code = String.Empty;

            foreach (var session in _sessionList)
            {
                StringBuilder stringBuilderRequest;
                StringBuilder stringBuilderHeader;
                StringBuilder stringBuilderData;
#if DEBUG
                FiddlerApplication.Log.LogFormat("开始生成【{0}】代码", session.host);
#endif
                stringBuilderRequest = AutoGenerateRequest(session);
                stringBuilderHeader = AutoGenerateHeader(session);
                stringBuilderData = AutoGenerateData(session);
                code += (stringBuilderRequest.ToString() +
                         stringBuilderHeader.ToString() +
                         stringBuilderData.ToString() + "\r\n").Replace("+\"\"", "");
                code += "byteData = http.GetResponseBody ()\r\n";
                code += "textData = UTF8到文本 (byteData)\r\n";
                code += "byteData = GZIP_解压 (byteData)\r\n";
                code += "textData = 选择 (UTF8到文本 (byteData) = “”, textData, UTF8到文本 (byteData))\r\n";
                code += "返回 (textData)\r\n";
            }

            return code;
        }

        /// <summary>
        /// 子程序定义、参数定义、变量定义、
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private StringBuilder AutoGenerateRequest(Session session)
        {
            string host = session.host;
            string queryUrl = session.url;
            string queryData = string.Empty;
            bool isHttps = session.isHTTPS;
            string funName = String.Empty;
            string method = session.RequestMethod;
            StringBuilder stringBuilder = new StringBuilder();

            //判断是否是GET查询
            if (queryUrl.Contains("?"))
            {
                string[] pathAndQuery = session.url.Split(new string[] { "?" }, StringSplitOptions.None);
                queryUrl = pathAndQuery[0];
                queryData = pathAndQuery[1];
            }
            else
            {
                queryUrl = session.url;
            }

            string[] funNames = queryUrl.Split('/');
            int index = funNames.Length;
            funName = RemoveSpecialCharacters(funNames[index - 1]);
            stringBuilder.AppendFormat(".子程序 {0}, 文本型, 公开, 备注\r\n", funName);

            string[] stringHangs = queryData.Split('&');
            if (queryData.Contains("="))
            {
                queryData = String.Empty;
                foreach (var stringHang in stringHangs)
                {
                    string[] pairs = stringHang.Split('=');
                    if (pairs.Length == 2)
                    {
                        string key = Uri.UnescapeDataString(pairs[0]);
                        string value = Uri.UnescapeDataString(pairs[1]);
                        stringBuilder.AppendFormat(".参数 {0}, 文本型, 可空, {1}\r\n", key, value);
                        if (pairs[1].Contains("%"))
                        {
                            queryData += string.Format("{0}=\"+URLEncodeUtf8 ({1})+\"", key, key);
                        }
                        else
                        {
                            if (queryData == String.Empty)
                            {
                                queryData += string.Format("{0}=\"+{1}+\"", key, key);
                            }
                            else
                            {
                                queryData += "&" + string.Format("{0}=\"+{1}+\"", key, key);
                            }
                        }
                    }
                }
            }

            stringBuilder.AppendFormat(".局部变量 {0}, {1}, , \"\", {2}\r\n", "http",
                this._module ? "WinHttpR" : "WinHttpW", "");
            stringBuilder.AppendFormat(".局部变量 {0}, 文本型, , \"\", {1}\r\n", "url", "");
            stringBuilder.AppendFormat(".局部变量 {0}, 文本型, , \"\", {1}\r\n", "method", "");
            stringBuilder.AppendFormat(".局部变量 {0}, 字节集, , \"\", {1}\r\n", "byteData", "");
            stringBuilder.AppendFormat(".局部变量 {0}, 文本型, , \"\", {1}\r\n", "textData", "");
            stringBuilder.AppendFormat(".局部变量 {0}, 文本型, , \"\", {1}\r\n", "postData", "");
            stringBuilder.AppendFormat(".局部变量 {0}, zyJsonValue, , \"\", {1}\r\n", "Json", "");

            stringBuilder.AppendFormat("method = \"{0}\"\r\n", method);

            if (queryData == String.Empty)
            {
                stringBuilder.AppendFormat("url = \"{0}{1}\"\r\n", isHttps ? "https://" : "http://", queryUrl);
            }
            else
            {
                stringBuilder.AppendFormat("url = \"{0}{1}?{2}\"\r\n", isHttps ? "https://" : "http://", queryUrl,
                    queryData);
            }

            stringBuilder.AppendLine("http.Open (method, url)");
            return stringBuilder;
        }

        /// <summary>
        /// 协议头设置
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private StringBuilder AutoGenerateHeader(Session session)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var requestHeader in session.RequestHeaders)
            {
                switch (requestHeader.Name)
                {
                    case "Cookie":
                        stringBuilder.AppendFormat("' http.SetCookie (“{0}”, “{1}”)\r\n", requestHeader.Name,
                            requestHeader.Value.Replace("\"", "\" + #引号 + \""));
                        break;
                    case "Content-Length":

                        break;
                    case "Accept-Encoding":
                        stringBuilder.AppendFormat("' http.SetRequestHeader (“{0}”, “{1}”)\r\n", requestHeader.Name,
                            requestHeader.Value.Replace("\"", "\" + #引号 + \""));
                        break;
                    default:
                        stringBuilder.AppendFormat("http.SetRequestHeader (“{0}”, “{1}”)\r\n", requestHeader.Name,
                            requestHeader.Value.Replace("\"", "\" + #引号 + \""));
                        break;
                }
            }

            return stringBuilder;
        }

        /// <summary>
        /// 请求数据包解析
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private StringBuilder AutoGenerateData(Session session)
        {
            string type = String.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            string textData = session.GetRequestBodyAsString();
            type = ProcessDataPacket(textData);
            stringBuilder.Append(this._stringBuilderLs.ToString());
            this._stringBuilderLs.Clear();
            if (textData != String.Empty && textData.Length < 1000)
            {
                switch (type)
                {
                    case "json":
                        stringBuilder.AppendLine("postData = Json.到文本 (, , , )");
                        break;
                    case "query":

                        break;
                    default:
                        stringBuilder.AppendLine("'无法识别该参数格式，请保存此包为SAZ后发给97348461@qq.com");
                        textData.Replace("\r", "");
                        stringBuilder.AppendLine($"postData=“{textData.Replace("\"", "”+ #引号 +“")}");
                        break;
                }

                stringBuilder.AppendLine("http.Send (postData)");
            }
            else
            {
                if (textData.Length > 1000)
                {
                    stringBuilder.AppendLine("'数据格式太长，我要偷懒咯");
                    stringBuilder.AppendLine($"postData=“{textData.Replace("\"", "”+ #引号 +“")}”");
                }
                else
                {
                    stringBuilder.AppendLine("'数据为Null，我要偷懒咯");
                    stringBuilder.AppendLine("http.Send ()");
                }
            }


            return stringBuilder;
        }

        /// <summary>
        /// 复制到剪辑板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(uiTextBox1.Text);
        }

        /// <summary>
        /// 获取Fiddler列备注
        /// </summary>
        /// <returns></returns>
        private int GetCommentsIndex()
        {
            int count = FiddlerApplication.UI.lvSessions.Columns.Count;
            int index = -1;
            for (int i = 0; i < count; i++)
            {
                if (FiddlerApplication.UI.lvSessions.Columns[i].Text == "Comments")
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 去掉字符串中的特殊符号，只保留字母和数字
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>处理后的字符串</returns>
        private string RemoveSpecialCharacters(string input)
        {
            // 正则表达式模式，匹配非字母和非数字的字符
            string pattern = "[^a-zA-Z0-9]";

            // 使用正则表达式替换匹配的字符
            string cleanedString = Regex.Replace(input, pattern, "");

            return cleanedString;
        }

        /// <summary>
        /// 遍历JSON对象并根据值的不同类型输出自定义代码
        /// </summary>
        /// <param name="token">当前JSON token</param>
        /// <param name="currentPath">当前路径</param>
        private void ParseJson(JToken token, string currentPath)
        {
            if (token is JObject)
            {
                foreach (var property in (JObject)token)
                {
                    ParseJson(property.Value, currentPath + property.Key + ".");
                }
            }
            else if (token is JArray)
            {
                JArray array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    ParseJson(array[i], $"{currentPath}[{i}].");
                }
            }
            else
            {
                OutputCustomCode(currentPath.TrimEnd('.'), token);
            }
        }

        /// <summary>
        /// 根据值的不同类型输出自定义代码
        /// </summary>
        /// <param name="path">当前路径</param>
        /// <param name="token">当前JSON token</param>
        private void OutputCustomCode(string path, JToken token)
        {
            string keyName = path.ToString();
            if (keyName.Contains("."))
            {
                string[] lsKeyName = keyName.Split('.');
                keyName = lsKeyName[lsKeyName.Length - 1];
            }

            //清除特殊字符
            keyName = RemoveSpecialCharacters(keyName);

            switch (token.Type)
            {
                case JTokenType.String:
                    this._stringBuilderLs.AppendLine($".参数 {keyName}, 文本型, 可空, {token}");
                    this._stringBuilderLs.AppendLine($"Json.置文本 (“{path}”, {keyName})");
                    break;
                case JTokenType.Integer:
                    this._stringBuilderLs.AppendLine($".参数 {keyName}, 整数型, 可空, {token}");
                    this._stringBuilderLs.AppendLine($"Json.置文本 (“{path}”, {keyName})");
                    break;
                case JTokenType.Float:
                    this._stringBuilderLs.AppendLine($".参数 {keyName}, 小数型, 可空, {token}");
                    this._stringBuilderLs.AppendLine($"Json.置文本 (“{path}”, {keyName})");
                    break;
                case JTokenType.Boolean:
                    this._stringBuilderLs.AppendLine($".参数 {keyName}, 逻辑型, 可空, {token}");
                    this._stringBuilderLs.AppendLine($"Json.置文本 (“{path}”, {keyName})");
                    break;
                default:
                    this._stringBuilderLs.AppendLine($".参数 {keyName}, 懵逼型, 可空, {token}");
                    this._stringBuilderLs.AppendLine($"Json.置文本 (“{path}”, {keyName})");
                    break;
            }
        }

        /// <summary>
        /// 识别数据包的格式
        /// </summary>
        /// <param name="data">数据包</param>
        /// <returns>数据包的格式类型</returns>
        private string IdentifyDataPacketFormat(string data)
        {
            // 尝试解析为JSON对象
            try
            {
                JToken.Parse(data);
                return "json";
            }
            catch
            {
                // 忽略解析错误
            }

            // 尝试解析为查询字符串
            if (data.Contains("=") && data.Contains("&"))
            {
                return "query";
            }

            // 尝试解析为查询字符串
            if (data.Contains("="))
            {
                return "query";
            }

            return "unknown";
        }

        /// <summary>
        /// 处理请求数据包的函数
        /// </summary>
        /// <param name="data">数据包</param>
        private string ProcessDataPacket(string data)
        {
            string format = IdentifyDataPacketFormat(data);

            if (format == "json")
            {
                Console.WriteLine("处理JSON数据包");
                JObject jsonObj = JObject.Parse(data);
                ParseJson(jsonObj, "");
                return "json";
            }
            else if (format == "query")
            {
                Console.WriteLine("处理查询字符串数据包");
                StringBuilder stringBuilder = new StringBuilder();
                var queryDict = HttpUtility.ParseQueryString(data);
                stringBuilder.Append("postData =“");
                foreach (var key in queryDict.AllKeys)
                {
                    this._stringBuilderLs.AppendLine($".参数 {key}, 文本型, 可空, {queryDict[key]}");
                    if (HttpUtility.UrlEncode(queryDict[key], Encoding.UTF8).Contains("%"))
                    {
                        stringBuilder.Append($"{key}=”+ URLEncodeUtf8 ({key})+“&");
                    }
                    else
                    {
                        stringBuilder.Append($"{key}=”+ {key}+“&");
                    }
                }

                stringBuilder.Remove(stringBuilder.Length - 3, 3);
                this._stringBuilderLs.AppendLine(stringBuilder.ToString());
                return "query";
            }
            else
            {
                Console.WriteLine("未知数据包格式，请将此包保存为SAZ后发邮件:97348461@qq.com");
            }

            return "no";
        }

        /// <summary>
        /// 模块代码选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiRadioButton1WinHttpR_Click(object sender, EventArgs e)
        {
            uiRadioButton1WinHttpR.Checked = uiRadioButton1WinHttpR.Checked;
            this._module = uiRadioButton1WinHttpR.Checked;
        }

        /// <summary>
        /// 模块代码选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiRadioButton2WinHttpW_Click(object sender, EventArgs e)
        {
            uiRadioButton1WinHttpR.Checked = uiRadioButton1WinHttpR.Checked;
            this._module = uiRadioButton1WinHttpR.Checked;
        }

        /// <summary>
        /// 手动调试代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3CodeDebuger_Click(object sender, EventArgs e)
        {
            //todo 调试代码
        }
    }
}