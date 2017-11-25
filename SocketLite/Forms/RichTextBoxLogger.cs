using SocketLite.Logs;
using System;
using System.Windows.Forms;

namespace SocketLite.Forms
{
    public class RichTextBoxLogger : Logger
    {
        private RichTextBox rtbLog;

        public RichTextBoxLogger(RichTextBox rtbLog) : this(rtbLog, false) { }

        public RichTextBoxLogger(RichTextBox rtbLog, bool addTime) : base(addTime)
        {
            this.rtbLog = rtbLog;
        }

        public override void Clear()
        {
            rtbLog.Clear();
        }

        protected override void WriteLine(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                rtbLog.AppendText(message);
                rtbLog.AppendText(Environment.NewLine);
            }
        }
    }
}
