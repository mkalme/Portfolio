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
using System.Diagnostics;

namespace Portfolio
{
    public partial class Form5 : Form
    {
        public static Form1 form;

        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            monthCalendar1.MinDate = DateTime.Parse(getOldest());
            monthCalendar1.MaxDate = DateTime.Now;
            monthCalendar1.SetDate(DateTime.Parse(Form1.timeNow));
        }

        public String getOldest() {
            String date = "";

            String path = Form1.basePath;

            for (int i = 0; i < 3; i++) {
                String[] directories = Directory.GetDirectories(path);
                int[] myInts = new int[directories.Length];
                for (int b = 0; b < directories.Length; b++)
                {
                    myInts[b] = Int32.Parse(Path.GetFileName(directories[b]));
                }

                date += (myInts.Min()).ToString() + (i == 2 ? "" : "-");
                path += @"\" + (myInts.Min()).ToString();
            }

            return date;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            String date = monthCalendar1.SelectionRange.Start.ToString("yyyy-MM-dd");

            if (Form1.inPortfolio)
            {
                String path = Form1.basePath + @"\" + DateTime.Parse(date).Year + @"\" + DateTime.Parse(date).Month + @"\" + DateTime.Parse(date).Day + @"\" + Path.GetFileName(Form1.currentPath);

                if (Directory.Exists(path))
                {
                    Form1.currentPath = path;
                }
                else
                {
                    Form1.currentPath = Path.GetDirectoryName(path);
                    Form1.inPortfolio = false;
                }
            }
            else {
                Form1.currentPath = Form1.basePath + @"\" + DateTime.Parse(date).Year + @"\" + DateTime.Parse(date).Month + @"\" + DateTime.Parse(date).Day;
                Form1.inPortfolio = false;
            }

            form.refreshDataGridView();

            Close();
        }
    }
}
