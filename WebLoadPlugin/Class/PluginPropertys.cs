using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoadPlugin.Class
{
    /// <summary>
    /// 插件属性
    /// </summary>
    public class PluginPropertys
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string PText { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string PValue { get; set; }
    }
}
