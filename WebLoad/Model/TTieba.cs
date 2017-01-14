using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad.Model
{
    public class TTieba
    {
        public string FId { get; set; }
        public DateTime? FCreateTime { get; set; }
        public DateTime? FUpdateTime { get; set; }
        public int FVersion { get; set; }
        /// <summary>
        /// 贴吧地址
        /// </summary>
        public string FUrl { get; set; }
        /// <summary>
        /// 贴吧名
        /// </summary>
        public string FTitle { get; set; }
    }
}
