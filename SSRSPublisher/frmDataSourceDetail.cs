using System.Windows.Forms;

namespace SSRSPublisher
{
    public partial class frmDataSourceDetail : Form
    {

        public string DataSourceName
        {
            get { return txNom.Text.Trim(); }
            set { txNom.Text = value; }
        }

        public string SQLServer
        {
            get { return txServer.Text.Trim(); }
            set { txServer.Text = value; }
        }

        public string DBName
        {
            get { return txDBName.Text.Trim(); }
            set { txDBName.Text = value; }
        }

        public frmDataSourceDetail()
        {
            InitializeComponent();
        }

        private void btOK_Click(object sender, System.EventArgs e)
        {
            string dataSourceName = txNom.Text.Trim();
            string sqlServer = txServer.Text.Trim();
            string dbName = txDBName.Text.Trim();

            if (dataSourceName == string.Empty || sqlServer == string.Empty || dbName == string.Empty)
            {
                DialogResult = DialogResult.None;
                return;
            }

            DataSourceName = dataSourceName;
            SQLServer = sqlServer;
            DBName = dbName;
        }
    }
}
