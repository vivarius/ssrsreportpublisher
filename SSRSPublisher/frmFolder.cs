using System;
using System.Windows.Forms;

namespace SSRSPublisher
{
    public partial class frmFolder : Form
    {
        public frmFolder()
        {
            InitializeComponent();
        }

        public string FolderName { get; set; }

        private void btOK_Click(object sender, EventArgs e)
        {
            string folderName = txNom.Text.Trim();

            if (folderName == string.Empty)
            {
                DialogResult = DialogResult.None;
                return;
            }

            FolderName = folderName;
        }
    }
}
