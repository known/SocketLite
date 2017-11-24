using System.Windows.Forms;

namespace SocketLite.Forms
{
    public class FormBase : Form
    {
        public FormBase()
        {
            CheckForIllegalCrossThreadCalls = false;
        }
    }
}
