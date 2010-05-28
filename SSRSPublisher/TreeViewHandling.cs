﻿using System.Collections.Generic;
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

        private static TreeNode FillTreeView(string Path, TreeNode ParentNode, ReportingService2005 reportingService2005, bool showDataSource)
        {
            CatalogItem[] catalogItems = reportingService2005.ListChildren(Path, false);
            foreach (CatalogItem catalogItem in catalogItems)
            {
                switch (catalogItem.Type)
                {
                    case ItemTypeEnum.Folder:
                        TreeNode SubNode = new TreeNode(catalogItem.Name)
                        {
                            ImageIndex = 0,
                            Tag = catalogItem,
                            Name = catalogItem.Path
                        };

                        SubNode.Tag = catalogItem.Type;
                        SubNode.SelectedImageIndex = SubNode.ImageIndex;
                        ParentNode.Nodes.Add(FillTreeView(catalogItem.Path, SubNode, reportingService2005, showDataSource));
                        break;
                    case ItemTypeEnum.Report:
                        if (showDataSource)
                            break;
                        TreeNode ReportNode = new TreeNode(catalogItem.Name)
                        {
                            ImageIndex = 1,
                            Tag = catalogItem,
                            Name = catalogItem.Path
                        };

                        ReportNode.Tag = catalogItem.Type;
                        ReportNode.SelectedImageIndex = ReportNode.ImageIndex;
                        ParentNode.Nodes.Add(ReportNode);
                        break;
                    case ItemTypeEnum.DataSource:
                        if (showDataSource)
                        {
                            TreeNode DataSourceNode = new TreeNode(catalogItem.Name)
                                                          {
                                                              ImageIndex = 2,
                                                              Tag = catalogItem,
                                                              Name = catalogItem.Path
                                                          };

                            DataSourceNode.Tag = catalogItem.Type;
                            DataSourceNode.SelectedImageIndex = DataSourceNode.ImageIndex;
                            ParentNode.Nodes.Add(DataSourceNode);
                        }
                        break;
                }
            }
            return ParentNode;
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

        public static void CheckNodes(TreeNode Node)
        {
            if (Node.Level == 0)
                return;

            if (Node.Tag != null)
                if (((ItemTypeEnum)(Node.Tag)) != ItemTypeEnum.Folder)
                    return;

            List<TreeNode> listTreeNodes = GetNodes(Node.Nodes);

            foreach (TreeNode treeNode in listTreeNodes)
            {
                treeNode.Checked = Node.Checked;
            }
        }
    }
}
