using System;
using System.Collections.Generic;
using System.Xml;

namespace SSRSPublisher
{
    public class FileNameAndPath
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class Projects
    {
        List<Project> listProjects = new List<Project>();
        XmlDocument doc = new XmlDocument();

        public Projects()
        {
            try
            {
                doc.Load("settings.xml");

                XmlNodeList projectsList = doc.GetElementsByTagName("Project");

                foreach (XmlNode node in projectsList)
                {
                    XmlElement projectElement = (XmlElement)node;

                    listProjects.Add(new Project
                    {
                        Id = Convert.ToInt32(projectElement.Attributes["ItemNo"].InnerText),
                        Server1 = projectElement.GetElementsByTagName("srvsql1")[0].Attributes["Name"].InnerText,
                        Server2 = projectElement.GetElementsByTagName("srvsql2")[0].Attributes["Name"].InnerText
                    });

                }
            }
            catch (Exception)
            {

            }

        }

        //Add

        public List<Project> ListProjects
        {
            get { return listProjects; }
        }


        public IEnumerable<List<Project>> Get()
        {
            yield return ListProjects;
        }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Server1 { get; set; }
        public string Server2 { get; set; }
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
