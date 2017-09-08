using System;
using System.Text;
using mshtml;
namespace DiagnoseAssistant1.crawler
{
    class DefaultCrawler : HTMLCrawler
    {
        Log log = new Log("DefaultCrawler.txt");

        public override void parseElement(IHTMLElement item)
        {
            if (item.tagName != null && item.tagName.ToUpper().Equals("TD"))
            {
                
                if (item.innerText != null && item.innerText.IndexOf("检查所见") > -1)
                {
                    log.WriteLog("Crawled:" + item.innerText);
                }
            }
        }
    }
}
