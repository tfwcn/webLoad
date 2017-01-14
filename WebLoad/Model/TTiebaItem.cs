using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad.Model
{
    public class TTiebaItem
    {
        public string FId { get; set; }
        public DateTime? FCreateTime { get; set; }
        public DateTime? FUpdateTime { get; set; }
        public int FVersion { get; set; }
        /// <summary>
        /// 帖子地址
        /// </summary>
        public string FUrl { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string FTitle { get; set; }
        /// <summary>
        /// 一楼内容
        /// </summary>
        public string FContent { get; set; }
        /// <summary>
        /// 发帖人
        /// </summary>
        public string FUser { get; set; }
        public string FTiebaId { get; set; }
        /// <summary>
        /// 发帖时间
        /// </summary>
        public DateTime? FTime { get; set; }
    }
}
