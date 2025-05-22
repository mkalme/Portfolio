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
    public partial class Form3 : Form
    {
        public static Form1 form;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Done_Click(object sender, EventArgs e)
        {
            form.createFolder(Form1.currentPath + @"\" + textBox2.Text);
            form.createFile(Form1.currentPath + @"\" + textBox2.Text + @"\information", textBox1.Text + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n" + numericUpDown1.Value, false);
            form.createFile(Form1.currentPath + @"\" + textBox2.Text + @"\actions", "All shares bought. ", false);
            form.refreshDataGridView();
            Close();
        }

        private bool checkIfEmpty() {
            bool ifEmpty = true;

            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && numericUpDown1.Value != 0) {
                ifEmpty = false;
            }

            return ifEmpty;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String[] directories = Directory.GetDirectories(Form1.currentPath);

            if (!checkIfEmpty() && directories.Length > 0)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    if (Path.GetFileName(directories[i]).Equals(textBox2.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        doneButton.Enabled = false;
                    }
                    else
                    {
                        doneButton.Enabled = true;
                    }
                }
            }
            else if (!checkIfEmpty() && directories.Length == 0)
            {
                doneButton.Enabled = true;
            }
            else
            {
                doneButton.Enabled = false;
            }
        }
    }
}
