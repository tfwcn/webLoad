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
    public partial class FMText : Form
    {
        public string Value { get; private set; }
        public FMText()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="value">值</param>
        /// <param name="isEdit">是否可编辑</param>
        public FMText(string title, string value, bool isEdit)
            : this()
        {
            this.Text = title;
            this.Value = value;
            txtValue.Text = this.Value;
            if (!isEdit)
            {
                panel1.Visible = false;
                txtValue.ReadOnly = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Value = txtValue.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
