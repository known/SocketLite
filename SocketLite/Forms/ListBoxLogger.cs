using SocketLite.Log;
using System;
using System.Windows.Forms;

namespace SocketLite.Forms
{
    public class ListBoxLogger : Logger
    {
        private ListBox lbLog;

        public ListBoxLogger(ListBox lbLog) : this(lbLog, false) { }

        public ListBoxLogger(ListBox lbLog, bool addTime) : base(addTime)
        {
            this.lbLog = lbLog;
        }

        public override void Clear()
        {
            lbLog.Items.Clear();
        }

        protected override void WriteLine(string message)
        {
            var messages = message.Split(Environment.NewLine.ToCharArray());
            foreach (var item in messages)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    lbLog.Items.Add(item);
                }
            }
        }
    }
}
