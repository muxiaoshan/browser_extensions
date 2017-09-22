using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiagnoseAssistant1
{
    public class RegexTest
    {
        public static void Main()
        {
            test7();
        }
        static void test1()
        {
            // Define a regular expression for repeated words.
            Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Define a test string.        
            string text = "The the quick brown fox  fox jumped over the lazy dog dog.";

            // Find matches.
            MatchCollection matches = rx.Matches(text);

            // Report the number of matches found.
            Console.WriteLine("{0} matches found.", matches.Count);

            // Report on each match.
            foreach (Match match in matches)
            {
                string word = match.Groups["word"].Value;
                int index = match.Index;
                Console.WriteLine("{0} repeated at position {1}", word, index);
            }
        }
        static void test2()
        {
            string url = "http://172.26.100.32/dthealth/web/csp/websys.csp?a=a&TMENU=50214&TPAGID=3242735760&TWKFLJ=websys.csp^1363680009&PatientID=783889&EpisodeID=2405124&mradm=2405124&WardID=69";
            Regex rxPEID = new Regex(@"PatientID=(\d+)&EpisodeID=(\d+)&mradm=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match(url);
            //int matchCount = 0;

            //此属性判断是否匹配成功
            while (m.Success)
            {
                //匹配的个数
                //Console.WriteLine("Match" + (++matchCount));

                //这里为什么要从下标1开始，因为下面获取组时：
                //下标0为一个整组，是根据匹配规则“(\w+)\s+(car)”获取的整组
                //下标1为第一个小括号里面的数据
                //下标2为第二个括号里面的数据....依次论推
                for (int i = 1; i <= 2; i++)
                {
                    //获取由正则表达式匹配的组的集合,这行代码相当于下面两句
                    //GroupCollection gc = mt.Groups;
                    //Group g = gc[i];
                    Group g = m.Groups[i];
                    //输出
                    Console.WriteLine("Group" + i + "='" + g + "'");

                    //获取由捕获组匹配的所有捕获的集合
                    CaptureCollection cc = g.Captures;
                    for (int j = 0; j < cc.Count; j++)
                    {
                        Capture c = cc[j];

                        System.Console.WriteLine("Capture" + j + "='" + c + "', Position=" + c.Index);
                    }
                }
                //匹配下一个
                m = m.NextMatch();
            }
        }

        static void test3()
        {
            string url = "http://172.26.102.4/RisWeb3/ReportContent.aspx?OID=2415677||29&USERID=2452&SID=1209052&LOC=549&STYLE=RIS3-4";
            string regex = @"RisWeb3/ReportContent[.]aspx(.+?)LOC=549[&]STYLE=RIS3[-]4$";
            Regex rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match(url);
            if (m.Success)
            {
                Console.WriteLine("匹配成功");
            }
            else
            {
                Console.WriteLine("匹配失败");
            }
        }
        static void test4()
        {
            string str = " 检查所见: \n\n" +

 " UCGB型：左心房内径29mm，左心室内径46mm，右心房、右心室大小正常范围。房室间隔连续性无中断，室间隔厚10mm，左心室后壁厚9mm,室壁运动正常、协调。各瓣膜回声正常，活动自如。主动脉根部内径28mm，壁回声正常，心包膜腔未见明显分离暗区。M型：二尖瓣前叶曲线呈双峰，E峰＜A峰；主动脉根部运动曲线主波存在，重搏波低平。CDFI：房室间隔未见明显过隔血流信号；各瓣口血流信号未见明显异常。 心功能检测：二尖瓣前叶曲线EF斜率50mm/s，EPSS6mm；左室EF值76%，FS45%，SV104ml/次，HR70次/分，节律齐；左室流出道血流Vmax:143cm/s，二尖瓣口血流频谱E峰57cm/s,A峰88cm/s。 ";
            string regex = @"检查所见: \n+(.+)$";
            Regex rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = rxPEID.Match(str);
            if (m.Success)
            {
                Console.WriteLine("匹配成功:" + m.Groups[1].ToString());
            }
            else
            {
                Console.WriteLine("匹配失败。");
            }
        }
        static void test5()
        {
            StreamReader sr = new StreamReader("f:/input.txt", Encoding.UTF8);
            
            string str = sr.ReadToEnd();
            
            Console.WriteLine("原文：【"+str+"】");
            string regex = @"^\s*检查所见:\s*\n+(.+)$";
            Regex rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = rxPEID.Match(str);
            if (m.Success)
            {
                Console.WriteLine("匹配成功:" + m.Groups[1].ToString());
            }
            else
            {
                Console.WriteLine("匹配失败。");
            }
        }
        static void test6()
        {
            Regex rxPEID = new Regex(@"/(\w+?)\.pdf$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match("http://172.26.102.4/ClinicRptView/2017-08-29/0000792159_%e5%91%a8%e6%99%ae%e7%94%9f/E20170831-012/E20170831-012.pdf");
            if (m.Success)
            {
                Console.WriteLine("匹配成功:" + m.Groups[1].ToString());
            }
            else
            {
                Console.WriteLine("匹配失败。");
            }
        }
        static void test7()
        {
            string str = "http://172.26.102.4/ClinicRptView/2017-08-29/0000792159_%e5%91%a8%e6%99%ae%e7%94%9f/E20170831-012/E20170831-012.pdf";
            int dotIdx = str.LastIndexOf(".");
            int slashIdx = str.LastIndexOf("/");
            Console.WriteLine("dotIdx=" + dotIdx + ",slashIdx=" + slashIdx);
            string filename = str.Substring(slashIdx + 1, (dotIdx - slashIdx - 1));
            Console.WriteLine("filename=" + filename);
        }
    }
}
