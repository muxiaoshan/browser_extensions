using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagnoseAssistant1
{
    public class Episode
    {
        //患者编码
        public string PatientID { get; set; }
        //就诊编码
        public string EpisodeID { get; set; }
        override
        public string ToString()
        {
            return "[PatientID=" + PatientID + ", EpisodeID=" + EpisodeID + "]";
        }
    }
}
