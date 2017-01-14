using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoadTeiBa
{
    public class PluginII : WebLoadPlugin.IPlugin
    {
        private List<WebLoadPlugin.Class.PluginPropertys> listProperty = new List<WebLoadPlugin.Class.PluginPropertys>();
        public string GetId()
        {
            return this.GetType().FullName;
        }

        public string GetText()
        {
            return "插件2";
        }

        public void SetProperty(string PID, string PValue)
        {
            throw new NotImplementedException();
        }

        public List<WebLoadPlugin.Class.PluginPropertys> GetPropertys()
        {
            listProperty.Add(new WebLoadPlugin.Class.PluginPropertys() { PID = "1", PName = "sx1", PText = "sxms1" });
            listProperty.Add(new WebLoadPlugin.Class.PluginPropertys() { PID = "2", PName = "sx2", PText = "sxms2" });
            return listProperty;
        }

        public void Run(string url, string name, string sql, string result)
        {
            throw new NotImplementedException();
        }
    }
}
