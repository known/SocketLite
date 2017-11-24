using SocketLite.Forms;
using System;

namespace SocketLite.Client
{
    public partial class LoadingForm : ClientForm
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            var context = new AsyncContext(new LabelLogger(lblInfo));
            Utils.ExecuteAsync(context, e1 =>
            {
                var ctx = e1.Argument as AsyncContext;
                if (ctx == null)
                    return;

                ctx.Logger.WriteLog("正在连接服务器......");
                try
                {
                    ConnectServer("192.168.0.187", 7878);
                    //SendRequest<string>(RequestCode.ConfigInfo);
                    ctx.Logger.WriteLog("连接成功，正在启动程序......");

                    e1.Result = "1";
                }
                catch (Exception ex)
                {
                    for (int i = 5; i > 0; i--)
                    {
                        ctx.Logger.WriteLog("连接失败：" + ex.Message + "  " + i + "秒后将重新连接。");
                    }

                    e1.Result = "0";
                }
            },
            e2 =>
            {
                var result = e2.Result.ToString();
                if (result == "1")
                {
                    Hide();
                    new MainForm().Show();
                }
            });
        }
    }
}
