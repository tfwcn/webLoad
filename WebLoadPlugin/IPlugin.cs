using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLoadPlugin.Class;

namespace WebLoadPlugin
{
    /// <summary>
    /// 插件接口（必须）
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 获取插件标识（唯一值，用于保存和还原设置）
        /// </summary>
        string GetId();
        /// <summary>
        /// 获取插件说明
        /// </summary>
        string GetText();
        /// <summary>
        /// 初始化设置
        /// </summary>
        void SetProperty(string PID, string PValue);
        /// <summary>
        /// 所需设置，界面可录入，可保存状态
        /// </summary>
        List<PluginPropertys> GetPropertys();
        /// <summary>
        /// 获取结果，并执行后续操作
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="name">步骤名</param>
        /// <param name="sql">sql</param>
        /// <param name="result">返回值</param>
        void Run(string url, string name, string sql, string result);
    }
}
