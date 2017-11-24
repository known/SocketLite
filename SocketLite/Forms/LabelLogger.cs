using SocketLite.Log;
using System.Windows.Forms;

namespace SocketLite.Forms
{
    public class LabelLogger : Logger
    {
        private Label lblLog;

        public LabelLogger(Label lblLog) : base(false)
        {
            this.lblLog = lblLog;
        }

        public override void Clear()
        {
            lblLog.Text = string.Empty;
        }

        protected override void WriteLine(string message)
        {
            lblLog.Text = message;
        }
    }
}
