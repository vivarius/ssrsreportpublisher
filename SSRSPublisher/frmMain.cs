using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SSRSPublisher.ReportService2005;

namespace SSRSPublisher
{
    public partial class frmMain
    {
        #region Properties
        public ReportServerProperties ReportServerSource { get; set; }
        public ReportServerProperties ReportServerDestination { get; set; }
        //private BackgroundWorker backgroundWorker = new BackgroundWorker();
        #endregion

        #region ctor
        public frmMain()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += backgroundWorker_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
        }
        #endregion

        #region Events

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblBWInfo.Text = (e.ProgressPercentage + "%");
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadTreeViewPanelsFromSrv();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tvReportServerDestination.SelectedNode == null)
            {
                MessageBox.Show(@"Merci de sélectionner le dossier destination");
                return;
            }

            FileNameAndPath dataSourceMap = null;
            using (var frmDataSource = new frmDataSource())
            {
                frmDataSource.FillTreeview(ReportServerDestination.ReportsServerInstance);
                if (frmDataSource.ShowDialog() == DialogResult.OK)
                    dataSourceMap = frmDataSource.SelectedNode;
            }

            Cursor.Current = Cursors.WaitCursor;
            if (dataSourceMap == null)
                return;

            var checkedNodes = TreeViewHandling.GetCheckedNodes(tvReportServerSource.Nodes);

            pbTransfer.Visible = true;
            pbTransfer.Maximum = checkedNodes.Count(node => node.Checked);

            int i = 0;

            foreach (TreeNode checkedNode in checkedNodes.Where(node => node.Checked))
            {
                switch (((ItemTypeEnum)(checkedNode.Tag)))
                {
                    case ItemTypeEnum.Report:
                        pbTransfer.Value = (i < pbTransfer.Maximum) ? ++i : i;
                        ReportServerDestination.DeployReport(ReportServerSource.ReportsServerInstance,
                                                             checkedNode.Text,
                                                             checkedNode.FullPath.Replace(tvReportServerSource.Nodes[0].Text, string.Empty),
                                                             tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty),
                                                             dataSourceMap.Name,
                                                             dataSourceMap.Path);
                        break;
                    case ItemTypeEnum.Folder:
                        if (ReportServerDestination.CheckItemExist(ItemTypeEnum.Folder,
                                                                tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                checkedNode.Text))
                        {
                            ReportServerDestination.CreateFolder(tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty), checkedNode.Text);
                        }
                        break;
                }
            }

            pbTransfer.Visible = false;
            Cursor.Current = Cursors.Arrow;
        }

        private void ReportServer_Load(object sender, EventArgs e)
        {
            var list = new Projects().ListProjects;

            foreach (var project in list)
                cmbProject.Items.Add(project.Server1 + " = " + project.Server2);

            cmbProject.Tag = list;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            groupBox1.Height = Height - btnOK.Height + 100;
        }

        private void tvReportServerSource_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewHandling.CheckNodes(e.Node);
        }

        private void btLoadServers_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void creationDossierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateFolder(sender);
        }

        private void downloadDossierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UploadReport(sender);
        }

        private void creationDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDatasource(sender);
        }

        private void suppRapportDossierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure for item deletion?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                DeleteItem((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
        }

        private void tvReportServerDestination_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            txItemPath.Text = e.Node.FullPath.Replace(e.Node.TreeView.Nodes[0].Text, string.Empty);
        }

        #endregion

        #region Methods
        private void LoadTreeViewPanelsFromSrv()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                var cmbBoxValue = new string[] { };
                var tagValue = new List<Project>();
                var selectedProject = new Project();
                if (cmbProject.InvokeRequired)
                {
                    cmbProject.Invoke(new MethodInvoker(delegate
                                                            {
                                                                cmbBoxValue = ((string)cmbProject.SelectedItem).Split('=');
                                                            }));
                    cmbProject.Invoke(new MethodInvoker(delegate
                                                            {
                                                                tagValue = cmbProject.Tag as List<Project>;
                                                            }));
                }

                foreach (var project in
                    tagValue.Where(project => project.Server1 == cmbBoxValue[0].Trim() &&
                                   project.Server2 == cmbBoxValue[1].Trim()))
                {
                    selectedProject = project;
                    break;
                }

                //var selectedProject = (from tv in tagValue
                //                             where tv.Server1 == cmbBoxValue[0].Trim() &&
                //                                   tv.Server2 == cmbBoxValue[1].Trim()
                //                             select tv) as List<Project>;

                if (selectedProject != null)
                {
                    ReportServerSource = new ReportServerProperties(selectedProject.URL1);
                    ReportServerDestination = new ReportServerProperties(selectedProject.URL2);
                    RefreshSourceTreeView(tvReportServerSource, ReportServerSource);
                    RefreshSourceTreeView(tvReportServerDestination, ReportServerDestination);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            Cursor.Current = Cursors.Arrow;
        }

        private static void RefreshSourceTreeView(TreeView treeView, ReportServerProperties reportingService2005)
        {
            if (treeView.InvokeRequired)
            {
                treeView.Invoke(new MethodInvoker(treeView.BeginUpdate));
                treeView.Invoke(new MethodInvoker(() => treeView.Nodes.Clear()));
                treeView.Invoke(new MethodInvoker(() => treeView.Nodes.Add(TreeViewHandling.GetFolderAsNodes(reportingService2005.ReportsServerInstance, true))));
                treeView.Invoke(new MethodInvoker(treeView.EndUpdate));
                treeView.Tag = reportingService2005;
            }
            else
            {
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                treeView.Nodes.Add(TreeViewHandling.GetFolderAsNodes(reportingService2005.ReportsServerInstance, true));
                treeView.EndUpdate();
                treeView.Tag = reportingService2005;
            }
        }

        private static void DeleteItem(TreeView treeView)
        {
            if (!treeView.SelectedNode.TreeView.HasChildren)
                if (((ReportServerProperties)(treeView.Tag)).DeleteItem((ItemTypeEnum)(treeView.SelectedNode.Tag),
                                                                     treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(treeView.SelectedNode.Text, string.Empty),
                                                                     treeView.SelectedNode.Text))
                {
                    treeView.Nodes.Remove(treeView.SelectedNode);
                }
        }

        private void CreateFolder(object sender)
        {
            var treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.Folder)
            {
                using (var frmFolder = new frmFolder())
                {
                    if (frmFolder.ShowDialog() == DialogResult.OK)
                    {
                        if (((ReportServerProperties)(treeView.Tag)).CreateFolder(treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                                  frmFolder.FolderName))
                        {
                            treeView.SelectedNode.Nodes.Add(treeView.SelectedNode.FullPath + @"/" + frmFolder.FolderName, frmFolder.FolderName, 0);
                            MessageBox.Show(@"Folder created succesfully");
                        }
                        else
                        {
                            MessageBox.Show(@"Folder NOT created");
                        }
                    }
                }
            }
            else
                MessageBox.Show(@"Please choose a folder");
        }

        private void UploadReport(object sender)
        {
            var treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.Folder)
            {
                FileNameAndPath dataSourceMap = null;
                using (var frmDataSource = new frmDataSource())
                {
                    frmDataSource.FillTreeview(ReportServerDestination.ReportsServerInstance);
                    if (frmDataSource.ShowDialog() == DialogResult.OK)
                        dataSourceMap = frmDataSource.SelectedNode;
                }

                openFileDialog1.Filter = @"*.*|*.*";
                openFileDialog1.InitialDirectory = @"c:\";
                openFileDialog1.Multiselect = false;
                openFileDialog1.ShowDialog();
                if (File.Exists(openFileDialog1.FileName))
                {
                    if (((ReportServerProperties)(treeView.Tag)).DeployReport(
                            ((ReportServerProperties)(treeView.Tag)).ReportsServerInstance,
                            openFileDialog1.FileName,
                            txItemPath.Text,
                            dataSourceMap.Name,
                            dataSourceMap.Path))
                    {
                        var ReportNode = new TreeNode(openFileDialog1.FileName)
                                             {
                                                 ImageIndex = 1,
                                                 Tag = openFileDialog1.FileName,
                                                 Name = openFileDialog1.FileName,
                                                 SelectedImageIndex = 1
                                             };

                        treeView.SelectedNode.Nodes.Add(ReportNode);
                    }
                    else
                        MessageBox.Show(@"The Report wasn't deployed");
                }
                else
                {
                    MessageBox.Show(@"Please choose a file");
                }
            }
            else
                MessageBox.Show(@"Please choose a folder");
        }

        private void HandleDatasource(object sender)
        {
            TreeView treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.DataSource)
            {
                EditDataSource(sender);
            }
            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.Folder)
            {
                CreateDatasource(sender);
            }
        }

        private void EditDataSource(object sender)
        {
            TreeView treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) != ItemTypeEnum.DataSource)
            {
                MessageBox.Show("Please choose a Datasource");
                return;
            }

            ReportServerProperties reportServerProperties = new ReportServerProperties(((ReportServerProperties)(treeView.Tag)).ReportsServerInstance.Url);
            //DataSource dataSource = reportServerProperties.GetDataSource(
            //    treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
            //    treeView.SelectedNode.Text);

            DataSourceDefinition dataSourceDefinition = reportServerProperties.GetDataSourceDefinition(treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"));

            using (var frmDataSourceDetail = new frmDataSourceDetail())
            {
                string[] connectionString = dataSourceDefinition.ConnectString.Split(';');

                foreach (var item in connectionString)
                {
                    string[] itemDict = item.Split('=');
                    foreach (var sItem in itemDict)
                    {
                        if (sItem == "Data Source")
                            frmDataSourceDetail.DataSourceName = sItem[1].ToString();
                    }
                }

                frmDataSourceDetail.DataSourceName = dataSourceDefinition.ConnectString;
                frmDataSourceDetail.SQLServer = dataSourceDefinition.ConnectString;
                frmDataSourceDetail.DataSourceName = dataSourceDefinition.ConnectString;

                if (frmDataSourceDetail.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(ReportServerDestination.CreateDataSource(frmDataSourceDetail.DataSourceName,
                                                                             tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                             frmDataSourceDetail.SQLServer,
                                                                             frmDataSourceDetail.DBName)
                                        ? "DataSource modified succesfully"
                                        : "DataSource WASN'T modified");
                }
            }
        }

        private void CreateDatasource(object sender)
        {
            TreeView treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.Folder)
            {
                using (var frmDataSourceDetail = new frmDataSourceDetail())
                {
                    if (frmDataSourceDetail.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(ReportServerDestination.CreateDataSource(frmDataSourceDetail.DataSourceName,
                                                                                 tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                                 frmDataSourceDetail.SQLServer,
                                                                                 frmDataSourceDetail.DBName)
                                            ? "DataSource created succesfully"
                                            : "DataSource WASN'T created");
                    }
                }
            }
            else
                MessageBox.Show(@"Please choose a folder");
        }
        #endregion
    }
}

//private void RefreshDestinationTreeView()
//{
//    if (tvReportServerDestination.InvokeRequired)
//    {
//        tvReportServerDestination.Invoke(new MethodInvoker(() => tvReportServerDestination.BeginUpdate()));
//        tvReportServerDestination.Invoke(new MethodInvoker(() => tvReportServerDestination.Nodes.Clear()));
//        tvReportServerDestination.Invoke(new MethodInvoker(() => tvReportServerDestination.Nodes.Add(TreeViewHandling.GetFolderAsNodes(ReportServerDestination.ReportsServerInstance))));
//        tvReportServerDestination.Invoke(new MethodInvoker(() => tvReportServerDestination.EndUpdate()));
//    }
//    else
//    {
//        tvReportServerDestination.BeginUpdate();
//        tvReportServerDestination.Nodes.Clear();
//        tvReportServerDestination.Nodes.Add(TreeViewHandling.GetFolderAsNodes(ReportServerDestination.ReportsServerInstance));
//        tvReportServerDestination.EndUpdate();
//    }
//}

//private void RefreshSourceTreeView()
//{
//    if (tvReportServerDestination.InvokeRequired)
//    {
//        tvReportServerSource.Invoke(new MethodInvoker(() => tvReportServerSource.BeginUpdate()));
//        tvReportServerSource.Invoke(new MethodInvoker(() => tvReportServerSource.Nodes.Clear()));
//        tvReportServerSource.Invoke(new MethodInvoker(() => tvReportServerSource.Nodes.Add(TreeViewHandling.GetFolderAsNodes(ReportServerSource.ReportsServerInstance))));
//        tvReportServerSource.Invoke(new MethodInvoker(() => tvReportServerSource.EndUpdate()));
//    }
//    else
//    {
//        tvReportServerSource.BeginUpdate();
//        tvReportServerSource.Nodes.Clear();
//        tvReportServerSource.Nodes.Add(TreeViewHandling.GetFolderAsNodes(ReportServerSource.ReportsServerInstance));
//        tvReportServerSource.EndUpdate();
//    }
//}