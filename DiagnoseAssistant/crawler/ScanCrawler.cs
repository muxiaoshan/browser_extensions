using System;
using System.Text;
using System.Text.RegularExpressions;
using mshtml;
using SHDocVw;
using System.Collections;
using System.Collections.Generic;
using DiagnoseAssistant1.crawler.entity;


namespace DiagnoseAssistant1.crawler
{
    abstract class ScanCrawler : HTMLCrawler
    {
        /// <summary>
        /// 检查号
        /// </summary>
        public string JCH { get; set; }
    }
    /// <summary>
    /// 检查列表抓取
    /// </summary>
    class ScanListCrawler : ScanCrawler
    {
        Log log = new Log("ScanListCrawler.txt");
        
        public override void parseElement(IHTMLElement item)
        {
            if (item.id != null && item.id.Equals("tDHCRisclinicQueryOEItem"))
            {
                log.WriteLog("处理检查结果列表table id=【tDHCRisclinicQueryOEItem】");
                
                ArrayList scans = new ArrayList();
                int i = 1;
                ScanEntity se = null;
                Dictionary<string, ScanEntity> jch2Bean = new Dictionary<string, ScanEntity>();
                foreach (IHTMLElement ele in item.all)
                {                    
                    if (ele.id != null && ele.id.Equals("TStudyNoz" + i))
                    {
                        string jch = ele.innerText;
                        se = get(jch2Bean, jch);
                        if (se == null)
                        {
                            se = new ScanEntity();
                            jch2Bean.Add(jch, se);
                            se.TStudyNoz = jch;
                            scans.Add(se); 
                        }
                    }
                    else if (ele.id != null && ele.id.Equals("TItemNamez" + i))
                    {
                        //同一个检查号，检查名称合并
                        se.TItemNamez = (se.TItemNamez == null ? "" : se.TItemNamez) + ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TItemDatez" + i))
                    {
                        se.TItemDatez = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TItemStatusz" + i))
                    {
                        se.TItemStatusz = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TOEOrderDrz" + i))
                    {
                        se.TOEOrderDrz = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TIsIllz" + i))
                    {
                        se.TIsIllz = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TLocNamez" + i))
                    {
                        se.TLocNamez = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TreplocDrz" + i))
                    {
                        se.TreplocDrz = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TIshasImgz" + i))
                    {
                        se.TIshasImgz = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("TMediumNamez" + i))
                    {
                        se.TMediumNamez = ele.innerText;
                    }
                    else if (ele.id != null && ele.id.Equals("Memoz" + i))
                    {
                        se.Memoz = ele.innerText;
                        i++; //处理完最后一个元素，i++准备处理下一个元素                        
                    }
                }
                insert2DB(scans);
            }
        }
        public static ScanEntity get(Dictionary<String, ScanEntity> jch2Entity, string jch)
        {

            foreach (var entry in jch2Entity)
            {
                if (jch.Equals(entry.Key))
                {
                    return (ScanEntity)entry.Value;
                }
            }
            return null;
        }
        public void insert2DB(ArrayList scans)
        {
            ArrayList scanSqls = new ArrayList();            
            foreach (ScanEntity se in scans)
            {
                log.WriteLog("检查号:" + se.TStudyNoz + ",检查名称:" + se.TItemNamez + ",是否阳性:" + se.TIsIllz);
                string qrySql;
                if (se.TStudyNoz != null)
                {
                    //根据检查号判断检查结果是否已存在
                    qrySql = "select count(*) from tb_hyft_jcjl where JCH = '" + se.TStudyNoz + "'";
                }
                else
                {
                    // 先根据患者编码、就诊编码、检查名称、开单时间查询检查结果是否已存在      
                    qrySql = "select count(*) from tb_hyft_jcjl where HZBM='" + episode.PatientID + "' and JZBM='" + episode.EpisodeID 
                        + "' and JCMC='"+se.TItemNamez+"' and KDSJ='"+se.TItemDatez+"'";
                }
                int cnt = sqlOp.executeCount(qrySql);
                if (cnt == 0)
                {
                    //MJ 测试标识
                    string scanSql = "insert into tb_hyft_jcjl (HZBM, JZBM, JCH, JCMC, KDSJ, JCKSMC, BZHJY, MJ) values ('"
                        + episode.PatientID + "','" + episode.EpisodeID
                        + "','" + se.TStudyNoz + "','" + se.TItemNamez + "', '"
                        + se.TItemDatez + "', '" + se.TLocNamez + "', '"
                        + se.Memoz + "', '1001')";
                    scanSqls.Add(scanSql);
                }
            }
            
            try
            {
                sqlOp.executeUpdateBatch(scanSqls);
            }
            catch (Exception e)
            {
                log.WriteLog("检查结果入库异常：" + e);
            }
        }
    }
    /// <summary>
    /// 超声检查报告抓取
    /// </summary>
    class UltrasonicCrawler : ScanCrawler
    {
        Log log = new Log("UltrasonicCrawler.txt");
        
        public override void parseElement(IHTMLElement item)
        {
            try
            {
                if (item.tagName != null && item.tagName.ToUpper().Equals("TABLE"))
                {
                    ScanEntity se = new ScanEntity();
                    se.TStudyNoz = JCH;
                    foreach (IHTMLElement td in item.all)
                    {
                        if (td.tagName != null && td.tagName.ToUpper().Equals("TD"))
                        {
                            //把单元格中的检查与建议内容抓取出来
                            crawlScanAndSuggestion(td, se);
                            
                        }
                    }
                    /*
                    string updateSql = "update tb_hyft_jcjl set YXBXHJCSJ = '" + se.YXBXHJCSJ + "', JCZDHTS='" + se.JCZDHTS
                        + "' where JCH='" + se.TStudyNoz + "' and HZBM='" + episode.PatientID + "' and JZBM='" + episode.EpisodeID + "'";
                     * */
                    if (se.YXBXHJCSJ != null || se.JCZDHTS != null)
                    {
                        if (se.YXBXHJCSJ != null)
                        {
                            se.YXBXHJCSJ = se.YXBXHJCSJ.Replace("'", " ");
                        }
                        if (se.JCZDHTS != null)
                        {
                            se.JCZDHTS = se.JCZDHTS.Replace("'", " ");
                        }
                        string updateSql = "update tb_hyft_jcjl set YXBXHJCSJ = '" + se.YXBXHJCSJ + "', JCZDHTS='" + se.JCZDHTS
                            + "' where JCH='" + se.TStudyNoz + "'";
                        log.WriteLog("更新报告检查意见与诊断意见，sql=" + updateSql);
                        sqlOp.executeUpdate(updateSql);
                    }
                }
            }
            catch (Exception e)
            {
                log.WriteLog("爬取超声报告内容异常：" + e.ToString() + "。" + e.StackTrace);
            }
            
        }
        /// <summary>
        /// 把单元格中的检查与建议内容抓取出来
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="cont"></param>
        private void crawlScanAndSuggestion(mshtml.IHTMLElement td, ScanEntity se)
        {
            string content = td.innerText;
            if (content != null && !"".Equals(content.Trim()))
            {
                //log.WriteLog("报告原文：【" + content + "】");
                if (content.Contains("检查所见"))
                {

                    se.YXBXHJCSJ = EpisodeRegexUtils.getFirstMatchedFromString(content, @"^\s*检查所见:\s*\n+(.+)$", true);                    
                }
                else if (content.Contains("诊断意见"))
                {
                    se.JCZDHTS = EpisodeRegexUtils.getFirstMatchedFromString(content, @"^\s*诊断意见:\s*\n+(.+)$", true);                    
                }
            }

        }
    }
    
}
