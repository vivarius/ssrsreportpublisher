using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SSRSPublisher
{
    public class FileNameAndPath
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class Project
    {
        public string Id { get; set; }
        public string Server1 { get; set; }
        public string Server2 { get; set; }
        public string URL1 { get; set; }
        public string URL2 { get; set; }
        public string ReportPath1 { get; set; }
        public string ReportPath2 { get; set; }

    }

    public class Projects
    {
        readonly List<Project> _listProjects = new List<Project>();
        readonly XmlDocument _doc = new XmlDocument();

        public Projects()
        {
            try
            {
                _doc.Load(Directory.GetCurrentDirectory() + "/settings.xml");

                XmlNodeList projectsList = _doc.GetElementsByTagName("Project");

                foreach (XmlElement projectElement in projectsList.Cast<XmlElement>())
                {
                    _listProjects.Add(new Project
                                          {
                                              Id = projectElement.Attributes["ItemNo"].InnerText,
                                              Server1 = projectElement.GetElementsByTagName("srvsql1")[0].Attributes["Name"].InnerText,
                                              Server2 = projectElement.GetElementsByTagName("srvsql2")[0].Attributes["Name"].InnerText,
                                              URL1 = projectElement.GetElementsByTagName("srvsql1")[0].Attributes["URL"].InnerText,
                                              URL2 = projectElement.GetElementsByTagName("srvsql2")[0].Attributes["URL"].InnerText,
                                              ReportPath1 = projectElement.GetElementsByTagName("srvsql1")[0].Attributes["ReportPath"].InnerText,
                                              ReportPath2 = projectElement.GetElementsByTagName("srvsql2")[0].Attributes["ReportPath"].InnerText
                                          });
                }
            }
            catch (Exception)
            {

            }

        }

        public List<Project> ListProjects
        {
            get { return _listProjects; }
        }


        public IEnumerable<List<Project>> Get()
        {
            yield return ListProjects;
        }
    }

    public class ComboItem
    {
        public object BindingValue { get; private set; }

        public object DisplayValue { get; private set; }

        public ComboItem(object aBindingValue, object aDisplayValue)
        {
            BindingValue = aBindingValue;
            DisplayValue = aDisplayValue;
        }

        public override String ToString()
        {
            return Convert.ToString(DisplayValue);
        }
    }
}
