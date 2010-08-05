using System;
using System.Windows.Forms;
using SSRSPublisher.ReportService2005;

namespace SSRSPublisher
{
    public partial class frmReportViewer : Form
    {
        public TreeNode SourceNode { get; set; }

        public frmReportViewer()
        {
            InitializeComponent();
        }

        private void frmReportViewer_Load(object sender, EventArgs e)
        {
            //GetReportParameters();
            lbDataSource.Text = string.Format("Current DataSource: {0}", GetDataSource());
        }

        private void btChangeDataSource_Click(object sender, EventArgs e)
        {
            try
            {
                FileNameAndPath dataSourceMap = null;
                var reportsServerInstance = ((ReportServerProperties)(SourceNode.TreeView.Tag)).ReportsServerInstance;
                using (var frmDataSource = new frmDataSource())
                {
                    frmDataSource.FillTreeview(reportsServerInstance);
                    if (frmDataSource.ShowDialog() == DialogResult.OK)
                        dataSourceMap = frmDataSource.SelectedNode;
                }

                Cursor.Current = Cursors.WaitCursor;
                if (dataSourceMap == null)
                {
                    Cursor.Current = Cursors.Default;
                    return;
                }

                MessageBox.Show(((ReportServerProperties)(SourceNode.TreeView.Tag)).AttachDataSourceToReport(dataSourceMap.Name,
                                                                                                              dataSourceMap.Path,
                                                                                                              SourceNode.Text,
                                                                                                              SourceNode.FullPath.Replace(SourceNode.TreeView.Nodes[0].Text, string.Empty).Replace(@"\", @"/").Replace(SourceNode.Text, string.Empty).Replace(@"//", @"/"))
                        ? "Report's datasource is updated"
                        : "Cannot update report's datasource");
                lbDataSource.Text = string.Format("Current DataSource: {0}", GetDataSource());
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Cannot update report's datasource. Error : {0}", exception.Message));
            }
        }

        private void frmReportViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void btPreview_Click(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;

            Uri completeURI = new Uri(((ReportServerProperties)(SourceNode.TreeView.Tag)).ReportServer);

            reportViewer1.ServerReport.ReportServerUrl = new Uri(string.Format("http://{0}{1}{2}", completeURI.Host, completeURI.Segments[0], completeURI.Segments[1]));
            reportViewer1.ServerReport.ReportPath = SourceNode.FullPath.Replace(SourceNode.TreeView.Nodes[0].Text, string.Empty).Replace(@"\", @"/");
            reportViewer1.RefreshReport();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GetDataSource()
        {
            string itemDataSource = string.Empty;
            try
            {
                using (ReportingService2005 reportsServerInstance = ((ReportServerProperties)(SourceNode.TreeView.Tag)).ReportsServerInstance)
                {
                    itemDataSource = reportsServerInstance.GetItemDataSources(SourceNode.FullPath.Replace(SourceNode.TreeView.Nodes[0].Text, string.Empty).Replace(@"\", @"/"))[0].Name;
                }
            }
            catch { }
            return itemDataSource;
        }

        private void GetReportParameters()
        {
            try
            {
                ReportParameter[] parameters;
                using (ReportingService2005 reportsServerInstance = ((ReportServerProperties)(SourceNode.TreeView.Tag)).ReportsServerInstance)
                {
                    reportsServerInstance.Credentials = System.Net.CredentialCache.DefaultCredentials;

                    string historyID = null;
                    bool forRendering = true;
                    ParameterValue[] values = null;
                    DataSourceCredentials[] credentials = null;

                    parameters = reportsServerInstance.GetReportParameters
                        (
                            SourceNode.FullPath.Replace(SourceNode.TreeView.Nodes[0].Text, string.Empty).Replace(@"\", @"/"),
                            historyID,
                            forRendering,
                            values,
                            credentials
                        );
                }

                int ControlCounter = 0;

                foreach (var parameter in parameters)
                {
                    Control control = new Label
                    {
                        Name = string.Format("txParamLabel{0}", ControlCounter),
                        Text = string.Format("[{0} - {1}]", parameter.Name, parameter.Type),
                        Visible = true,
                        Height = 120
                    };

                    flowLayoutPanel1.Controls.Add(control);


                    control = (parameter.Type != ParameterTypeEnum.Boolean)
                                  ? new TextBox()
                                  : new CheckBox() as Control;

                    control.Name = string.Format("txDynamicControl{0}", ControlCounter);
                    control.Visible = true;
                    control.Tag = parameter;
                    control.Height = 120;

                    flowLayoutPanel1.Controls.Add(control);


                    ControlCounter++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }
    }
}
