using SocketLite.Forms;
using SocketLite.Net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketLite.Server
{
    public partial class MainForm : ServerForm
    {
        NotifyIcon notifyIcon1 = new NotifyIcon();
        private static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private AsyncTcpServer tcpServer;
        private AsyncContext context;

        public MainForm()
        {
            InitializeComponent();

            notifyIcon1.Text = Text;
            notifyIcon1.Icon = Icon;
            notifyIcon1.DoubleClick += OnNotifyIconDoubleClicked;
            SizeChanged += OnSizeChanged;
            ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, OnProgramStarted, null, -1, false);
            WindowState = FormWindowState.Normal;

            context = new AsyncContext(new RichTextBoxLogger(rtbLog, true));
            context.Logger.Clear();
        }

        #region NotifyIcon
        void OnSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                Visible = false;
            }
        }

        void OnNotifyIconDoubleClicked(object sender, EventArgs e)
        {
            Visible = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        void OnProgramStarted(object state, bool timeout)
        {
            notifyIcon1.ShowBalloonTip(2000, "您好", "我在这呢！", ToolTipIcon.Info);
            Show();
            WindowState = FormWindowState.Normal;
        }
        #endregion

        #region FormEvents
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetButtonEnabled(false);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialog = MessageBox.Show("确定关闭服务？", "确认", MessageBoxButtons.OKCancel);
            if (dialog == DialogResult.OK)
            {
                StopServer();
            }
            e.Cancel = dialog == DialogResult.Cancel;
        }
        #endregion

        #region TcpServerEvents
        private void TcpServer_ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            context.Logger.WriteLog("{0}已连接！", e.TcpClient.Client.RemoteEndPoint);
        }

        private void TcpServer_ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            context.Logger.WriteLog("{0}已断开连接！", e.TcpClient.Client.RemoteEndPoint);
        }

        private void TcpServer_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            if (e.Datagram != null && e.Datagram.Length > 0)
            {
                try
                {
                    var message = Encoding.UTF8.GetString(e.Datagram);
                    var request = Utils.Deserialize<RequestInfo>(message);
                    if (request.Handler == "Connect")
                    {
                        tcpServer.Send(e.TcpClient, request, "连接成功！");
                        return;
                    }

                    var logger = new TcpServerLogger(rtbLog, request, tcpServer, e.TcpClient);
                    var ctxRequest = new RequestContext(logger);
                    ctxRequest.Request = request;
                    
                    Utils.ExecuteAsync(ctxRequest, e1 =>
                    {
                        var ctx = e1.Argument as RequestContext;
                        if (ctx == null)
                            return;

                        var handler = RequestHandler.GetHandler(ctx);
                        if (handler == null)
                        {
                            ctx.Logger.WriteLog("不支持{0}的处理者！", ctx.Request.Handler);
                            return;
                        }

                        e1.Result = handler.Execute();
                    },
                    e2 =>
                    {
                        if (e2.Result != null)
                            tcpServer.Send(e.TcpClient, Utils.Serialize(e2.Result));
                    });
                }
                catch (Exception ex)
                {
                    context.Logger.WriteLog("{0}接收异常，{1}", e.TcpClient.Client.RemoteEndPoint, ex.Message);
                }
            }
            Application.DoEvents();
        }
        #endregion

        #region ButtonEvents
        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (tcpServer == null)
                {
                    tcpServer = new AsyncTcpServer(ConfigHelper.AppSettings<int>("ServerPort"));
                    tcpServer.Encoding = Encoding.UTF8;
                    tcpServer.ClientConnected += TcpServer_ClientConnected;
                    tcpServer.ClientDisconnected += TcpServer_ClientDisconnected;
                    tcpServer.DatagramReceived += TcpServer_DatagramReceived;
                }

                tcpServer.Start();
                context.Logger.WriteLog("服务已启动！");
                SetButtonEnabled(true);
            }
            catch (Exception ex)
            {
                context.Logger.WriteLog("服务启动失败：" + ex.ToString());
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopServer();
            context.Logger.WriteLog("服务已停止！");
            SetButtonEnabled(false);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            context.Logger.Clear();
        }
        #endregion

        #region Private
        private void SetButtonEnabled(bool started)
        {
            btnStart.Enabled = !started;
            btnStop.Enabled = started;
        }

        private void StopServer()
        {
            if (tcpServer != null)
                tcpServer.Stop();
        }
        #endregion
    }
}
