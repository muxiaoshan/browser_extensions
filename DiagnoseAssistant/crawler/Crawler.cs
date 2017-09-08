using System;
using System.Text;
using DiagnoseAssistant1.database;
using mshtml;

namespace DiagnoseAssistant1.crawler
{
    abstract class Crawler
    {
        /// <summary>
        /// 静态的患者就诊信息，不同爬虫间共享
        /// </summary>
        public static Episode episode { get; set; }
        /// <summary>
        /// 数据库操作
        /// </summary>
        public MySqlOp sqlOp = new MySqlOp();

        public abstract void crawl(IHTMLDocument2 document);
    }
}
