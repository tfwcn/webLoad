using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using CefSharp.WinForms;
using CefSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace WebLoad
{
    public partial class FMMain : Form
    {
        /// <summary>
        /// 运行状态
        /// </summary>
        private enum MRunState { Play, Stop }
        private MSetting mSetting;
        private string settingPath = "setData";
        private string strUrl;
        private ChromiumWebBrowser browser;
        /// <summary>
        /// 执行堆栈
        /// </summary>
        private List<MSetting.MSetItem> listPlay = new List<MSetting.MSetItem>();
        /// <summary>
        /// 插件列表
        /// </summary>
        private List<WebLoadPlugin.IPlugin> listPlugin = new List<WebLoadPlugin.IPlugin>();
        /// <summary>
        /// 运行状态
        /// </summary>
        private MRunState mRunState = MRunState.Stop;
        private DAL.DBHelper dbHelper;

        private BindingSource bsItems = new BindingSource();
        private BindingSource bsPlay = new BindingSource();
        private BindingSource bsPlugin = new BindingSource();
        private BindingSource bsPluginProperty = new BindingSource();

        public FMMain()
        {
            InitializeComponent();
            BrowserInit();
        }
        /// <summary>
        /// 初始化浏览器内核
        /// </summary>
        private void BrowserInit()
        {
            //strUrl = "http://tieba.baidu.com/f?kw=%E7%99%BD%E7%8C%ABproject&ie=utf-8";
            //strUrl += "&pn=0";
            browser = new ChromiumWebBrowser(strUrl)
            {
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled
                }
            };
            browser.Dock = DockStyle.Fill;
            browser.FrameLoadEnd += browser_FrameLoadEnd;
            pnlBrowser.Controls.Add(browser);
        }
        /// <summary>
        /// 初始化插件
        /// </summary>
        private void PluginInit()
        {
            //mSetting.ListPlugin.Clear();
            string pluginsDir = "Plugins";
            if (!Directory.Exists(pluginsDir))
            {
                Directory.CreateDirectory(pluginsDir);
            }
            foreach (var filePath in Directory.EnumerateFiles(pluginsDir, "*.dll"))
            {
                if (filePath.Length < 5 || filePath.ToLower().Substring(filePath.Length - 4) != ".dll")
                    continue;
                try
                {
                    Assembly assembly = Assembly.LoadFrom(filePath);
                    foreach (var type in assembly.GetTypes())
                    {
                        var interfaces = type.GetInterfaces();
                        if (type.GetInterfaces().ToList().Exists(t => t.FullName == "WebLoadPlugin.IPlugin"))
                        {
                            var tmpPlugin = assembly.CreateInstance(type.FullName) as WebLoadPlugin.IPlugin;
                            var nowPlugin = mSetting.ListPlugin.Find(m => m.PId == tmpPlugin.GetId());
                            if (nowPlugin != null)//存在设置记录
                            {
                                nowPlugin.PFilePath = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                                nowPlugin.PText = tmpPlugin.GetText();
                                if (nowPlugin.Propertys != null)//读取旧属性
                                {
                                    foreach (var p in nowPlugin.Propertys)
                                    {
                                        tmpPlugin.SetProperty(p.PID, p.PValue);
                                    }
                                }
                            }
                            else//不存在设置记录
                            {
                                mSetting.ListPlugin.Add(new MSetting.MSetPlugin()
                                {
                                    PId = tmpPlugin.GetId(),
                                    PFilePath = filePath.Substring(filePath.LastIndexOf("\\") + 1),
                                    PText = tmpPlugin.GetText(),
                                    PEnable = true,
                                    Propertys = tmpPlugin.GetPropertys()
                                });
                            }
                            listPlugin.Add(tmpPlugin);
                        }
                    }
                    //移除旧设置
                    mSetting.ListPlugin.RemoveAll(m => !listPlugin.Exists(m2 => m.PId == m2.GetId()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("错误文件：{0}，错误信息：{1}", filePath, ex.ToString()), "加载错误", MessageBoxButtons.OK);
                }
            }
        }

        private void FMMain_Load(object sender, EventArgs e)
        {
            LoadSetting(this.settingPath);
            PluginInit();
            bsItems.DataSource = mSetting.ListItems;
            bsPlay.DataSource = mSetting.ListPlay;
            bsPlugin.CurrentItemChanged += bsPlugin_CurrentItemChanged;
            bsPlugin.DataSource = mSetting.ListPlugin;
            dgvItems.AutoGenerateColumns = false;
            dgvItems.DataSource = bsItems;
            dgvPlayList.AutoGenerateColumns = false;
            dgvPlayList.DataSource = bsPlay;
            dgvPlugin.AutoGenerateColumns = false;
            dgvPlugin.DataSource = bsPlugin;
            dgvPluginProperty.AutoGenerateColumns = false;
            dgvPluginProperty.DataSource = bsPluginProperty;
        }

        private void bsPlugin_CurrentItemChanged(object sender, EventArgs e)
        {
            bsPluginProperty.DataSource = null;
            var mSetPlugin = bsPlugin.Current as MSetting.MSetPlugin;
            if (mSetPlugin != null)
            {
                bsPluginProperty.DataSource = mSetPlugin.Propertys;
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (((e.HttpStatusCode == 200) || (e.HttpStatusCode == 0 && e.Url == "about:blank")) && e.Url == strUrl)
            {
                MSetting.MSetItem mSetItem = listPlay.Last();
                var script = mSetItem.FJavascript;
                browser.GetFocusedFrame().GetResponseReturn(script).ContinueWith(task =>
                {
                    //string json = JsonConvert.SerializeObject(dt);
                    if (task.Result != null)
                    {
                        Action actionDelegate = () =>
                        {
                            txtMsg.Text += "\r\n\r\n=================================\r\n\r\n";
                            txtMsg.Text += task.Result;
                            txtMsg.Select(txtMsg.Text.Length, 0);
                        };
                        this.Invoke(actionDelegate, null);
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>(task.Result);
                        mSetItem.FJSData = dt;
                        if (!String.IsNullOrEmpty(mSetItem.FSql) && dbHelper != null)
                            dbHelper.RunSql(mSetItem.FSql, dt);//执行SQL
                    }
                    else
                    {
                        mSetItem.FJSData = null;
                    }

                    Run();
                });
                if (mRunState == MRunState.Stop)
                {
                    Action actionDelegate = () =>
                    {
                        MessageBox.Show("运行结束", "消息");
                        tsbtnPlay.Enabled = true;
                        tsbtnStop.Enabled = false;
                    };
                    this.Invoke(actionDelegate, null);
                }
            }
        }
        /// <summary>
        /// 运行（纵向遍历所有条件）
        /// </summary>
        private void Run()
        {
            if (mRunState == MRunState.Stop)
            {
                tsbtnPlay.Enabled = true;
                return;
            }
            MSetting.MSetItem mSetItem = listPlay.Last();
            if (mSetting.ListPlay.Last() != mSetItem)//执行下一条
            {
                MSetting.MSetItem mSetItemNext = mSetting.ListPlay[mSetting.ListPlay.IndexOf(mSetItem) + 1];
                mSetItemNext.FJSDataIndex = 0;
                listPlay.Add(mSetItemNext);
                if (mSetItem.FJSData != null && mSetItem.FJSData.Rows.Count > 0)//通过返回值进行替换
                {
                    strUrl = JArrayReplace(mSetItemNext.FUrl, mSetItem.FJSData.Rows[mSetItem.FJSDataIndex]);
                }
                else
                {
                    strUrl = mSetItemNext.FUrl;
                }
                browser.Load(strUrl);
            }
            else//返回上一条
            {
                while (listPlay.Count > 1)
                {
                    MSetting.MSetItem mSetItemNext = listPlay.Last();
                    mSetItemNext.FJSDataIndex = 0;
                    mSetItem = mSetting.ListPlay[mSetting.ListPlay.IndexOf(mSetItemNext) - 1];
                    if (mSetItem.FJSData != null && mSetItem.FJSDataIndex < mSetItem.FJSData.Rows.Count - 1)//循环返回值数组
                    {
                        mSetItem.FJSDataIndex++;
                        strUrl = JArrayReplace(mSetItemNext.FUrl, mSetItem.FJSData.Rows[mSetItem.FJSDataIndex]);
                        browser.Load(strUrl);
                        return;
                    }
                    else
                    {
                        listPlay.Remove(listPlay.Last());
                    }
                }
                Action actionDelegate = () =>
                {
                    mRunState = MRunState.Stop;
                    MessageBox.Show("运行结束", "消息");
                    tsbtnPlay.Enabled = true;
                    tsbtnStop.Enabled = false;
                };
                this.Invoke(actionDelegate, null);
            }
        }
        private string JArrayReplace(string val, DataRow row)
        {
            string retVal = val;
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                retVal = retVal.Replace("{" + row.Table.Columns[i].ColumnName + "}", row[i].ToString());
            }
            return retVal;
        }
        /// <summary>
        /// 读取设置
        /// </summary>
        private void LoadSetting(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        mSetting = (MSetting)formatter.Deserialize(stream);
                        stream.Close();
                    }
                    if (!String.IsNullOrEmpty(mSetting.DBConnStr))
                    {
                        if (mSetting.DBType == MSetting.EnumDBType.PostgreSQL)
                            dbHelper = new DAL.NpgsqlDBHelper(mSetting.DBConnStr);
                    }
                    else
                    {
                        dbHelper = null;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("错误：" + ex.Message, "消息");
                    mSetting = new MSetting();
                }
            }
            else
            {
                mSetting = new MSetting();
            }
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSetting(string path)
        {
            try
            {
                if (mSetting == null)
                    return;
                IFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, mSetting);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "消息");
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtFName.Text.Trim() == "")
            {
                MessageBox.Show("请录入名称", "消息");
                return;
            }
            if (txtFUrl.Text.Trim() == "")
            {
                MessageBox.Show("请录入URL", "消息");
                return;
            }
            if (mSetting.ListItems.Exists(m => m.FName == txtFName.Text.Trim()))
            {
                if (MessageBox.Show("名称已存在，是否覆盖？", "消息", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
                MSetting.MSetItem mSetItem = mSetting.ListItems.Find(m => m.FName == txtFName.Text.Trim());
                mSetItem.FName = txtFName.Text.Trim();
                mSetItem.FUrl = txtFUrl.Text.Trim();
                mSetItem.FJavascript = txtFJavascript.Text;
                mSetItem.FSql = txtFSql.Text;
            }
            else
            {
                MSetting.MSetItem mSetItem = new MSetting.MSetItem();
                mSetItem.FName = txtFName.Text.Trim();
                mSetItem.FUrl = txtFUrl.Text.Trim();
                mSetItem.FJavascript = txtFJavascript.Text;
                mSetItem.FSql = txtFSql.Text;
                mSetting.ListItems.Add(mSetItem);
            }
            bsItems.ResetBindings(false);
            SaveSetting(this.settingPath);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (bsItems.Current != null)
                bsItems.RemoveCurrent();
            SaveSetting(this.settingPath);
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            txtFName.Text = "";
            txtFUrl.Text = "";
            txtFJavascript.Text = "";
            txtFSql.Text = "";
        }

        private void dgvItems_Click(object sender, EventArgs e)
        {
            if (bsItems.Current != null)
            {
                MSetting.MSetItem mSetItem = bsItems.Current as MSetting.MSetItem;
                txtFName.Text = mSetItem.FName;
                txtFUrl.Text = mSetItem.FUrl;
                txtFJavascript.Text = mSetItem.FJavascript;
                txtFSql.Text = mSetItem.FSql;
            }
        }

        private void tsbtnTop_Click(object sender, EventArgs e)
        {
            if (bsPlay.Current != null)
            {
                MSetting.MSetItem mSetItem = bsPlay.Current as MSetting.MSetItem;
                mSetting.ListPlay.Remove(mSetItem);
                mSetting.ListPlay.Insert(0, mSetItem);
                bsPlay.ResetBindings(false);
                bsPlay.Position = 0;
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnUp_Click(object sender, EventArgs e)
        {
            if (bsPlay.Current != null)
            {
                MSetting.MSetItem mSetItem = bsPlay.Current as MSetting.MSetItem;
                int i = mSetting.ListPlay.IndexOf(mSetItem);
                mSetting.ListPlay.Remove(mSetItem);
                if (i > 0) i--;
                mSetting.ListPlay.Insert(i, mSetItem);
                bsPlay.ResetBindings(false);
                bsPlay.Position = i;
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnAdd_Click(object sender, EventArgs e)
        {
            if (bsItems.Current != null)
            {
                MSetting.MSetItem mSetItem = bsItems.Current as MSetting.MSetItem;
                if (mSetting.ListPlay.Exists(m => m == mSetItem))
                {
                    MessageBox.Show("该条件已添加", "消息");
                    return;
                }
                mSetting.ListPlay.Add(mSetItem);
                bsPlay.ResetBindings(false);
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnRemove_Click(object sender, EventArgs e)
        {
            if (bsPlay.Current != null)
            {
                bsPlay.RemoveCurrent();
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnDown_Click(object sender, EventArgs e)
        {
            if (bsPlay.Current != null)
            {
                MSetting.MSetItem mSetItem = bsPlay.Current as MSetting.MSetItem;
                int i = mSetting.ListPlay.IndexOf(mSetItem);
                mSetting.ListPlay.Remove(mSetItem);
                if (i < mSetting.ListPlay.Count()) i++;
                mSetting.ListPlay.Insert(i, mSetItem);
                bsPlay.ResetBindings(false);
                bsPlay.Position = i;
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnBottom_Click(object sender, EventArgs e)
        {
            if (bsPlay.Current != null)
            {
                MSetting.MSetItem mSetItem = bsPlay.Current as MSetting.MSetItem;
                mSetting.ListPlay.Remove(mSetItem);
                mSetting.ListPlay.Insert(mSetting.ListPlay.Count(), mSetItem);
                bsPlay.ResetBindings(false);
                bsPlay.Position = mSetting.ListPlay.Count() - 1;
                SaveSetting(this.settingPath);
            }
        }

        private void tsbtnPlay_Click(object sender, EventArgs e)
        {
            if (mSetting.ListPlay.Count > 0)
            {
                mRunState = MRunState.Play;
                MSetting.MSetItem mSetItem = mSetting.ListPlay[0];
                mSetItem.FJSDataIndex = 0;
                listPlay.Clear();
                listPlay.Add(mSetItem);
                strUrl = mSetItem.FUrl;
                browser.Load(strUrl);
                tsbtnPlay.Enabled = false;
                tsbtnStop.Enabled = true;
                txtMsg.Text = "";
                tabControl1.SelectedIndex = 1;
            }
        }

        private void tsbtnStop_Click(object sender, EventArgs e)
        {
            mRunState = MRunState.Stop;
            tsbtnStop.Enabled = false;
        }

        private void dgvPlugin_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPluginProperty_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
