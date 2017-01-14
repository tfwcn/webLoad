using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad
{
    /// <summary>
    /// 设置类（界面所有设置）
    /// </summary>
    [Serializable]
    public class MSetting
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum EnumDBType { PostgreSQL }
        /// <summary>
        /// 条件，步骤
        /// </summary>
        [Serializable]
        public class MSetItem
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string FName { get; set; }
            /// <summary>
            /// URL
            /// </summary>
            public string FUrl { get; set; }
            /// <summary>
            /// 筛选条件(JS)
            /// </summary>
            public string FJavascript { get; set; }
            /// <summary>
            /// 插入语句(SQL)
            /// </summary>
            public string FSql { get; set; }
            /// <summary>
            /// 运行时的返回值
            /// </summary>
            public DataTable FJSData { get; set; }
            /// <summary>
            /// 运行时的返回值游标
            /// </summary>
            public int FJSDataIndex { get; set; }
        }
        /// <summary>
        /// 插件
        /// </summary>
        [Serializable]
        public class MSetPlugin
        {
            /// <summary>
            /// 插件标识（唯一值，用于保存和还原设置）
            /// </summary>
            public string PId { get; set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string PFilePath { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            public string PText { get; set; }
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool PEnable { get; set; }
            /// <summary>
            /// 插件设置
            /// </summary>
            public List<WebLoadPlugin.Class.PluginPropertys> Propertys { get; set; }
        }
        /// <summary>
        /// 条件列表
        /// </summary>
        public List<MSetItem> ListItems { get; set; }
        /// <summary>
        /// 执行顺序
        /// </summary>
        public List<MSetItem> ListPlay { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public EnumDBType DBType { get; set; }
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public string DBConnStr { get; set; }
        /// <summary>
        /// 保存的插件设置
        /// </summary>
        public List<MSetPlugin> ListPlugin { get; set; }

        public MSetting()
        {
            ListItems = new List<MSetItem>();
            ListPlay = new List<MSetItem>();
            ListPlugin = new List<MSetPlugin>();
        }
    }
}
