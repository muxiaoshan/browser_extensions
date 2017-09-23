﻿using DiagnoseAssistant1.crawler;
using Microsoft.Win32;
using mshtml;
using SHDocVw;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Windows.Forms;

namespace DiagnoseAssistant1
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    //个性化GUID
    [Guid("1194A96A-BF78-405D-8CE8-55385C878094")]
    [ComDefaultInterface(typeof(JavaScriptExtension))]
    public class DiagnoseAssistant1 : IObjectWithSite, IOleCommandTarget, JavaScriptExtension
    {
        static IWebBrowser2 browser;
        private object site;
        static Log log = new Log("DiagnoseAssistant1.log");

        //存放就诊编码
        static Episode _episode = null;
        /// <summary>
        /// 登录用户名
        /// </summary>
        static string username = null;
        const string fzzlUrlPrefix = "http://172.26.111.12/newAiadt/a/home/login";        
        
        #region OnDocumentComplete
        //页面加载完成，包括iframe内页面加载后调用
        void OnDocumentComplete(object pDisp, ref object URL)
        {
            log.WriteLog("DiagnoseAssistant1 OnDocumentComplete. URL:" + URL);
            try
            {
                if (URL != null)
                {
                    string urlStr = URL.ToString();
                    
                    //访问电子病历页面
                    if (urlStr.Contains("websys.csp?a=a&TMENU=51686"))
                    {
                        Episode episode = EpisodeRegexUtils.getEpisodeFromUrl(urlStr);
                        log.WriteLog("访问门诊电子病历页面，暂存患者编码[" + episode.PatientID + "]与就诊编码[" + episode.EpisodeID + "]");
                        _episode = episode;
                    }
                    /* */
                    //访问门诊患者列表页面
                    else if (urlStr.Contains("websys.csp?a=a&TMENU=50136"))
                    {
                        if (_episode != null) //ie 11以下患者列表页面加载完后打开辅助诊疗；ie 11及以上则在患者列表页面跳转前就打开
                        {
                            log.WriteLog("访问门诊患者列表页面。已查看并暂存的门诊患者电子病历数，episode=" + _episode.ToString() + "，非null则访问辅助诊疗页面。");
                        }
                    }
                     /* */
                    //if (EpisodeRegexUtils.matchUrl(urlStr, "RisWeb3/ReportContent[.]aspx(.+?)LOC=549[&]STYLE=RIS3[-]4$"))
                    //检查报告
                    else if (urlStr.Contains("RisWeb3/ReportContent.aspx") || urlStr.Contains("csp/epr.chart.csp?PatientID=")
                        || urlStr.ToUpper().EndsWith(".PDF") || urlStr.Equals("http://172.26.102.9/ekgweb/service/ShowEKGReport.aspx?OID=2412431||18"))
                    {
                        //FIXME 测试ajax是否加载完成
                        if (urlStr.Equals("http://172.26.102.9/ekgweb/service/ShowEKGReport.aspx?OID=2412431||18"))
                        {
                            //IHTMLDocument2 doc = browser.Document;
                            //log.WriteLog(doc.body.innerHTML);
                            /*
                            registerMonitor(urlStr);
                            HTMLDocument document = (HTMLDocument)browser.Document;

                            IHTMLElement head = (IHTMLElement)((IHTMLElementCollection)document.all.tags("head")).item(null, 0);
                            IHTMLScriptElement scriptObject = (IHTMLScriptElement)document.createElement("script");
                            scriptObject.type = @"text/javascript";
                            scriptObject.text = "document.documentElement.addBehavior(\"foo.htc\");" +
                                                "document.documentElement.attachEvent(\"onreadystatechange\", Notify);";
                            ((HTMLHeadElement)head).appendChild((IHTMLDOMNode)scriptObject);
                             * */
                            //给window扩展一个插件属性
                            dynamic Window = ((IHTMLDocument2)browser.Document).parentWindow;
                            IExpando ScriptObject = (IExpando)Window;
                            PropertyInfo MyExtension = ScriptObject.GetProperty("MyExtension", BindingFlags.Default);
                            if (MyExtension == null) MyExtension = ScriptObject.AddProperty("MyExtension");
                            MyExtension.SetValue(ScriptObject, this, null);
                            
                            //修改his的js方法，让ajax返回调用window的扩展属性
                            HTMLDocument document = (HTMLDocument)browser.Document;                            
                            IHTMLElement body = (IHTMLElement)((IHTMLElementCollection)document.all.tags("body")).item(null, 0);
                            IHTMLScriptElement scriptObject = (IHTMLScriptElement)document.createElement("script");
                            scriptObject.type = @"text/javascript";
                            scriptObject.text = "Dispaly=function(){document.getElementById ('myTime').innerHTML = req.responseText;window.MyExtension.callBHO(req.responseText);}";
                            ((HTMLBody)body).appendChild((IHTMLDOMNode)scriptObject);
                                                         
                            /*((HTMLDocumentEvents_Event)(browser.Document)).ondataavailable += new HTMLDocumentEvents_ondataavailableEventHandler(dataavaliable);*/
                            /*browser.Navigate(@"javascript:Dispaly=new function(){document.getElementById ('myTime').innerHTML = req.responseText;window.MyExtension.callBHO('hello sam.');}");*/
                        }
                        else
                        {
                            crawl(urlStr);
                        }
                    }
                   
                    //获取登录名
                    else if (urlStr.Contains("/web/csp/epr.menu.csp?LogonFromVB="))
                    {
                        IHTMLDocument2 document = browser.Document;
                        foreach (IHTMLElement element in document.all)
                        {
                            if (element.tagName != null && element.tagName.ToUpper().Equals("HEAD"))
                            {
                                string headHTML = element.innerHTML;
                                //log.WriteLog("head string in epr.menu.csp:\n" + headHTML);
                                if (headHTML.Contains("session['LOGON.USERCODE']="))
                                {
                                    username = EpisodeRegexUtils.getFirstMatchedFromString(headHTML, @"session\['LOGON\.USERCODE'\]='(\w+?)'");
                                    log.WriteLog("当前登录用户名=" + username);
                                }
                            }
                        }
                    }
                }
                // @Eric Stob: Thanks for this hint!
                // This will prevent this method being executed more than once.
                if (pDisp != this.site)
                    return;    
            }
            catch (Exception ex)
            {
                log.WriteLog("DiagnoseAssistant1 OnDocumentComplete exception occured." + ex.ToString() + ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
        }
        
        #endregion
        
        //固定GUID
        [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        [InterfaceType(1)]
        public interface IServiceProvider
        {
            int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
        }
        #region Implementation of IObjectWithSite
        int IObjectWithSite.SetSite(object site)
        {
            this.site = site;
            try
            {                
                if (site != null)
                {
                    var serviceProv = (IServiceProvider)this.site;
                    var guidIWebBrowserApp = Marshal.GenerateGuidForType(typeof(IWebBrowserApp)); // new Guid("0002DF05-0000-0000-C000-000000000046");
                    var guidIWebBrowser2 = Marshal.GenerateGuidForType(typeof(IWebBrowser2)); // new Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E");
                    IntPtr intPtr;
                    serviceProv.QueryService(ref guidIWebBrowserApp, ref guidIWebBrowser2, out intPtr);

                    browser = (IWebBrowser2)Marshal.GetObjectForIUnknown(intPtr);                    

                    ((DWebBrowserEvents2_Event)browser).DocumentComplete +=
                        new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                    
               }
               else
               {
                    ((DWebBrowserEvents2_Event)browser).DocumentComplete -=
                        new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                    browser = null;
                }                
            }
            catch (Exception ex)
            {
                log.WriteLog("exception occured:" + ex.ToString());
                MessageBox.Show("exception occured:" + ex.ToString());
            }
            //log.WriteLog("browser.LocationURL:" + browser.LocationURL);
            return 0;
        }
        int IObjectWithSite.GetSite(ref Guid guid, out IntPtr ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(browser);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSite);
            Marshal.Release(punk);
            return hr;
        }
        #endregion

        #region Implementation of IOleCommandTarget
        
       
        int IOleCommandTarget.QueryStatus(IntPtr pguidCmdGroup, uint cCmds, ref OLECMD prgCmds, IntPtr pCmdText)
        {
            return 0;
        }
        int IOleCommandTarget.Exec(IntPtr pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            try
            {
                log.WriteLog("按钮无效，请勿点击，请运行AutoAccess程序");
            }
            catch (Exception ex)
            {
                log.WriteLog("获取到InternetExplorer异常" + ex.ToString() + ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
        
        #endregion       
       
        #region business methods

        public void dataavaliable()
        {
            log.WriteLog("dataavaliable");
        }
        /// <summary>
        /// js调用BHO方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string callBHO(string s)
        {
            log.WriteLog("In callBHO." + s);
            return null;
        }
        void crawl(string url)
        {
            //解析dom元素
            Crawler crawler = CrawlerFactory.getCrawler(url);
            if (crawler != null)
            {
                crawler.crawl(browser.Document);
            }
        }
        void registerMonitor(string url)
        {
            HTMLDocument document = browser.Document;
            ChangeMonitor monitor = new ChangeMonitor();
            monitor.url = url;
            monitor.document = document;
            IHTMLChangeSink changeSink = monitor;
            IHTMLChangeLog changeLog = null;

            ((IMarkupContainer2)document).CreateChangeLog(changeSink, out changeLog, 1, 1);
        }
        public class ChangeMonitor : IHTMLChangeSink
        {
            public HTMLDocument document { get; set; }
            public string url { get; set; }
            public void Notify()
            {
                MessageBox.Show("document.readyState: " + document.readyState);
                if ("4".Equals(document.readyState))
                {
                    MessageBox.Show("notified");
                    //解析dom元素
                    Crawler crawler = CrawlerFactory.getCrawler(url);
                    if (crawler != null)
                    {
                        IHTMLDocument2 doc = browser.Document;
                        log.WriteLog("document:\n" + doc.body.innerHTML);
                        //crawler.crawl(doc);
                    }
                }
                
            }
        }
        #endregion

        #region Registering with regasm
        public static string RegBHO = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";
        public static string RegCmd = "Software\\Microsoft\\Internet Explorer\\Extensions";

        [ComRegisterFunction]
        public static void RegisterBHO(Type type)
        {
            string guid = type.GUID.ToString("B");

            // BHO
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
                if (registryKey == null)
                    registryKey = Registry.LocalMachine.CreateSubKey(RegBHO);
                RegistryKey key = registryKey.OpenSubKey(guid);
                if (key == null)
                    key = registryKey.CreateSubKey(guid);
                key.SetValue("Alright", 1);
                registryKey.Close();
                key.Close();
            }

            // Command
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegCmd, true);
                if (registryKey == null)
                    registryKey = Registry.LocalMachine.CreateSubKey(RegCmd);
                RegistryKey key = registryKey.OpenSubKey(guid);
                if (key == null)
                    key = registryKey.CreateSubKey(guid);
                key.SetValue("ButtonText", "辅助诊疗");
                //固定GUID，IE Extension的CLSID
                key.SetValue("CLSID", "{1FBA04EE-3024-11d2-8F1F-0000F87ABD16}");
                key.SetValue("ClsidExtension", guid);
                key.SetValue("Icon", "C:\\fzzl\\DiagnoseAssistantIcon.ico");
                key.SetValue("HotIcon", "C:\\fzzl\\DiagnoseAssistantIcon.ico");
                key.SetValue("Default Visible", "Yes");
                key.SetValue("MenuText", "&辅助诊疗");
                key.SetValue("ToolTip", "辅助诊疗");
                //key.SetValue("KeyPath", "no");
                registryKey.Close();
                key.Close();
            }
        }
        
        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type)
        {
            string guid = type.GUID.ToString("B");
            // BHO
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
                if (registryKey != null)
                    registryKey.DeleteSubKey(guid, false);
            }
            // Command
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegCmd, true);
                if (registryKey != null)
                    registryKey.DeleteSubKey(guid, false);
            }
        }
        #endregion
    }
}
