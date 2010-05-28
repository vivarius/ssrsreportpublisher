using System.Windows.Forms;

namespace SSRSPublisher
{

    partial class frmMain : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblBWInfo = new System.Windows.Forms.Label();
            this.btLoadServers = new System.Windows.Forms.Button();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.tvReportServerSource = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.creationDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creationDossierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadDossierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suppRapportDossierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.txItemPath = new System.Windows.Forms.TextBox();
            this.tvReportServerDestination = new System.Windows.Forms.TreeView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pbTransfer = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(884, 607);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.tvReportServerSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.tvReportServerDestination);
            this.splitContainer1.Size = new System.Drawing.Size(878, 588);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblBWInfo);
            this.panel1.Controls.Add(this.btLoadServers);
            this.panel1.Controls.Add(this.cmbProject);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(440, 47);
            this.panel1.TabIndex = 5;
            // 
            // lblBWInfo
            // 
            this.lblBWInfo.AutoSize = true;
            this.lblBWInfo.Location = new System.Drawing.Point(317, 17);
            this.lblBWInfo.Name = "lblBWInfo";
            this.lblBWInfo.Size = new System.Drawing.Size(0, 13);
            this.lblBWInfo.TabIndex = 2;
            // 
            // btLoadServers
            // 
            this.btLoadServers.Location = new System.Drawing.Point(267, 12);
            this.btLoadServers.Name = "btLoadServers";
            this.btLoadServers.Size = new System.Drawing.Size(44, 23);
            this.btLoadServers.TabIndex = 1;
            this.btLoadServers.Text = "OK";
            this.btLoadServers.UseVisualStyleBackColor = true;
            this.btLoadServers.Click += new System.EventHandler(this.btLoadServers_Click);
            // 
            // cmbProject
            // 
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.Location = new System.Drawing.Point(3, 14);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(258, 21);
            this.cmbProject.TabIndex = 0;
            // 
            // tvReportServerSource
            // 
            this.tvReportServerSource.CheckBoxes = true;
            this.tvReportServerSource.ContextMenuStrip = this.contextMenuStrip1;
            this.tvReportServerSource.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tvReportServerSource.ImageIndex = 0;
            this.tvReportServerSource.ImageList = this.imageList1;
            this.tvReportServerSource.Location = new System.Drawing.Point(0, 53);
            this.tvReportServerSource.Name = "tvReportServerSource";
            this.tvReportServerSource.SelectedImageIndex = 0;
            this.tvReportServerSource.Size = new System.Drawing.Size(440, 535);
            this.tvReportServerSource.TabIndex = 3;
            this.tvReportServerSource.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvReportServerSource_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creationDataSourceToolStripMenuItem,
            this.creationDossierToolStripMenuItem,
            this.downloadDossierToolStripMenuItem,
            this.suppRapportDossierToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(188, 92);
            // 
            // creationDataSourceToolStripMenuItem
            // 
            this.creationDataSourceToolStripMenuItem.Name = "creationDataSourceToolStripMenuItem";
            this.creationDataSourceToolStripMenuItem.ShowShortcutKeys = false;
            this.creationDataSourceToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.creationDataSourceToolStripMenuItem.Text = "Création DataSource";
            this.creationDataSourceToolStripMenuItem.Click += new System.EventHandler(this.creationDataSourceToolStripMenuItem_Click);
            // 
            // creationDossierToolStripMenuItem
            // 
            this.creationDossierToolStripMenuItem.Name = "creationDossierToolStripMenuItem";
            this.creationDossierToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.creationDossierToolStripMenuItem.Text = "Création Dossier";
            this.creationDossierToolStripMenuItem.Click += new System.EventHandler(this.creationDossierToolStripMenuItem_Click);
            // 
            // downloadDossierToolStripMenuItem
            // 
            this.downloadDossierToolStripMenuItem.Name = "downloadDossierToolStripMenuItem";
            this.downloadDossierToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.downloadDossierToolStripMenuItem.Text = "Téléchargement dossier";
            this.downloadDossierToolStripMenuItem.Click += new System.EventHandler(this.downloadDossierToolStripMenuItem_Click);
            // 
            // suppRapportDossierToolStripMenuItem
            // 
            this.suppRapportDossierToolStripMenuItem.Name = "suppRapportDossierToolStripMenuItem";
            this.suppRapportDossierToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.suppRapportDossierToolStripMenuItem.Text = "Supp. rapport / dossier";
            this.suppRapportDossierToolStripMenuItem.Click += new System.EventHandler(this.suppRapportDossierToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.gif");
            this.imageList1.Images.SetKeyName(1, "report.gif");
            this.imageList1.Images.SetKeyName(2, "Datasource.gif");
            this.imageList1.Images.SetKeyName(3, "database.gif");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txItemPath);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(434, 47);
            this.panel2.TabIndex = 6;
            // 
            // txItemPath
            // 
            this.txItemPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txItemPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txItemPath.Location = new System.Drawing.Point(3, 3);
            this.txItemPath.Multiline = true;
            this.txItemPath.Name = "txItemPath";
            this.txItemPath.Size = new System.Drawing.Size(422, 41);
            this.txItemPath.TabIndex = 0;
            // 
            // tvReportServerDestination
            // 
            this.tvReportServerDestination.ContextMenuStrip = this.contextMenuStrip1;
            this.tvReportServerDestination.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tvReportServerDestination.ImageIndex = 0;
            this.tvReportServerDestination.ImageList = this.imageList1;
            this.tvReportServerDestination.Location = new System.Drawing.Point(0, 53);
            this.tvReportServerDestination.Name = "tvReportServerDestination";
            this.tvReportServerDestination.SelectedImageIndex = 0;
            this.tvReportServerDestination.Size = new System.Drawing.Size(434, 535);
            this.tvReportServerDestination.TabIndex = 4;
            this.tvReportServerDestination.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvReportServerDestination_NodeMouseClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(806, 613);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 47);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Sortir";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(3, 613);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 47);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&Transfer";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pbTransfer
            // 
            this.pbTransfer.Location = new System.Drawing.Point(94, 625);
            this.pbTransfer.Name = "pbTransfer";
            this.pbTransfer.Size = new System.Drawing.Size(706, 23);
            this.pbTransfer.TabIndex = 6;
            this.pbTransfer.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 660);
            this.Controls.Add(this.pbTransfer);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.MaximumSize = new System.Drawing.Size(892, 687);
            this.MinimumSize = new System.Drawing.Size(892, 687);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mediapost :: SSRS Publisher ";
            this.Load += new System.EventHandler(this.ReportServer_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private SplitContainer splitContainer1;
        private TreeView tvReportServerSource;
        private TreeView tvReportServerDestination;
        private Panel panel1;
        private Panel panel2;
        private ImageList imageList1;
        private Button btnCancel;
        private Button btnOK;
        private ProgressBar pbTransfer;
        private ComboBox cmbProject;
        private Button btLoadServers;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label lblBWInfo;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem creationDataSourceToolStripMenuItem;
        private ToolStripMenuItem creationDossierToolStripMenuItem;
        private ToolStripMenuItem downloadDossierToolStripMenuItem;
        private ToolStripMenuItem suppRapportDossierToolStripMenuItem;
        private TextBox txItemPath;
        private OpenFileDialog openFileDialog1;
    }
}


