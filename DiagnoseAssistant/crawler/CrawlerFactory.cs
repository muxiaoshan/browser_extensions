﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagnoseAssistant1.crawler
{
    class CrawlerFactory
    {
        static Log log = new Log();
        /**
         * 根据访问url获取爬虫实例
         * */
        public static Crawler getCrawler(string url)
        {
            log.WriteLog("url=" + url);
            log.WriteLog("sb===" + EpisodeRegexUtils.matchUrl(url, @"ShowEKGReport[.]aspx(.+?)OID=2412431||18"));
            //超声报告爬虫
            if (EpisodeRegexUtils.matchUrl(url, @"RisWeb3/ReportContent[.]aspx(.+?)LOC=549[&]STYLE=RIS3[-]4$"))
            {
                UltrasonicCrawler crawler = new UltrasonicCrawler();
                string jch = EpisodeRegexUtils.getFirstMatchedFromString(url, @"SID=(.+?)[&]");
                crawler.JCH = jch;
                log.WriteLog("完成检查超声报告内容爬虫构造，开始爬取检查超声报告内容。检查号JCH='" + jch 
                    + "', 患者就诊信息Episode==null is " + (Crawler.episode == null));
                return crawler;
            }
            //PDF报告爬虫
            else if (url.ToUpper().EndsWith(".PDF"))
            {
                PdfCrawler crawler = new PdfCrawler();
                int dotIdx = url.LastIndexOf(".");
                int slashIdx = url.LastIndexOf("/");            
                string fileName = url.Substring(slashIdx + 1, (dotIdx - slashIdx - 1));
                crawler.fileName = fileName;
                crawler.url = url;
                log.WriteLog("完成PDF报告爬虫构造，开始爬取PDF报告内容。fileName=" + fileName); 
                return crawler;
            }
            //检查列表爬虫
            else if (EpisodeRegexUtils.matchUrl(url, @"epr[.]chart[.]csp[?]PatientID=(\d+?)[&]EpisodeID=(\d+?)[&]EpisodeIDs=[&]mradm=(\d+?)[&]ChartID=23"))
            {                
                ScanListCrawler crawler = new ScanListCrawler();
                Crawler.episode = EpisodeRegexUtils.getEpisodeFromUrl(url);
                log.WriteLog("完成检查列表爬虫构造，开始爬取检查列表内容。PatientID=" + Crawler.episode.PatientID 
                    + ", EpisodeID=" + Crawler.episode.EpisodeID);
                return crawler;
            }
            //TODO if matches heater function then get JCH, then create HearterFuncCrawler
            else if (EpisodeRegexUtils.matchUrl(url, @"ShowEKGReport[.]aspx(.+?)OID=2412431||18"))
            {
                log.WriteLog("OID=" + EpisodeRegexUtils.getFirstMatchedFromString(url, @"SID=(.+?)"));
                HearterFuncCrawler crawler = new HearterFuncCrawler();
                string jch = EpisodeRegexUtils.getFirstMatchedFromString(url, @"SID=(.+?)$");
                crawler.JCH = jch;
                log.WriteLog("完成检查心功能报告内容爬虫构造，开始爬取检查心功能报告内容。检查号JCH='" + jch
                    + "', 患者就诊信息Episode==null is " + (Crawler.episode == null));
                return crawler;
            }
            return null;
        }
    }
}
