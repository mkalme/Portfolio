using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Portfolio
{
    public partial class Form4 : Form
    {
        public static Form1 form;
        public static String text = "";

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Bought");
            comboBox1.Items.Add("Sold");
            comboBox1.Items.Add("Sold All");
            comboBox1.Items.Add("Stock Split");

            numericUpDown1.Visible = false;
            label3.Visible = false;

            timer1.Start();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            String path = Form1.currentPath + @"\" + form.dataGridView1.SelectedCells[1].Value.ToString();
            string[] lines = form.readFile(path + @"\information").Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

            if (text.Equals("Bought"))
            {
                int shares = Int32.Parse(form.dataGridView1.SelectedCells[3].Value.ToString());
                form.createFile(path + @"\actions", "Bought " + numericUpDown1.Value + " shares, from " + shares + " shares to " + (shares + numericUpDown1.Value) + " shares.", true);
                form.createFile(path + @"\information", lines[0] + "\r\n" + lines[1] + "\r\n" + (shares + Int32.Parse(numericUpDown1.Value.ToString())), false);
            }
            else if (text.Equals("Sold All"))
            {
                form.createFile(path + @"\actions", "All shares sold. ", true);
                form.createFile(path + @"\information", lines[0] + "\r\n" + lines[1] + "\r\n0", false);

            }
            else if (text.Equals("Sold"))
            {
                int shares = Int32.Parse(form.dataGridView1.SelectedCells[3].Value.ToString());
                form.createFile(path + @"\actions", "Sold " + numericUpDown1.Value + " shares, from " + shares + " shares to " + (shares - numericUpDown1.Value) + " shares.", true);
                form.createFile(path + @"\information", lines[0] + "\r\n" + lines[1] + "\r\n" + (shares - Int32.Parse(numericUpDown1.Value.ToString())), false);
            }
            else if (text.Equals("Stock Split"))
            {
                int shares = Int32.Parse(form.dataGridView1.SelectedCells[3].Value.ToString());
                form.createFile(path + @"\actions", "Stock Split " + numericUpDown1.Value + " to 1, from " + shares + " shares to " + (shares * numericUpDown1.Value) + " shares.", true);
                form.createFile(path + @"\information", lines[0] + "\r\n" + lines[1] + "\r\n" + (shares * Int32.Parse(numericUpDown1.Value.ToString())), false);
            }

            form.refreshDataGridView();
            Close();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            text = comboBox1.Text;
            enableControls();
        }

        private void enableControls() {
            if (text.Equals("Bought"))
            {
                numericUpDown1.Visible = true;
                label2.Visible = true;

                label3.Visible = false;

                label2.Text = "Bought how many shares";
            }
            else if(text.Equals("Sold All"))
            {
                numericUpDown1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
            }
            else if (text.Equals("Sold"))
            {
                numericUpDown1.Visible = true;
                label2.Visible = true;

                label3.Visible = false;

                label2.Text = "Sold how many shares";
            }
            else if (text.Equals("Stock Split"))
            {
                numericUpDown1.Visible = true;
                label3.Visible = true;

                label2.Visible = false;

                label3.Text = "for one";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (text.Equals("Sold All"))
            {
                doneButton.Enabled = true;
            }
            else {
                if (numericUpDown1.Value == 0 || string.IsNullOrEmpty(text))
                {
                    doneButton.Enabled = false;
                }
                else {
                    doneButton.Enabled = true;
                }
            }
            
        }
    }
}
