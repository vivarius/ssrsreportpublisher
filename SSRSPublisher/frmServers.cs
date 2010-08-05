using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SSRSPublisher
{
    public partial class frmServers : Form
    {
        public List<Project> ListProjects { get; set; }
        public Project CurrentProject { get; set; }

        private bool isUpdate;

        public bool IsUpdate
        {
            get
            {
                return isUpdate;
            }
            set
            {
                isUpdate = value;
                if (!value)
                {
                    btOK.Text = "Insert";
                    btRemove.Enabled = false;
                    btOK.Enabled = true;
                }
                else
                {
                    btOK.Text = "Update";
                    btRemove.Enabled = true;
                    btOK.Enabled = true;
                }
            }
        }

        public frmServers()
        {
            InitializeComponent();

            InitializeProjectsList();
        }

        private void InitializeProjectsList()
        {
            ListProjects = new Projects().ListProjects;

            FillListBox();
        }

        private void FillListBox()
        {
            lstProjects.Items.Clear();
            foreach (var item in ListProjects)
            {
                lstProjects.Items.Add(string.Format("{0} = {1}", item.Server1, item.Server2));
            }
        }

        private void lstProjects_Click(object sender, EventArgs e)
        {
            try
            {
                var currItem = lstProjects.SelectedItem.ToString().Split('=');

                foreach (var item in
                    ListProjects.Where(item => item.Server1 == currItem[0].Trim() && item.Server2 == currItem[1].Trim()))
                {
                    propertyGridServers.SelectedObject = item;
                    btOK.Enabled = true;
                    IsUpdate = true;
                    CurrentProject = item;
                    break;
                }
            }
            catch
            {

                MessageBox.Show("Error! Please Retry!");
            }

        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                Project newElement = (Project)propertyGridServers.SelectedObject;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Directory.GetCurrentDirectory() + "/settings.xml");

                if (IsUpdate)
                {
                    XmlNode xmlNode = xmlDocument.DocumentElement;
                    XmlNode projectNode = xmlNode.SelectSingleNode("/Projects/Project[@ItemNo='" + newElement.Id + "']");

                    projectNode["srvsql1"].SetAttribute("Name", newElement.Server1);
                    projectNode["srvsql1"].SetAttribute("URL", newElement.URL1);
                    projectNode["srvsql1"].SetAttribute("ReportPath", newElement.ReportPath1);


                    projectNode["srvsql2"].SetAttribute("Name", newElement.Server2);
                    projectNode["srvsql2"].SetAttribute("URL", newElement.URL2);
                    projectNode["srvsql2"].SetAttribute("ReportPath", newElement.ReportPath2);
                }
                else
                {

                    XmlElement xmlNewProject = xmlDocument.CreateElement("Project");
                    xmlNewProject.SetAttribute("ItemNo", newElement.Id);

                    XmlElement srvProject = xmlDocument.CreateElement("srvsql1");
                    srvProject.SetAttribute("Name", newElement.Server1);
                    srvProject.SetAttribute("URL", newElement.URL1);
                    srvProject.SetAttribute("ReportPath", newElement.ReportPath1);

                    xmlNewProject.AppendChild(srvProject);

                    srvProject = xmlDocument.CreateElement("srvsql2");
                    srvProject.SetAttribute("Name", newElement.Server2);
                    srvProject.SetAttribute("URL", newElement.URL2);
                    srvProject.SetAttribute("ReportPath", newElement.ReportPath2);

                    xmlNewProject.AppendChild(srvProject);

                    xmlDocument.DocumentElement.AppendChild(xmlNewProject);

                }

                xmlDocument.Save(Directory.GetCurrentDirectory() + "/settings.xml");

                lbInfo.Visible = true;
                lbInfo.Text = @"The items was saved in the project file";

                InitializeProjectsList();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            propertyGridServers.SelectedObject = new Project
                                                     {
                                                         Id = Guid.NewGuid().ToString()
                                                     };
            IsUpdate = false;
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete this project?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(Directory.GetCurrentDirectory() + "/settings.xml");

                    if (IsUpdate)
                    {
                        XmlNode xmlNode = xmlDocument.DocumentElement;
                        XmlNode projectNode = xmlNode.SelectSingleNode("/Projects/Project[@ItemNo='" + ((Project)propertyGridServers.SelectedObject).Id + "']");
                        xmlDocument.SelectSingleNode("/Projects").RemoveChild(projectNode);
                        xmlDocument.Save(Directory.GetCurrentDirectory() + "/settings.xml");
                        InitializeProjectsList();
                        propertyGridServers.SelectedObject = null;
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Impossible to remove this project");
                }

            }
        }
    }
}
