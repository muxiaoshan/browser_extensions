using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoAccess
{
    public class RegexUtils
    {
        /// <summary>
        /// 判断str是否匹配对应的正则表达式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="regex"></param>
        /// <returns>matched true, otherwise false.</returns>
        public static bool matchString(string str, string regex)
        {
            Regex rxPEID = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = rxPEID.Match(str);
            if (m.Success)
            {
                return true;
            }
            return false;
        }
    }
}
