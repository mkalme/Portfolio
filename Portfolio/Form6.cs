using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Portfolio
{
    public partial class Form6 : Form
    {
        public static String dots = "";
        public static Form1 form;

        public int[] ArrayProgress = { 0, 0 };
        public static bool check = false;

        public String labelText = "";

        public Form6()
        {
            InitializeComponent();
            Shown += new EventHandler(Form6_Shown);
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;

            timer1.Start();
            timer1.Interval = 450;
        }

        private void Form6_Shown(object sender, EventArgs e)
        {
            loading();
        }

        public void loading() {
            Thread t1 = new Thread(delegate ()
            {
                //Check if directory exists, if not than create it
                form.createFolder(Form1.basePath);

                //start date
                form.date();

                //Last been
                form.emptyDays();
                form.lastBeen();
            });
            t1.Start();

            timer2.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String baseText = "Loading";

            if (dots.Length == 0) {
                dots = ".";
            } else if (dots.Length == 1) {
                dots = "..";
            } else if (dots.Length == 2) {
                dots = "...";
            } else{
                dots = "";
            }

            label1.Text = baseText + dots;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            progressBar1.Maximum = ArrayProgress[0];
            progressBar1.Value = ArrayProgress[1];

            label2.Text = labelText;

            if (progressBar1.Value >= progressBar1.Maximum)
            {
                timer1.Stop();
                timer2.Stop();
                Close();
            }
        }
    }
}