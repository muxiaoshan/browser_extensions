using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace DiagnoseAssistant1.crawler
{
    abstract class HTMLCrawler : Crawler
    {
        Log log = new Log("HTMLCrawler.txt");
        public override void crawl(IHTMLDocument2 document)
        {
            ReadDoc(document);
        }
        //读取网页内容
        public void ReadDoc(IHTMLDocument2 doc)
        {
            log.WriteLog("解析网页【" + doc.location.href + "】DOM结构");
            foreach (IHTMLElement item in doc.all)
            {
                if (item != null && item.tagName != null)
                {
                    if (item.tagName.ToUpper().Equals("FRAME")
                        || item.tagName.ToUpper().Equals("IFRAME"))
                    {
                        mshtml.IHTMLFrameBase2 iFrame = (mshtml.IHTMLFrameBase2)item;
                        mshtml.IHTMLDocument2 iHTMLDocument2 = iFrame.contentWindow.document;
                        ReadDoc(iHTMLDocument2);
                    }
                    else
                    {
                        parseElement(item);
                    }
                }

            }
        }
        public abstract void parseElement(IHTMLElement item);
    }
}
