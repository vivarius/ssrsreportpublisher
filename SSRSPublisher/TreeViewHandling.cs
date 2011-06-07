using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SSRSPublisher.ReportService2005;

namespace SSRSPublisher
{
    internal class TreeViewHandling
    {
        internal static TreeNode GetFolderAsNodes(ReportingService2005 reportingService2005)
        {
            return GetFolderAsNodes(reportingService2005, false);
        }

        internal static TreeNode GetFolderAsNodes(ReportingService2005 reportingService2005, bool showDataSource)
        {
            TreeNode xRoot = new TreeNode(reportingService2005.Url)
            {
                Tag = reportingService2005.Url,
                ImageIndex = 3
            };

            return (FillTreeView("/", xRoot, reportingService2005, showDataSource));
        }

        private static TreeNode FillTreeView(string path, TreeNode parentNode, ReportingService2005 reportingService2005, bool showDataSource)
        {
            try
            {
                CatalogItem[] catalogItems = reportingService2005.ListChildren(path, false);
                foreach (CatalogItem catalogItem in catalogItems)
                {
                    switch (catalogItem.Type)
                    {
                        case ItemTypeEnum.Folder:
                            TreeNode folderNode = new TreeNode(catalogItem.Name)
                            {
                                ImageIndex = 0,
                                // Tag = catalogItem,
                                Name = catalogItem.Path
                            };

                            folderNode.Tag = catalogItem.Type;
                            folderNode.SelectedImageIndex = folderNode.ImageIndex;
                            parentNode.Nodes.Add(FillTreeView(catalogItem.Path, folderNode, reportingService2005, showDataSource));
                            break;
                        case ItemTypeEnum.Report:
                            if (showDataSource)
                                break;
                            TreeNode reportNode = new TreeNode(catalogItem.Name)
                            {
                                ImageIndex = 1,
                                //Tag = catalogItem,
                                Name = catalogItem.Path
                            };

                            reportNode.Tag = catalogItem.Type;
                            reportNode.SelectedImageIndex = reportNode.ImageIndex;
                            parentNode.Nodes.Add(reportNode);
                            break;
                        case ItemTypeEnum.DataSource:
                            //if (showDataSource)
                            //{
                            TreeNode dataSourceNode = new TreeNode(catalogItem.Name)
                                                          {
                                                              ImageIndex = 2,
                                                              //Tag = catalogItem,
                                                              Name = catalogItem.Path,
                                                              Tag = catalogItem.Type,
                                                          };

                            dataSourceNode.SelectedImageIndex = dataSourceNode.ImageIndex;
                            parentNode.Nodes.Add(dataSourceNode);
                            //}
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format(@"The Reports Server {0} is not available. Exception details: {1}", reportingService2005.Url, exception.Message));
            }

            return parentNode;
        }

        public static List<TreeNode> GetNodes(TreeNodeCollection nodes)
        {
            var newNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                newNodes.Add(node);
                newNodes.AddRange(GetNodes(node.Nodes));
            }

            return newNodes;
        }

        public static List<TreeNode> GetCheckedNodes(TreeNodeCollection nodes)
        {
            var newNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                newNodes.Add(node);
                newNodes.AddRange(GetCheckedNodes(node.Nodes));
            }

            return newNodes;
        }

        public static void CheckNodes(TreeNode node)
        {
            if (node.Level == 0)
                return;

            if (node.Tag != null)
                if (((ItemTypeEnum)(node.Tag)) != ItemTypeEnum.Folder)
                    return;

            List<TreeNode> listTreeNodes = GetNodes(node.Nodes);

            foreach (TreeNode treeNode in listTreeNodes)
            {
                treeNode.Checked = node.Checked;
            }
        }
    }
}
