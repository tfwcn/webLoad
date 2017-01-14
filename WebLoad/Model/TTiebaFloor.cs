using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad.Model
{
    public class TTiebaFloor
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
        /// 楼层内容
        /// </summary>
        public string FContent { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        public string FUser { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public int FFloor { get; set; }
        public string FTiebaId { get; set; }
        public string FTiebaItemId { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? FTime { get; set; }
    }
}
