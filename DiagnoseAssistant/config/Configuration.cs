using System.Collections.Generic;
using System.Xml.Serialization;
using System.Configuration;
using System.IO;
using System.Xml;

namespace DiagnoseAssistant1.config
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("whitelist")]
        public List<User> whitelist { get; set; }

        public override string ToString()
        {
            string str = "[";
            foreach (User user in whitelist)
            {
                str += user.ToString();
            }
            str += "]";
            return str;
        }
    }

    [XmlRoot("User")]
    public class User
    {
        [XmlAttribute("account")]
        public string account { get; set; }

        public override string ToString()
        {
            return account;
        }
    }
    
}
