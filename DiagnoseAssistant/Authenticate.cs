using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.IO;
using System.Xml;
using DiagnoseAssistant1.config;

namespace DiagnoseAssistant1
{
    public class Authenticate
    {
        private static Log log = new Log();

        public static bool isWhiteUser(string username)
        {
            bool whiteuser = false;
            string configFilePath = @"c:\fzzl\DiagnoseAssistant1.xml";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFilePath);
                XmlNode conf = doc.SelectSingleNode("Configuration");
                //无该配置文件，全部用户可访问
                if (conf == null || conf.ChildNodes == null || conf.ChildNodes.Count == 0)
                {
                    return true;
                }
                XmlNode node = conf.SelectSingleNode("whitelist");
                XmlNodeList prop = node.SelectNodes("User");

                foreach (XmlNode item in prop)
                {
                    if (username != null && username.Equals(item.Attributes["account"].Value))
                    {
                        whiteuser = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                log.WriteLog("读取配置文件" + configFilePath + "异常：" + e.ToString() + e.StackTrace + e.Source);
            }
            return whiteuser;
        }

        public static bool isWhiteUser1(string username)
        {
            bool whiteuser = false;
            string configFilePath = @"c:\DiagnoseAssistant1.xml";
            try
            {
                var encoding = System.Text.Encoding.UTF8;
                var serializer = new XmlSerializer(typeof(Configuration), null, null, null, null);//new XmlSerializer(typeof(Configuration));                
                using (var stream = new StreamReader(configFilePath, encoding, false))
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        Configuration conf = serializer.Deserialize(reader) as Configuration;
                        log.WriteLog("读取"+configFilePath+"的配置：" + conf.ToString());
                        List<User> whitelist = conf.whitelist;
                        foreach (User user in whitelist)
                        {
                            if (username != null && username.Equals(user.account))
                            {
                                whiteuser = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.WriteLog("读取配置文件"+configFilePath+"异常：" + e.ToString() + e.StackTrace);
            }
            return whiteuser;
        }
    }
}
