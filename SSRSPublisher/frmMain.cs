using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        private void tvReportServerSource_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((ItemTypeEnum)e.Node.Tag == ItemTypeEnum.Report)
                PreviewReport(e.Node.TreeView);

            if ((ItemTypeEnum)e.Node.Tag == ItemTypeEnum.DataSource)
                EditDataSource(e.Node.TreeView);


        }

        private void tvReportServerDestination_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((ItemTypeEnum)e.Node.Tag == ItemTypeEnum.Report)
                PreviewReport(e.Node.TreeView);

            if ((ItemTypeEnum)e.Node.Tag == ItemTypeEnum.DataSource)
                EditDataSource(e.Node.TreeView);
        }

        private void btSettings_Click(object sender, EventArgs e)
        {
            new frmServers().ShowDialog();
            LoadProjectList();
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblBWInfo.Visible = true;
            lblBWInfo.Text = (e.ProgressPercentage + "%");
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
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
            TransferItems();
        }

        private void ReportServer_Load(object sender, EventArgs e)
        {
            LoadProjectList();
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
            Cursor = Cursors.WaitCursor;

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
            if (MessageBox.Show(@"Are you sure for item deletion?", @"Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                DeleteItem((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
        }

        private void tvReportServerDestination_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            txItemPath.Text = e.Node.FullPath.Replace(e.Node.TreeView.Nodes[0].Text, string.Empty);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PreviewReport((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DownloadReport((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
        }

        private void DownloadReport(TreeView treeView)
        {
            if ((ItemTypeEnum)treeView.SelectedNode.Tag != ItemTypeEnum.Report)
                return;

            try
            {
                byte[] reportContent = ((ReportServerProperties)(treeView.Tag)).ReportsServerInstance.GetReportDefinition(
                                treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"));

                if (reportContent == null)
                {
                    MessageBox.Show(@"Cannot obtain the report content");
                    return;
                }

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, reportContent);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #endregion

        #region Methods

        private void LoadProjectList()
        {
            try
            {
                var list = new Projects().ListProjects;

                cmbProject.Items.Clear();

                foreach (var project in list)
                    cmbProject.Items.Add(project.Server1 + " = " + project.Server2);

                cmbProject.Tag = list;
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        private void TransferItems()
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

            try
            {
                var checkedNodes = TreeViewHandling.GetCheckedNodes(tvReportServerSource.Nodes);

                pbTransfer.Visible = true;
                pbTransfer.Maximum = checkedNodes.Count(node => node.Checked);

                int i = 0;

                foreach (TreeNode checkedNode in checkedNodes.Where(node => node.Checked))
                {
                    try
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
                                tvReportServerDestination.SelectedNode.Nodes.Add(new TreeNode(checkedNode.Text, 1, 1)
                                {
                                    Tag = checkedNode.Tag
                                });
                                break;
                            case ItemTypeEnum.Folder:
                                if (ReportServerDestination.CheckItemExist(ItemTypeEnum.Folder,
                                                                           tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                           checkedNode.Text))
                                {
                                    ReportServerDestination.CreateFolder(tvReportServerDestination.SelectedNode.FullPath.Replace(tvReportServerDestination.Nodes[0].Text, string.Empty), checkedNode.Text);
                                    tvReportServerDestination.SelectedNode.Nodes.Add(new TreeNode(checkedNode.Text, 0, 0)
                                    {
                                        Tag = checkedNode.Tag
                                    });
                                }
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }

                tvReportServerDestination.SelectedNode.Expand();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }


            pbTransfer.Visible = false;
            Cursor.Current = Cursors.Arrow;
        }

        private void PreviewReport(TreeView treeView)
        {
            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) != ItemTypeEnum.Report)
                return;

            using (var frmReportViewer = new frmReportViewer())
            {

                frmReportViewer.SourceNode = treeView.SelectedNode;
                frmReportViewer.ShowDialog();
            }
        }

        private void LoadTreeViewPanelsFromSrv()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                var cmbBoxValue = new string[] { };
                var tagValue = new List<Project>();

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

                var selectedProject = tagValue.Where(project => project.Server1 == cmbBoxValue[0].Trim() &&
                                                                project.Server2 == cmbBoxValue[1].Trim()).FirstOrDefault();

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
                treeView.Invoke(new MethodInvoker(() => treeView.Nodes.Add(TreeViewHandling.GetFolderAsNodes(reportingService2005.ReportsServerInstance))));
                treeView.Invoke(new MethodInvoker(treeView.EndUpdate));
                treeView.Tag = reportingService2005;
            }
            else
            {
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                treeView.Nodes.Add(TreeViewHandling.GetFolderAsNodes(reportingService2005.ReportsServerInstance));
                treeView.EndUpdate();
                treeView.Tag = reportingService2005;
            }
        }

        private static void DeleteItem(TreeView treeView)
        {
            try
            {
                if (!treeView.SelectedNode.TreeView.HasChildren)
                    if (((ReportServerProperties)(treeView.Tag)).DeleteItem((ItemTypeEnum)(treeView.SelectedNode.Tag),
                                                                         treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(treeView.SelectedNode.Text, string.Empty),
                                                                         treeView.SelectedNode.Text))
                    {
                        treeView.Nodes.Remove(treeView.SelectedNode);
                    }
                    else
                    {
                        MessageBox.Show("The selected item cannot be deleted");
                    }
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        private void CreateFolder(object sender)
        {

            try
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
                                treeView.SelectedNode.Nodes.Add(new TreeNode(frmFolder.FolderName, 0, 0)
                                                                                                            {
                                                                                                                Tag = ItemTypeEnum.Folder
                                                                                                            });
                                treeView.SelectedNode.Expand();
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
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void UploadReport(object sender)
        {
            var treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

            try
            {
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
                        if (dataSourceMap != null)
                            if (((ReportServerProperties)(treeView.Tag)).DeployReport(((ReportServerProperties)(treeView.Tag)).ReportsServerInstance,
                                                                                      openFileDialog1.FileName,
                                                                                      treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", @"/"),
                                                                                      dataSourceMap.Name,
                                                                                      dataSourceMap.Path))
                            {

                                FileInfo fileInfo = new FileInfo(openFileDialog1.FileName);

                                string fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                                var reportNode = new TreeNode(fileName)
                                                     {
                                                         ImageIndex = 1,
                                                         Tag = ItemTypeEnum.Report,
                                                         Name = fileName,
                                                         SelectedImageIndex = 1
                                                     };

                                treeView.SelectedNode.Nodes.Add(reportNode);
                                treeView.SelectedNode.Expand();
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
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        private void HandleDatasource(object sender)
        {
            TreeView treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            try
            {
                switch ((ItemTypeEnum)(treeView.SelectedNode.Tag))
                {
                    case ItemTypeEnum.DataSource:
                        EditDataSource(sender);
                        break;
                    case ItemTypeEnum.Folder:
                        CreateDatasource(sender);
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void EditDataSource(TreeView treeView)
        {
            if ((ItemTypeEnum)(treeView.SelectedNode.Tag) != ItemTypeEnum.DataSource)
            {
                MessageBox.Show(@"Please choose a Datasource");
                return;
            }

            ReportServerProperties reportServerProperties = new ReportServerProperties(((ReportServerProperties)(treeView.Tag)).ReportsServerInstance.Url);

            DataSourceDefinition dataSourceDefinition = reportServerProperties.GetDataSourceDefinition(treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"));

            using (var frmDataSourceDetail = new frmDataSourceDetail())
            {
                string[] connectionString = dataSourceDefinition.ConnectString.Split(';');

                foreach (string[] itemDict in connectionString.Select(item => item.Split('=')))
                {
                    if (itemDict[0] == "Data Source")
                        frmDataSourceDetail.SQLServer = itemDict[1];
                    if (itemDict[0] == "Initial Catalog")
                        frmDataSourceDetail.DBName = itemDict[1];
                }

                frmDataSourceDetail.DataSourceName = treeView.SelectedNode.Text;

                CatalogItem[] catalogItems = reportServerProperties.ReportsServerInstance.ListDependentItems(treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"));

                frmDataSourceDetail.DependentItems = catalogItems.Select(catalogItem => string.Format("{0}{1}", catalogItem.Path, catalogItem.Name)).ToList();

                if (frmDataSourceDetail.ShowDialog() == DialogResult.OK)
                {
                    string folder = treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/").Replace(treeView.SelectedNode.Text, string.Empty);
                    if (ReportServerDestination.DeleteItem(ItemTypeEnum.DataSource, folder.Substring(0, folder.Length - 1), treeView.SelectedNode.Text) &&
                        ReportServerDestination.CreateDataSource(frmDataSourceDetail.DataSourceName, folder.Substring(0, folder.Length - 1), frmDataSourceDetail.SQLServer, frmDataSourceDetail.DBName))
                    {
                        treeView.SelectedNode.Text = frmDataSourceDetail.DataSourceName;
                        MessageBox.Show("DataSource modified succesfully");
                    }
                    else
                    {
                        MessageBox.Show("DataSource WASN'T modified");
                    }
                }
            }
        }

        private void EditDataSource(object sender)
        {
            EditDataSource((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
        }

        private void CreateDatasource(object sender)
        {
            try
            {
                TreeView treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

                if ((ItemTypeEnum)(treeView.SelectedNode.Tag) == ItemTypeEnum.Folder)
                {
                    using (var frmDataSourceDetail = new frmDataSourceDetail())
                    {
                        if (frmDataSourceDetail.ShowDialog() == DialogResult.OK)
                        {
                            if (ReportServerDestination.CreateDataSource(frmDataSourceDetail.DataSourceName,
                                                                                     treeView.SelectedNode.FullPath.Replace(treeView.Nodes[0].Text, string.Empty).Replace(@"\", "/"),
                                                                                     frmDataSourceDetail.SQLServer,
                                                                                     frmDataSourceDetail.DBName))
                            {
                                MessageBox.Show(@"DataSource created succesfully");
                                treeView.SelectedNode.Nodes.Add(new TreeNode(treeView.SelectedNode.Text + @"/" + frmDataSourceDetail.DataSourceName, 1, 1)
                                {
                                    Tag = ItemTypeEnum.DataSource
                                });
                                treeView.SelectedNode.Expand();
                            }
                            else
                            {
                                MessageBox.Show(@"DataSource WASN'T created");
                            }
                        }
                    }
                }
                else
                    MessageBox.Show(@"Please choose a folder");
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private bool TransferReportNode(TreeNode checkedNode, TreeNode destinationNode)
        {
            try
            {
                FileNameAndPath dataSourceMap = null;
                using (var frmDataSource = new frmDataSource())
                {
                    frmDataSource.FillTreeview(ReportServerDestination.ReportsServerInstance);
                    if (frmDataSource.ShowDialog() == DialogResult.OK)
                        dataSourceMap = frmDataSource.SelectedNode;
                }

                Cursor.Current = Cursors.WaitCursor;
                if (dataSourceMap == null)
                {
                    Cursor.Current = Cursors.Default;
                    return false;
                }

                ((ReportServerProperties)checkedNode.TreeView.Tag).DeployReport(((ReportServerProperties)checkedNode.TreeView.Tag).ReportsServerInstance,
                                                                                checkedNode.Text,
                                                                                checkedNode.FullPath.Replace(checkedNode.TreeView.Nodes[0].Text, string.Empty),
                                                                                destinationNode.FullPath.Replace(destinationNode.TreeView.Nodes[0].Text, string.Empty),
                                                                                dataSourceMap.Name,
                                                                                dataSourceMap.Path);

                destinationNode.Nodes.Add(new TreeNode(checkedNode.Text, 1, 1)
                {
                    Tag = checkedNode.Tag
                });
                return true;
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
            return false;
        }

        #endregion

        #region TreeView Drag & Drop

        private void tvReportServerSource_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tvReportServerSource_DragDrop(object sender, DragEventArgs e)
        {

            if (!e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false)) return;

            Point point = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode destinationNode = ((TreeView)sender).GetNodeAt(point);

            TreeNode newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if (destinationNode != null && (ItemTypeEnum)destinationNode.Tag == ItemTypeEnum.Folder)
            {
                //destinationNode.Nodes.Add((TreeNode)newNode.Clone());
                TransferDragAndDrop(newNode, destinationNode);
                destinationNode.Expand();
            }
            else
            {
                MessageBox.Show("The destination node must be a folder!");
            }
        }

        private void tvReportServerSource_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        private void tvReportServerDestination_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = Cursors.WaitCursor;
        }

        private void tvReportServerDestination_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tvReportServerDestination_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false)) return;

            Point point = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode destinationNode = ((TreeView)sender).GetNodeAt(point);

            TreeNode newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if (destinationNode != null && (ItemTypeEnum)destinationNode.Tag == ItemTypeEnum.Folder)
            {
                //destinationNode.Nodes.Add((TreeNode)newNode.Clone());
                TransferDragAndDrop(newNode, destinationNode);
                destinationNode.Expand();
            }
            else
            {
                MessageBox.Show("The destination node must be a folder!");
            }
        }

        private void tvReportServerDestination_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        public bool TransferDragAndDrop(TreeNode checkedNode, TreeNode destinationNode)
        {
            try
            {
                switch (((ItemTypeEnum)(checkedNode.Tag)))
                {
                    case ItemTypeEnum.Report:
                        if (!TransferReportNode(checkedNode, destinationNode))
                        {
                            Cursor.Current = Cursors.Default;
                            return false;
                        }

                        break;
                    case ItemTypeEnum.Folder:

                        break;

                    case ItemTypeEnum.DataSource:

                        break;
                }
                return true;
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
            Cursor.Current = Cursors.Default;
            return false;
        }

        #endregion
    }
}