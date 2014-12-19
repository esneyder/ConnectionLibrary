using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace databaseConnection
{
    public class Parameters
    {
        #region Attributes
        private string server;
        private string database;
        private string user;
        private string password;
        private bool integrateSecurity;
        private string fileParameters;
        private string connectionString;
        private string error;        
        private XmlDocument xml=new XmlDocument();
        private XmlNode node; 
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return connectionString; }
           
        }

        public string Error
        {
            get { return error; }
            
        } 
        #endregion

        #region Constructor
        public Parameters()
        {
            server = "";
            database = "";
            user = "";
            password = "";
            integrateSecurity = true;
            fileParameters = "";
            connectionString = "";
            error = "";
            
        } 
        #endregion

        #region Public Methods
        public bool GenerateConnectionString(string fileNameParameters)
        {
            fileParameters = Application.StartupPath + "\\" + fileNameParameters;
            try
            {
                // Read xml file
                xml.Load(fileParameters);

                // Read the first attribute of the connection string
                node = xml.SelectSingleNode("//Server");

                // Return the value of the node
                server = node.InnerText;

                node = xml.SelectSingleNode("//Database");
                database = node.InnerText;

                node = xml.SelectSingleNode("//User");
                user = node.InnerText;

                node = xml.SelectSingleNode("//Password");
                password = node.InnerText;

                node = xml.SelectSingleNode("//IntegrateSecurity");
                integrateSecurity = Convert.ToBoolean(node.InnerText);

                if (integrateSecurity) // Windows authentication
                {
                    connectionString = "Data Source=" + server + ";Initial Catalog=" +
                        database + ";Integrate Security=true";
                }
                else // Sql authentication
                {
                    connectionString = "Data Source=" + server + ";Initial Catalog=" +
                        database +
                        ";User Id=" + user +
                        ";Password=" + password;
                      
                }
                xml = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        } 
        #endregion
    }
}
