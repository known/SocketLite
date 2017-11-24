using SocketLite.Forms;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SocketLite.Client
{
    public partial class ConfigForm : ClientForm
    {
        public ConfigForm()
        {
            InitializeComponent();
            lblInfo.Text = string.Empty;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            var context = new AsyncContext(new LabelLogger(lblInfo));
            var sIP = txtIP.Text.Trim();
            var sPort = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(sIP))
            {
                context.Logger.WriteLog("请输入服务器IP！");
                txtIP.Select();
                return;
            }

            if (string.IsNullOrEmpty(sPort))
            {
                context.Logger.WriteLog("请输入服务器端口！");
                txtPort.Select();
                return;
            }

            var rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (!rx.IsMatch(sIP))
            {
                context.Logger.WriteLog("服务器IP地址不合法！");
                txtIP.Select();
                return;
            }

            rx = new Regex(@"^[0-9]\d*$");
            if (!rx.IsMatch(sPort))
            {
                context.Logger.WriteLog("服务器端口不正确，请输入数字！");
                txtPort.Select();
                return;
            }

            context.Params["ServerIP"] = sIP;
            context.Params["ServerPort"] = sPort;
            context.Logger.Clear();
            btnOK.Enabled = btnClose.Enabled = false;

            Utils.ExecuteAsync(context, e1 =>
            {
                var ctx = e1.Argument as AsyncContext;
                if (ctx == null)
                    return;

                ctx.Logger.WriteLog("正在连接服务器......");
                try
                {
                    var ip = ctx.Param<string>("ServerIP");
                    var port = ctx.Param<int>("ServerPort");
                    ConnectServer(ip, port);
                    //SendRequest<string>(RequestCode.ConfigInfo);
                    ctx.Logger.WriteLog("连接成功，正在保存配置信息......");
                    Config.Save(sIP, sPort);
                    ctx.Logger.WriteLog("配置保存成功，正在重新启动程序......");

                    e1.Result = "1";
                }
                catch (Exception ex)
                {
                    ctx.Logger.WriteLog("连接失败，" + ex.Message);
                    e1.Result = "0";
                }
            },
            e2 =>
            {
                btnOK.Enabled = btnClose.Enabled = true;
                var result = e2.Result.ToString();
                if (result == "1")
                {
                     Application.Restart();
                }
            });
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
