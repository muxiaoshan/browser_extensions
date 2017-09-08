using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagnoseAssistant1.crawler.entity
{
    /// <summary>
    /// 检查结果实体对象
    /// </summary>
    public class ScanEntity
    {
        public string PatientID { get; set; }//患者编码
        public string EpisodeID { get; set; }//就诊编码
        public string TStudyNoz {get; set;}//检查号
        public string TItemNamez {get; set;}//医嘱名称
        public string TItemDatez { get; set; }//医嘱日期，数据库开单时间
        public string TItemStatusz {get; set;}//是否发布
        public string TOEOrderDrz {get; set;}//OEOrderDr
        public string TIsIllz { get; set; }//是否阳性
        public string TLocNamez { get; set; }//检查科室
        public string TreplocDrz { get; set; }//科室DR
        public string TIshasImgz { get; set; }//是否有图像
        public string TMediumNamez { get; set; }//介质
        public string Memoz { get; set; }//备注
        public string YXBXHJCSJ { get; set; }//影像表现或检查所见
        public string JCZDHTS { get; set; }//检查诊断或提示
        public string JCSJ { get; set; }//检查时间
    }
}
