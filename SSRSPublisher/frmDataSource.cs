using System;
using System.Windows.Forms;
using SSRSPublisher.ReportService2005;

namespace SSRSPublisher
{
    public partial class frmDataSource : Form
    {
        #region ctor
        public frmDataSource()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void frmDataSource_Load(object sender, EventArgs e)
        {

        }

        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (((ItemTypeEnum)(treeView1.SelectedNode.Tag)) == ItemTypeEnum.DataSource)
                    SelectedNode = new FileNameAndPath
                    {
                        Path = treeView1.SelectedNode.FullPath.Replace(treeView1.Nodes[0].Text, string.Empty),
                        Name = treeView1.SelectedNode.Text
                    };
                else
                {
                    throw new Exception("Please select a DataSource");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                DialogResult = DialogResult.None;
            }
        }
        #endregion

        #region Properties
        public FileNameAndPath SelectedNode { get; set; }
        #endregion

        #region Methods
        public void FillTreeview(ReportingService2005 reportService2005)
        {
            Cursor.Current = Cursors.WaitCursor;
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(TreeViewHandling.GetFolderAsNodes(reportService2005, true));
            treeView1.EndUpdate();
            treeView1.ExpandAll();
            Cursor.Current = Cursors.Arrow;
        }
        #endregion

    }
}
