using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SSRSPublisher.ReportService2005;

namespace SSRSPublisher
{
    public class ReportServerProperties
    {
        #region ctor
        protected ReportServerProperties() { }

        public ReportServerProperties(string _ReportServer)
        {
            ReportServer = _ReportServer;
        }

        private ReportingService2005 _reportsServerInstance;

        private CatalogItem[] _returnedItems;
        #endregion

        #region Properties
        public ReportingService2005 ReportsServerInstance
        {
            get
            {
                return _reportsServerInstance ?? (_reportsServerInstance = new ReportingService2005
                                                                               {
                                                                                   Credentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                                                                                   Url = ReportServer
                                                                               });
            }
        }

        public string Reportname
        {
            get;
            set;
        }

        public string ReportServer
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool WindowsAuthorization
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public bool CreateDataSource(string dataSourceName, string dataSourceLocation, string sqlServerName, string dbName)
        {
            bool resVal = false;

            DataSourceDefinition dataSourceDefinition = new DataSourceDefinition
                                                            {
                                                                Extension = "SQL",
                                                                ConnectString =
                                                                    @"Data Source=" + sqlServerName +
                                                                    @";Initial Catalog=" + dbName,
                                                                ImpersonateUserSpecified = true,
                                                                Prompt = null,
                                                                WindowsCredentials = true,
                                                                CredentialRetrieval = CredentialRetrievalEnum.Integrated,
                                                                Enabled = true
                                                            };
            try
            {
                _reportsServerInstance.CreateDataSource(dataSourceName,
                                     dataSourceLocation,
                                     false,
                                     dataSourceDefinition,
                                     null);
                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }

            return resVal;


        }

        public bool CreateFolder(string folderDestinationPath, string folderName)
        {
            bool resVal = false;

            try
            {
                _reportsServerInstance.CreateFolder(folderName, folderDestinationPath.Replace(@"\", "/"), null);
                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }

            return resVal;
        }

        public DataSource GetDataSource(string sharedDataSourcePath, string dataSourceName)
        {
            var dataSources = ReportsServerInstance.GetItemDataSources(sharedDataSourcePath);

            return dataSources.Where(dataSource => dataSource.Name == dataSourceName).FirstOrDefault();
        }

        public DataSourceDefinition GetDataSourceDefinition(string sharedDataSourcePath)
        {
            return ReportsServerInstance.GetDataSourceContents(sharedDataSourcePath);
        }

        public bool AttachDataSourceToReport(string _dataSourceName, string _dataSourcePath, string _report, string _reportLocation)
        {
            bool resVal = false;

            try
            {
                string fullReportPath = (_reportLocation + "/" + _report).Replace("//", @"/");
                string fullDataSourcePath = (_dataSourcePath.Replace(_dataSourceName, string.Empty) + _dataSourceName).Replace(@"\", @"/");

                DataSource[] sharedDs = _reportsServerInstance.GetItemDataSources(fullReportPath);

                DataSource[] targetDs = new DataSource[sharedDs.Count()];

                int counter = 0;

                foreach (var dataSource in sharedDs)
                {
                    DataSourceReference dsRef = new DataSourceReference
                    {
                        Reference = fullDataSourcePath
                    };

                    dataSource.Item = dsRef;

                    targetDs[counter] = dataSource;
                }

                _reportsServerInstance.SetItemDataSources(fullReportPath, targetDs);

                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }

            return resVal;
        }


        public bool AttachDataSourceToReport(ReportingService2005 rsSource, string dataSourceName, string dataSourcePath, string report, string reportLocation)
        {
            bool resVal = false;

            try
            {
                string fullReportPath = (reportLocation + "/" + report).Replace("//", @"/");
                string fullDataSourcePath = (dataSourcePath.Replace(dataSourceName, string.Empty) + dataSourceName).Replace(@"\", @"/");

                DataSource[] sharedDs = _reportsServerInstance.GetItemDataSources(fullReportPath);

                DataSource[] targetDs = new DataSource[sharedDs.Count()];

                int counter = 0;

                foreach (var dataSource in sharedDs)
                {
                    DataSourceReference dsRef = new DataSourceReference
                    {
                        Reference = fullDataSourcePath
                    };

                    dataSource.Item = dsRef;

                    targetDs[counter] = dataSource;
                }

                _reportsServerInstance.SetItemDataSources(fullReportPath, targetDs);

                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }

            return resVal;
        }

        public bool CheckItemExist(ItemTypeEnum type, string path, string folderName)
        {
            var conditions = new SearchCondition[1];
            conditions[0] = new SearchCondition
                                            {
                                                Condition = ConditionEnum.Contains,
                                                ConditionSpecified = true,
                                                Name = "Name",
                                                Value = folderName
                                            };

            _returnedItems = _reportsServerInstance.FindItems(path, BooleanOperatorEnum.Or, conditions);

            return (_returnedItems.Where(item => item.Type == type)).Any(item => item.Path == path + "/" + folderName);
        }

        public bool DeleteItem(ItemTypeEnum type, string path, string folderName)
        {
            path = path.Substring(path.Length - 1, 1) == @"\"
                       ? path.Substring(0, path.Length - 1).Replace(@"\", @"/")
                       : path.Replace(@"\", @"/");

            bool resVal = false;
            if (CheckItemExist(type, path, folderName))
            {
                _reportsServerInstance.DeleteItem(path + "/" + folderName);
                resVal = true;
            }
            return resVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsSource">Report Server Source object //ReportingService2005</param>
        /// <param name="reportNameSource">Report Source Name</param>
        /// <param name="reportSourcePath">Report Source path</param>
        /// <param name="reportDestinationPath">Report Destination Path</param>
        /// <param name="dataSource">Report Destination DataSource</param>
        /// <param name="dataSourceLocation">Report DataSource Path</param>
        /// <returns>True or False</returns>
        public bool DeployReport(ReportingService2005 rsSource,
                                 string reportNameSource,
                                 string reportSourcePath,
                                 string reportDestinationPath,
                                 string dataSource,
                                 string dataSourceLocation)
        {
            bool resVal = false;

            try
            {
                if (rsSource == null)
                    return false;

                byte[] reportDefinition = rsSource.GetReportDefinition(reportSourcePath.Replace(@"\", "/"));

                DeleteItem(ItemTypeEnum.Report,
                           reportDestinationPath.Replace(reportNameSource, string.Empty).Replace(@"\", "/"),
                           reportNameSource);

                ReportsServerInstance.CreateReport(reportNameSource,
                                                   reportDestinationPath.Replace(reportNameSource, string.Empty).Replace(@"\", "/"),
                                                   true,
                                                   reportDefinition,
                                                   null);
                try
                {
                    AttachDataSourceToReport(rsSource, dataSource,
                                         dataSourceLocation.Replace(dataSource, string.Empty).Replace(@"\", "/"),
                                         reportNameSource,
                                         reportDestinationPath.Replace(reportNameSource, string.Empty).Replace(@"\", "/"));
                }
                catch (Exception)
                {
                    MessageBox.Show(string.Format("The Report {0} was created but the report's Datasource cannot be updated! Please do it manually... (if you want)", reportNameSource));
                }

                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }


            return resVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsSource">Report Server Source object //ReportingService2005</param>
        /// <param name="filePath">Report Source Path (local)</param>
        /// <param name="reportDestinationPath">Report Destination Path</param>
        /// <param name="dataSource">Report Destination DataSource</param>
        /// <param name="dataSourceLocation">Report DataSource Path</param>
        /// <returns>True or False</returns>
        public bool DeployReport(ReportingService2005 rsSource,
                                 string filePath,
                                 string reportDestinationPath,
                                 string dataSource,
                                 string dataSourceLocation)
        {
            bool resVal = false;
            try
            {
                reportDestinationPath = reportDestinationPath.Substring(reportDestinationPath.Length - 1, 1) == @"\"
                           ? reportDestinationPath.Substring(0, reportDestinationPath.Length - 1).Replace(@"\", @"/")
                           : reportDestinationPath.Replace(@"\", @"/");

                if (rsSource == null)
                    return false;

                var fileInfo = new FileInfo(filePath);

                string fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                DeleteItem(ItemTypeEnum.Report,
                           reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"),
                           fileName);

                ReportsServerInstance.CreateReport(fileName,
                                                   reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"),
                                                   true,
                                                   File.ReadAllBytes(filePath),
                                                   null);
                try
                {
                    AttachDataSourceToReport(dataSource,
                                             dataSourceLocation.Replace(dataSource, string.Empty).Replace(@"\", "/"),
                                             fileName,
                                             reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"));
                }
                catch (Exception)
                {
                    MessageBox.Show(string.Format("The Report {0} was created but the report's Datasource cannot be updated! Please do it manually... (if you want)", fileName));
                }
                resVal = true;

            }
            catch (Exception)
            {
                resVal = false;
            }

            return resVal;
        }

        #endregion
    }
}
