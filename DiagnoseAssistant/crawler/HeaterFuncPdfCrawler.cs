using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace DiagnoseAssistant1.crawler
{
    class HeaterFuncPdfCrawler : PdfCrawler
    {
        Log log = new Log("HeaterFuncPdfCrawler.log");
        public override void crawl(IHTMLDocument2 document)
        {
            log.WriteLog("pdf name:"+fileName+",pdf url:" + url);
            Dictionary<string, string> imagesItem = ExtractImagesFromPdf();
            string YXBXHJCSJ = get(imagesItem, "YXBXHJCSJ");
            string JCZDHTS = get(imagesItem, "JCZDHTS");
            string updateJCZDSql = "update tb_hyft_jcjl set YXBXHJCSJ = '" + YXBXHJCSJ + "', JCZDHTS='" + JCZDHTS
                        + "' where JCH='" + fileName + "'";
            log.WriteLog("更新影像与建议数据，sql=" + updateJCZDSql);
        }
    }
}
