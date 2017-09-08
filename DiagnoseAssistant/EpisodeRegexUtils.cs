using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DiagnoseAssistant1
{
    public class EpisodeRegexUtils
    {
        public static Episode getEpisodeFromUrl(string url)
        {
            Regex rxPEID = new Regex(@"PatientID=(\d+?)[&]EpisodeID=(\d+?)[&]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match(url);

            Episode epi = null;
            //此属性判断是否匹配成功
            while (m.Success)
            {
                epi = new Episode();
                //这里为什么要从下标1开始，因为下面获取组时：
                //下标0为一个整组，是根据匹配规则“(\w+)\s+(car)”获取的整组
                //下标1为第一个小括号里面的数据
                //下标2为第二个括号里面的数据....依次论推
                
                Group g1 = m.Groups[1];
                epi.PatientID = g1.ToString();

                Group g2 = m.Groups[2];
                epi.EpisodeID = g2.ToString();
                    
                //匹配下一个
                m = m.NextMatch();
            }
            return epi;
        }
        /// <summary>
        /// 判断url是否匹配对应的正则表达式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="regex"></param>
        /// <returns>matched true, otherwise false.</returns>
        public static bool matchUrl(string url, string regex)
        {
            Regex rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match(url);
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 从string中获取指定第一个()内的内容
        /// </summary>
        /// <param name="str">待匹配字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static string getFirstMatchedFromString(string str, string regex)
        {
            return getFirstMatchedFromString(str, regex, false);
        }
        /// <summary>
        /// 从string中获取指定第一个()内的内容
        /// </summary>
        /// <param name="str">待匹配字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="multiline">是否跨行匹配</param>
        /// <returns></returns>
        public static string getFirstMatchedFromString(string str, string regex, bool multiline)
        {
            Regex rxPEID;
            if (multiline)
            {
                rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            
            Match m = rxPEID.Match(str);
            if (m.Success)
            {
                return m.Groups[1].ToString();
            }
            return null;
        }
    }
}
