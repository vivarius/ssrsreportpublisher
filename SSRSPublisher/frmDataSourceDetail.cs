using System.Windows.Forms;

namespace SSRSPublisher
{
    public partial class frmDataSourceDetail : Form
    {

        public string DataSourceName { get; set; }
        public string SQLServer { get; set; }
        public string DBName { get; set; }

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
