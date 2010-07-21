using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSRSPublisher
{
    public partial class frmServers : Form
    {
        public List<Project> ListProjects { get; set; }

        public frmServers()
        {
            InitializeComponent();
            propertyGridServers.SelectedObject = ListProjects;
        }
    }
}
