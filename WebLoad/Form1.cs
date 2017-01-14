using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebLoad
{
    public partial class Form1 : Form
    {
        private string strUrl;
        private Timer timer = new Timer();
        private ChromiumWebBrowser browser;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Tick += timer_Tick;
            timer.Interval = 100;

            strUrl = "http://tieba.baidu.com/f?kw=%E7%99%BD%E7%8C%ABproject&ie=utf-8";
            strUrl += "&pn=0";
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
            this.Controls.Add(browser);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var script =
                @"(function () {
                    var n = $('#thread_list>.j_thread_list').find('a.j_th_tit,.threadlist_abs');
                    var data=new Array();
                    for(var i=0;i< n.length;i++){
                        var href= n.get(i).href;
                        var title=n.get(i).innerText;
                        i++;
                        var content=n.get(i).innerText;
                        data.push({href:href,title:title,content:content});
                    }
                    return JSON.stringify(data);
                })();";
            browser.GetFocusedFrame().GetResponseReturn(script).ContinueWith(task =>
            {
                List<MData> data = JSON.parse<List<MData>>(task.Result);
                string s = data[0].href;
            });
            timer.Stop();
        }

        void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //e.Frame.GetSource(new MyStringVisitor());
            if (e.HttpStatusCode == 200 && e.Url == strUrl)
            {
                timer.Start();
            }
        }

        class MyStringVisitor : IStringVisitor
        {

            public void Visit(string str)
            {
                Console.WriteLine(str);
            }

            public void Dispose()
            {

            }
        }

    }
}
