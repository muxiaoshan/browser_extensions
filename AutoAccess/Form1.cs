using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WatiN.Core;

namespace AutoAccess
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        Log log = new Log();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            autoaccess();
        }

        
        private void autoaccess()
        {
            try
            {
                //实例化IE类，可以把实例化的ie看成是页面，以后的操作基本都是它打交道
                IE ie = new IE("http://172.26.100.35/dthealth/web/csp/logon.csp?LANGID=1&USERNAME=2394&PASSWORD=898747", true);

                ie.Link(Find.ById("Logon")).Click();

                
                IE ieAfterLogon = IE.AttachTo<IE>(Find.ByUrl("http://172.26.100.35/dthealth/web/csp/logon.csp"));                
                Frame TRAK_main = ieAfterLogon.Frame(Find.ByName("TRAK_main"));
                Table tDHCDocInPatientList = TRAK_main.Table("tDHCDocInPatientList");                
                TableRowCollection trs = tDHCDocInPatientList.TableRows;
                for (int i = 1; i < trs.Count; i++)
                {
                    viewPatientScanReport(i);
                }
            }
            catch (Exception ex)
            {

                log.WriteLog("访问HIS出错，" + ex.ToString() + ex.StackTrace + ex.Source);
            }
        }
        
        private void viewPatientScanReport(int patientIdx)
        {        
            IE ieAfterLogon = IE.AttachTo<IE>(Find.ByUrl("http://172.26.100.35/dthealth/web/csp/logon.csp"));            
            Frame eprmenu = ieAfterLogon.Frame(Find.ByName("eprmenu"));
            Frame TRAK_main = ieAfterLogon.Frame(Find.ByName("TRAK_main"));
            Table tDHCDocInPatientList = TRAK_main.Table("tDHCDocInPatientList");
            TableRowCollection trs = tDHCDocInPatientList.TableRows;
            //第一行为标题，从第二行开始
            TableRow tr = trs[patientIdx];
            string patientName = tr.TableCells[5].Text;
            log.WriteLog("登记号：" + tr.TableCells[2].Text + "，患者姓名：" + patientName);
            tr.Click();

            TableCell menuTc = eprmenu.TableCell("tb57");
            menuTc.Links[0].Click();
            log.WriteLog("进入患者医嘱录入界面。患者姓名：" + patientName);
            /*
            Frame maindata = TRAK_main.Frame("maindata");
            log.WriteLog("maindata frame name:" + maindata.Name);
                //无法将类型为“mshtml.HTMLDocumentClass”的 COM 对象强制转换为接口类型“mshtml.DispHTMLDocument”。此操作失败的原因是对 IID 为“{3050F55F-98B5-11CF-BB82-00AA00BDCE0B}”的接口的 COM 组件调用 QueryInterface 因以下错误而失败: 不支持此接口 (异常来自 HRESULT:0x80004002 (E_NOINTERFACE))
                //要重新附加浏览器
            * */
            /*
            Div scanResultDiv = TRAK_main.Div("chart23");
            //WatiN.Core.Exceptions.ElementNotFoundException: Could not find DIV element tag matching criteria: Attribute 'id' equals 'chart23' at http://172.26.100.35/dthealth/web/csp/logon.csp                     
            * */
            //ie跳转后重新获取ie界面，不然无法获取frame内部frame
            IE ieAfterPatientSelected = IE.AttachTo<IE>(Find.ByUrl("http://172.26.100.35/dthealth/web/csp/logon.csp"));
            TRAK_main = ieAfterPatientSelected.Frame(Find.ByName("TRAK_main"));
            //访问frame内部的frame
            Frame maindata = TRAK_main.Frame("maindata");
            Div scanResultDiv = maindata.Div("chart23");
            scanResultDiv.Click();
            //打开检查结果列表
            Frame dataframe = maindata.Frame("dataframe");
            Table tDHCRisclinicQueryOEItem = dataframe.Table("tDHCRisclinicQueryOEItem");
            TableRowCollection scans = tDHCRisclinicQueryOEItem.TableRows;
            //第一行为标题，从第二行开始
            for (int scanIdx = 1; scanIdx < scans.Count; scanIdx++)
            {
                TableRow scan = scans[scanIdx];
                Link lk = null;
                foreach (TableCell tc in scan.TableCells)
                {
                    if ("报告".Equals(tc.Text))
                    {
                        lk = tc.Links[0];
                    }
                }
                if (lk != null)
                {
                    string reportName = scan.Label("TItemNamez" + scanIdx).Text;
                    log.WriteLog("打开【" + reportName + "】报告窗口");
                    lk.Click();
                    Regex rxPEID = new Regex(@"http://172[.]26[.]102[.]\d{1,3}/.+");
                    IE newWind = null;
                    try
                    {
                        //AttachToNoWait不抛异常，以用于后面的强制关闭，参考https://sourceforge.net/p/watin/feature-requests/95/
                        newWind = IE.AttachToNoWait<IE>(Find.ByUrl(rxPEID));
                        
                        //内含iframe时，将抛出异常WatiN.Core.Exceptions.TimeoutException: Timeout while waiting for frame document becoming available
                        newWind.WaitForComplete(180);//请求扫描等报告时，等待完成，超时时间180s
                        
                        newWind.Close();
                        log.WriteLog("正常关闭【" + reportName + "】报告窗口");
                    }
                    catch (Exception ex)
                    {
                        log.WriteLog("无法获取并关闭【" + reportName + "】报告窗口，稍后强制关闭：" + ex.ToString() + ex.StackTrace);
                        try
                        {                            
                            newWind.Close();
                            log.WriteLog("强制关闭【" + reportName + "】报告窗口");
                        }
                        catch (Exception ex1)
                        {
                            log.WriteLog("强制关闭【" + reportName + "】报告窗口异常：" + ex1.ToString() + ex1.StackTrace);
                        }
                    }
                }
            }
            //处理下一个患者
            eprmenu = ieAfterPatientSelected.Frame(Find.ByName("eprmenu"));
            //进入患者列表
            TableCell patientListMenuTc = eprmenu.TableCell("tb49");
            patientListMenuTc.Links[0].Click();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IE ie = new IE("http://localhost:8080/sysz-portal-web/a/login");
            ie.TextField(Find.ById("username")).TypeText("admin");
            ie.TextField(Find.ById("password")).TypeText("adminszyxm");
            Div loginDiv = ie.Div(Find.ByClass("loginicon"));
            try
            {
                loginDiv.Links[0].Click();
            }
            catch (WatiN.Core.Exceptions.TimeoutException te)
            {
                 log.WriteLog("登录水源超时：" + te.ToString());
            }
            Frame iframe = ie.Frame(Find.ById("iframe"));
            log.WriteLog("iframe.name:" + iframe.Name);
            
            try
            {
                //跳转到统计报表-基本情况一览表
                //效果不一样,ie.link().Click()丢失左侧菜单
                //ie.Link(Find.ByText("基本情况一览")).Click();
                //iframe.Link(Find.ByText("基本情况一览")).Click();
                //跳转到水源信息模块
                ie.Link(Find.By("navid", "bf24df5f99ab4477b4cfaf3b29dbaa1d")).Click();
            }
            catch (Exception te)
            {
                //log.WriteLog("访问基本情况一览表异常：" + te.ToString());
                log.WriteLog("访问水源信息异常：" + te.ToString());
            }
            Div waterDiv = ie.Div(Find.ById("QueryResultTable"));
            int i = 0;
            foreach (TableRow tr in waterDiv.TableRows)
            {
                if (i++ > 2)
                {
                    break;
                }
                tr.Links[0].Click();

            }
            try
            {
                //ie.Link(Find.ByText("自备水源数量及类型统计")).Click();
            }
            catch (Exception te)
            {
                //log.WriteLog("访问自备水源数量及类型统计表异常：" + te.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SHDocVw.InternetExplorerClass iec = new SHDocVw.InternetExplorerClass();
                MessageBox.Show("构造SHDocVw.InternetExplorerClass成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("new SHDocVw.InternetExplorerClass：" + ex.ToString());
                log.WriteLog(ex.ToString());
            }
        }
    }
}
