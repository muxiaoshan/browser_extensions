using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using DiagnoseAssistant1.pdfstrategy;

namespace DiagnoseAssistant1.crawler
{
    class PdfCrawler : Crawler
    {
        Log log = new Log("PdfCrawler.txt");
        public string fileName { get; set;} //文件名
        public string url { get; set; } //访问路径
        public override void crawl(IHTMLDocument2 document)
        {
            try
            {
                Dictionary<string, string> scanItem = ReadPdfContent();
                string jcsj = get(scanItem, "JCSJ");
                string bgsj = get(scanItem, "BGSJ");
                string updateSql = "update tb_hyft_jcjl set JCSJ = '" + jcsj + "', BGSJ='" + bgsj
                            + "', BGRQ='"+ bgsj
                            + "' where JCH='" + fileName + "'";
                log.WriteLog("更新报告检查时间与报告时间、报告日期，sql=" + updateSql);
                sqlOp.executeUpdate(updateSql);
                //log.WriteLog("读取到的PDF内容:" + contents.ToString());
            }
            catch (Exception e)
            {
                log.WriteLog("读取PDF异常:" + e.ToString() + e.StackTrace);
            }
        }

        public Dictionary<string, string> ReadPdfContent()
        {
            PdfReader pdfReader = new PdfReader(new Uri(url));
            int numberOfPages = pdfReader.NumberOfPages;
            StringBuilder text = new StringBuilder();
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            Dictionary<string, string> scanItem = new Dictionary<string, string>();
            string page;
            
            for (int i = 1; i <= numberOfPages; ++i)
            {
                page = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, i, strategy);
                string[] linesOfPage = page.Split('\n');
                string curLine = null; //当前行
                string preLine = null;//前一行
                foreach (string line in linesOfPage)
                {
                    curLine = line;
                    log.WriteLog("line=" + line);
                    try
                    {
                        if (line.Contains("检查时间"))
                        {
                            string dateString = EpisodeRegexUtils.getFirstMatchedFromString(line, @"^检查时间：(.+)$");
                            DateTime dt = DateTime.ParseExact(dateString, "yyyy年MM月dd日 HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            scanItem.Add("JCSJ", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (line.Contains("报告时间"))
                        {
                            string dateString = EpisodeRegexUtils.getFirstMatchedFromString(line, @"^报告时间：(.+)$");
                            DateTime dt = DateTime.ParseExact(dateString, "yyyy年MM月dd日", System.Globalization.CultureInfo.CurrentCulture);
                            string bgsj = dt.ToString("yyyy-MM-dd");
                            scanItem.Add("BGSJ", bgsj);
                        }
                    }
                    catch (Exception e)
                    {
                        log.WriteLog("【"+line+"】解析失败：" + e.ToString() + e.StackTrace);
                    }
                    
                    
                    preLine = line;
                }
            }
            
            pdfReader.Close();
            return scanItem;
        }
        public string get(Dictionary<string, string> scanItem, string key)
        {
            foreach (var entry in scanItem)
            {
                if (key.Equals(entry.Key))
                {
                    return entry.Value;
                }
            }
            return null;
        }
    }
}
