using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            try
            {
                //实例化IE类，可以把实例化的ie看成是页面，以后的操作基本都是它打交道
                IE ie = new IE("http://172.26.100.35/dthealth/web/csp/logon.csp?LANGID=1&USERNAME=2394&PASSWORD=898747");
                
                ie.Link(Find.ById("Logon")).Click();
                /*
                 * COM对象与其基础RCW分开后就不能再使用
                log.WriteLog("登录后的Frames:" + ie.Frames);
                 * */        
                log.WriteLog("获取新的ie窗体");
                IE ieAfterLogon = IE.AttachTo<IE>(Find.ByUrl("http://172.26.100.35/dthealth/web/csp/logon.csp"));
                log.WriteLog("登录后的IE url:" + ieAfterLogon.Url);
                log.WriteLog("登录后的Frames:" + ieAfterLogon.Frames);

                Frame TRAK_main = ieAfterLogon.Frame(Find.ByName("TRAK_main"));
                Table tDHCDocInPatientList = TRAK_main.Table("tDHCDocInPatientList");
                log.WriteLog("Test if iframe accessed. tDHCDocInPatientList ClassName:" + tDHCDocInPatientList.ClassName);


                
            }
            catch (Exception ex)
            {

                log.WriteLog("访问HIS出错，" + ex.ToString() + ex.StackTrace);
            }
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
            //ie.Link(Find.ByText("基本情况一览")).Click();//效果不一样,ie.link().Click()丢失左侧菜单
            try
            {
                iframe.Link(Find.ByText("基本情况一览")).Click();
            }
            catch (WatiN.Core.Exceptions.TimeoutException te)
            {
                log.WriteLog("访问基本情况一览表超时：" + te.ToString());
            }
        }
    }
}
