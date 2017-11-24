using SocketLite.Forms;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SocketLite.Client
{
    public partial class MainForm : ClientForm
    {
        NotifyIcon notifyIcon1 = new NotifyIcon();

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
            SetEnabledForm(false);
            var context = new AsyncContext(new RichTextBoxLogger(rtbLog));
            Utils.ExecuteAsync(context, e1 =>
            {
                var ctx = e1.Argument as AsyncContext;
                if (ctx == null)
                    return;

                ctx.Logger.WriteLog("正在连接服务器......");
                ConnectServer();
                SendRequest("Connect");
            },
            e2 =>
            {
            });
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region ButtonEvents
        private void BtnSend_Click(object sender, EventArgs e)
        {
            SetEnabledForm(false);
            SendRequest(txtHandler.Text, txtAction.Text, txtData.Text);
        }
        #endregion

        #region Private
        private void SetEnabledForm(bool enabled)
        {
            pnlForm.Enabled = enabled;
        }
        #endregion

        #region Protected
        protected override void ConnectSuccess()
        {
            SetEnabledForm(true);
        }

        protected override void ResponseFinished()
        {
            SetEnabledForm(true);
        }
        #endregion
    }
}
