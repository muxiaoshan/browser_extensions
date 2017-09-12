using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace DiagnoseAssistant1.crawler.OCR
{
    /// <summary>  
    /// 图片识别，验证识别  
    /// </summary>  
    public class Identify
    {
        /// <summary>  
        /// 功能描述：静态成员 识别数字、字母，验证码等图片上的内容  
        /// </summary>  
        /// <param name="img_path">待识别内容的图片</param>  
        /// <param name="save_dir">识别内容保存目录</param>  
        /// <param name="scale">放大处理比率（建议值范围：100~1000）-（参数：提高识别成功率）</param>  
        /// <returns></returns>  
        public static string StartIdentifyingCaptcha(string img_path, string save_dir, int scale)
        {
            return StartIdentifyingCaptcha(img_path, save_dir, 4, 0, scale);
        }
        /// <summary>  
        /// 功能描述：静态成员 识别数字、字母，验证码等图片上的内容  
        /// </summary>  
        /// <param name="img_path">待识别内容的图片</param>  
        /// <param name="save_dir">识别内容保存目录</param>  
        /// <param name="content_type">内容分类（0：纯数字 1：纯字母 2：数字与字母混合 3：纯汉字，4、中英文混合）-（参数：提高识别成功率）</param>  
        /// <param name="content_length">内容长度（参数：提高识别成功率）</param>  
        /// <param name="scale">放大处理比率（建议值范围：100~1000）-（参数：提高识别成功率）</param>  
        /// <returns></returns>  
        public static string StartIdentifyingCaptcha(string img_path, string save_dir, byte content_type, int content_length, int scale)
        {
            string captcha = "";            // 识别的验证码  

            //int captcha_length = 6;         // 验证码长度(参数：提高识别成功率)  

            //int scale = 730;                // 放大处理比率(参数：提高识别成功率)  

            
            // 识别验证码(一、安装ImageMagick，二、拷备tesseract目录至应用程序下)  
            Common.StartProcess("cmd.exe", new string[] {   // "cd tesseract" ,  //已把命令配置到path
                                                            // 输换图片  
                                                            String.Format(@"magick convert -compress none -depth 8 -alpha off -scale {0}% -colorspace gray {1} {2}\captcha.tif",scale,img_path,save_dir),  
                              
                                                            // 识别图片  
                                                            String.Format(@"tesseract {0}\captcha.tif {0}\{1} -l chi_sim+eng",save_dir, Path.GetFileNameWithoutExtension(img_path)),  
  
                                                            "exit"});

            // 读取识别的验证码
            StreamReader reader = new StreamReader(String.Format(@"{0}\{1}.txt", save_dir, Path.GetFileNameWithoutExtension(img_path)));

            captcha = reader.ReadToEnd();

            reader.Close();
                
            return captcha;
        }
    }  
}
