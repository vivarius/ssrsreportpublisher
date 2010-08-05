//using System.IO;
//using System.Linq;
//using System.Net;
//using SSRSPublisher;
//using SSRSPublisher.ReportService2005;
//using ParameterValue = SSRSPublisher.ReportService2005.ParameterValue;
//using Warning = SSRSPublisher.ReportService2005.Warning;


//namespace LaPoste.Tools.Reporting
//{
//    public struct ReportParameter
//    {
//        public string Name;
//        public string Value;
//    }

//    public class ReportTools
//    {
//        public static string PDF = "PDF";
//        public static string EXCEL = "EXCEL";
//        public static string WORD = "WORD";

//        public static string SelectItems = "PDF|EXCEL|HTML";

//        public byte[] RenderReport(string url, string reportPath, ReportParameter[] parameters, string FormatType)
//        {
//            //return RenderReport(url, reportPath, parameters, FormatType, false);
//        }

//        //public byte[] RenderReport(string url, string reportPath, ReportParameter[] parameters, string FormatType, bool HumanReadablePDF)
//        //{
//        //    using (var rs = new ReportServerProperties(url).ReportsServerInstance)
//        //    {
//        //        rs.Credentials = CredentialCache.DefaultCredentials;
//        //        rs.Url = string.Format(url);

//        //        const string historyID = null;
//        //        string encoding;
//        //        string mimeType;
//        //        string extension;
//        //        Warning[] warnings;
//        //        string[] streamIDs;

//        //        var execHeader = new ExecutionHeader();
//        //        var execInfo = rs.LoadReport(reportPath, historyID);

//        //        rs.SetExecutionParameters(parameters.Select(p => new ParameterValue
//        //                                                                            {
//        //                                                                                Label = p.Name,
//        //                                                                                Name = p.Name,
//        //                                                                                Value = p.Value
//        //                                                                            }).ToArray()
//        //                                  , "fr-FR");

//        //        rs.ExecutionHeaderValue = execHeader;
//        //        rs.ExecutionHeaderValue.ExecutionID = execInfo.ExecutionID;
//        //        const string deviceSettings = "<DeviceInfo><HumanReadablePDF>True</HumanReadablePDF></DeviceInfo>";
//        //        return rs.Render(FormatType, (HumanReadablePDF) ? deviceSettings : null, out extension, out encoding, out mimeType, out warnings, out streamIDs);
//        //    }
//        //}

//        public static void RenderReports(string FileName, string URL, string ReportPath, ReportParameter[] _params, string FormatType)
//        {
//            RenderReports(FileName, URL, ReportPath, _params, FormatType, false);
//        }

//        public static void RenderReports(string FileName, string URL, string ReportPath, ReportParameter[] _params, string FormatType, bool HumanReadablePDF)
//        {
//            var reportTools = new ReportTools();
//            var file = new FileInfo(FileName);
//            if (file.Exists)
//                File.Delete(FileName);

//            File.WriteAllBytes(FileName, reportTools.RenderReport(URL, ReportPath, _params, FormatType, HumanReadablePDF));
//        }
//    }
//}