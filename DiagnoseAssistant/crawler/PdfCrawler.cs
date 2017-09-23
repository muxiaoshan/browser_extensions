﻿using System;
using System.IO;
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
        Log log = new Log("PdfCrawler.log");
        public string JCH { get; set;} //检查号
        public string url { get; set; } //访问路径
        public override void crawl(IHTMLDocument2 document)
        {
            try
            {
                Dictionary<string, string> scanItem = ExtractTextFromPdf();
                string jcsj = get(scanItem, "JCSJ");
                string bgsj = get(scanItem, "BGSJ");
                string updateSql = "update tb_hyft_jcjl set JCSJ = '" + jcsj + "', BGSJ='" + bgsj
                            + "', BGRQ='"+ bgsj
                            + "' where JCH='" + JCH + "'";
                log.WriteLog("更新报告检查时间与报告时间、报告日期，sql=" + updateSql);
                sqlOp.executeUpdate(updateSql);
                //log.WriteLog("读取到的PDF内容:" + contents.ToString());
                Dictionary<string, string> imagesItem = ExtractImagesFromPdf();
                string YXBXHJCSJ = get(imagesItem, "YXBXHJCSJ");
                string JCZDHTS = get(imagesItem, "JCZDHTS");
                string updateJCZDSql = "update tb_hyft_jcjl set YXBXHJCSJ = '" + YXBXHJCSJ + "', JCZDHTS='" + JCZDHTS
                            + "' where JCH='" + JCH + "'";
                sqlOp.executeUpdate(updateJCZDSql);
                log.WriteLog("更新影像与建议数据，sql=" + updateJCZDSql);
                sqlOp.executeUpdate(updateJCZDSql);
            }
            catch (Exception e)
            {
                log.WriteLog("读取PDF异常:" + e.ToString() + e.StackTrace);
            }
        }
        public Dictionary<string, string> ExtractImagesFromPdf()
        {
            //创建图片保存目录
            string imageDirectory = "c:\\fzzl\\crawler\\images";
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }
            Dictionary<string, string> scanItem = new Dictionary<string, string>();
            Dictionary<string, System.Drawing.Image> images = PdfUtils.PdfImageExtractor.ExtractImages(new Uri(url));
            log.WriteLog("读取到的pdf文件：");
           
            foreach (var image in images)
            {
                log.WriteLog(image.Key);
                int imageIndex = image.Key.IndexOf('.')-1;
                String imageDi = image.Key.Substring(imageIndex, 1);
                string imagePath = imageDirectory + "\\" + image.Key;
                log.WriteLog("image+imagePath:" + imagePath);
                //保存图片文件
                image.Value.Save(imagePath);
                try
                {
                    //图片识别
                    string textFromImage = OCR.Identify.StartIdentifyingCaptcha(imagePath, imageDirectory, 200);
                    if (imageDi != null && "1".Equals(imageDi))
                    {
                        scanItem.Add("YXBXHJCSJ", textFromImage);
                    }
                    if (imageDi != null && "2".Equals(imageDi))
                    {
                        scanItem.Add("JCZDHTS", textFromImage);
                    }
                    log.WriteLog("text from image:" + textFromImage);
                }
                catch (Exception e)
                {
                    log.WriteLog("Exception occured when recognizing image["+imagePath+"] " + e.ToString() + e.Source);
                }
            }
            return scanItem;
        }
        public Dictionary<string, string> ExtractTextFromPdf()
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
                foreach (string line in linesOfPage)
                {
                    //log.WriteLog("line=" + line);
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
