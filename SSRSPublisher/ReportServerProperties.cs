using System;
using System.IO;
using System.Linq;
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
                                                                                   Credentials = System.Net.CredentialCache.DefaultCredentials,
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
        public bool CreateDataSource(string _dataSourceName, string _dataSourceLocation, string _sqlServerName, string _dbName)
        {
            bool resVal = false;

            DataSourceDefinition dataSourceDefinition = new DataSourceDefinition
                                                            {
                                                                Extension = "SQL",
                                                                ConnectString =
                                                                    @"Data Source=" + _sqlServerName +
                                                                    @";Initial Catalog=" + _dbName,
                                                                ImpersonateUserSpecified = true,
                                                                Prompt = null,
                                                                WindowsCredentials = true,
                                                                CredentialRetrieval = CredentialRetrievalEnum.Integrated,
                                                                Enabled = true
                                                            };
            try
            {
                _reportsServerInstance.CreateDataSource(_dataSourceName,
                                     _dataSourceLocation,
                                     false,
                                     dataSourceDefinition,
                                     null);
                resVal = true;
            }
            catch (Exception ex)
            {
                resVal = false;
            }

            return resVal;


        }

        public bool CreateFolder(string _FolderDestinationPath, string _FolderName)
        {
            bool resVal = false;

            try
            {
                _reportsServerInstance.CreateFolder(_FolderName, _FolderDestinationPath.Replace(@"\", "/"), null);
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
                DataSource[] dataSources = _reportsServerInstance.GetItemDataSources(_reportLocation + "/" + _report);

                DataSourceReference dataSourceReference = new DataSourceReference
                                                                                {
                                                                                    Reference = _dataSourcePath + _dataSourceName
                                                                                };

                dataSources[0].Item = dataSourceReference;

                _reportsServerInstance.SetItemDataSources(_reportLocation + "/" + _report,
                                                          dataSources);

                //_reportsServerInstance.SetItemDataSources(_reportLocation + "/" + _report,
                //                                          new[] {
                //                                                    GetDataSource(_dataSourceLocation, _dataSourceName) 
                //                                                 });

                resVal = true;
            }
            catch (Exception ex)
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

        ///from server to server
        public bool DeployReport(ReportingService2005 _rsSource,
                                 string _reportNameSource,
                                 string _reportSourcePath,
                                 string _reportDestinationPath,
                                 string _dataSource,
                                 string _dataSourceLocation)
        {
            bool resVal = false;

            byte[] _reportDefinition;

            try
            {
                if (_rsSource == null)
                    return false;

                DeleteItem(ItemTypeEnum.Report,
                           _reportDestinationPath.Replace(_reportNameSource, string.Empty).Replace(@"\", "/"),
                           _reportNameSource);

                _reportDefinition = _rsSource.GetReportDefinition(_reportSourcePath.Replace(@"\", "/"));

                ReportsServerInstance.CreateReport(_reportNameSource,
                                                   _reportDestinationPath.Replace(_reportNameSource, string.Empty).Replace(@"\", "/"),
                                                   true,
                                                   _reportDefinition,
                                                   null);

                AttachDataSourceToReport(_dataSource,
                                         _dataSourceLocation.Replace(_dataSource, string.Empty).Replace(@"\", "/"),
                                         _reportNameSource,
                                         _reportDestinationPath.Replace(_reportNameSource, string.Empty).Replace(@"\", "/"));
                resVal = true;
            }
            catch (Exception)
            {
                resVal = false;
            }


            return resVal;
        }

        ///from local File to server
        public bool DeployReport(ReportingService2005 _rsSource,
                                 string _filePath,
                                 string _reportDestinationPath,
                                 string _dataSource,
                                 string _dataSourceLocation)
        {
            bool resVal = false;
            try
            {
                _reportDestinationPath = _reportDestinationPath.Substring(_reportDestinationPath.Length - 1, 1) == @"\"
                           ? _reportDestinationPath.Substring(0, _reportDestinationPath.Length - 1).Replace(@"\", @"/")
                           : _reportDestinationPath.Replace(@"\", @"/");

                if (_rsSource == null)
                    return false;

                FileInfo fileInfo = new FileInfo(_filePath);

                string fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                DeleteItem(ItemTypeEnum.Report,
                           _reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"),
                           fileName);

                ReportsServerInstance.CreateReport(fileName,
                                                   _reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"),
                                                   true,
                                                   File.ReadAllBytes(_filePath),
                                                   null);

                AttachDataSourceToReport(_dataSource,
                                         _dataSourceLocation.Replace(_dataSource, string.Empty).Replace(@"\", "/"),
                                         fileName,
                                         _reportDestinationPath.Replace(fileName, string.Empty).Replace(@"\", "/"));
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
