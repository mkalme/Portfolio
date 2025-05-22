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

namespace Portfolio
{
    public partial class Form2 : Form
    {
        public static Form1 form;

        public Form2()
        {
            InitializeComponent();

            timer1.Start();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            form.createFolder(Form1.currentPath + @"\" + textBox1.Text);
            form.refreshDataGridView();

            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String[] directories = Directory.GetDirectories(Form1.currentPath);

            if (!string.IsNullOrEmpty(textBox1.Text) && directories.Length > 0)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    if (Path.GetFileName(directories[i]).Equals(textBox1.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        doneButton.Enabled = false;
                    }
                    else
                    {
                        doneButton.Enabled = true;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(textBox1.Text) && directories.Length == 0)
            {
                doneButton.Enabled = true;
            }
            else {
                doneButton.Enabled = false;
            }
        }
    }
}
