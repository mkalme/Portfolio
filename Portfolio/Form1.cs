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
using System.Globalization;
using System.Threading;

namespace Portfolio
{
    public partial class Form1 : Form
    {
        public static String basePath = @"C:\Users\Michael\AppData\Roaming\My Stock Portfolio\Storage";
        public static String currentPath = basePath;

        public static bool inPortfolio = false;

        public static String timeNow = "";

        public static Form6 formDialog = new Form6();

        public Form1()
        {
            splashScreen();

            InitializeComponent();
        }

        public void splashScreen() {
            Form6.form = this;
            formDialog.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //REFRESH DATAGRIDVIEW
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            refreshDataGridView();

            //Start timer
            timer1.Start();
        }

        public void emptyDays() {
            String date = readFile(Path.GetDirectoryName(basePath) + @"\lastbeen");
            double time = (DateTime.Parse(date) - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))).TotalDays;

            time = (time < 0 ? time / -1 : time);

            //ADD PROGRESS TO ARRAY - MAXIMUM AMOUNT
            formDialog.ArrayProgress[0] = Convert.ToInt32(time);

            for (int i = 1; i <= time; i++){
                DateTime newDate = DateTime.Parse(date).AddDays(i);
                copyPort(newDate, basePath + @"\" + DateTime.Parse(date).AddDays(i - 1).Year + @"\" + DateTime.Parse(date).AddDays(i - 1).Month + @"\" + DateTime.Parse(date).AddDays(i - 1).Day);
                formDialog.ArrayProgress[1] = i;


                formDialog.labelText = "Copying data: " + currentDate(currentPath);
            }

            currentPath = basePath + @"\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\" + DateTime.Now.Day;
        }

        public void copyPort(DateTime date, String sourcePath) {
            currentPath = basePath + @"\" + date.Year + @"\" + date.Month + @"\" + date.Day;

            copyDirectory(sourcePath, currentPath);
            exclude(currentPath);
        }

        public void exclude(String SourcePath) {
            String[] directories = Directory.GetDirectories(SourcePath);

            for (int i = 0; i < directories.Length; i++) {
                String[] directoriesLevel = Directory.GetDirectories(directories[i]);
                for (int b = 0; b < directoriesLevel.Length; b++) {
                    checkIf(directoriesLevel[b]);
                    if (File.Exists(directoriesLevel[b] + @"\actions")) {
                        File.Delete(directoriesLevel[b] + @"\actions");
                        createFile(directoriesLevel[b] + @"\actions", "", true);
                    }
                }
            }
        }

        public void checkIf(String path) {
            string[] lines = readFile(path + @"\information").Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

            if (Int32.Parse(lines[2]) <= 0)
            {
                Directory.Delete(path, true);
            }
        }

        public void copyDirectory(String SourcePath, String DestinationPath) {
            createFolder(DestinationPath);

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public void refreshDataGridView() {
            dataGridView1.Rows.Clear();

            String[] directories = Directory.GetDirectories(currentPath);

            for (int i = 0; i < directories.Length; i++) {
                if (!inPortfolio)
                {
                    String[] directoriesAmount = Directory.GetDirectories(directories[i]);
                    int amount = directoriesAmount.Length;
                    dataGridView1.Rows.Add(Properties.Resources.portfolioIcon, Path.GetFileName(directories[i]), "", amount.ToString(), "", Directory.GetCreationTimeUtc(directories[i]).ToString("yyyy-MM-dd hh:mm:ss"), "");
                }
                else {
                    string[] lines = readFile(directories[i] + @"\information").Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

                    dataGridView1.Rows.Add(Properties.Resources.stockIcon, Path.GetFileName(directories[i]), lines[0], lines[2], readFile(directories[i] + @"\actions"), lines[1], "");

                    if (!string.IsNullOrEmpty(readFile(directories[i] + @"\actions").Trim()))
                    {
                        dataGridView1.Rows[i].Cells[6].Style.BackColor = Color.Orange;
                    }
                    else {
                        dataGridView1.Rows[i].Cells[6].Style.BackColor = Color.White;
                    }
                }
            }

            if (!inPortfolio)
            {
                label1.Text = currentDate(currentPath);
            }
            else {
                label1.Text = currentDate(Path.GetDirectoryName(currentPath));
            }
        }

        public String currentDate(String path) {
            String year = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path)));
            String month = Path.GetFileName(Path.GetDirectoryName(path));
            String day = Path.GetFileName(path);

            DateTime dt = DateTime.Parse(year + "-" + month + "-" + day);
            timeNow = year + "-" + month + "-" + day;

            return dt.ToString("MMMM", new CultureInfo("en")) + " " + Ordinal(Int32.Parse(day)) + ", " + year + "."; 
        }

        public string Ordinal(int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }

        public void lastBeen() {
            createFile(Path.GetDirectoryName(basePath) + @"\lastbeen", DateTime.Now.ToString("yyyy-MM-dd"), false);
        }

        public void createFolder(String path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        public void createFile(String path, String text, bool write)
        {
            using (var tw = new StreamWriter(path, write))
            {
                tw.WriteLine(text);
            }
        }

        public String readFile(String path) {
            String text = "";

            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    text += s + "\r\n";
                }
            }

            return text;
        }

        public void deleteFolder(String path) {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void date() {
            if (!File.Exists(Path.GetDirectoryName(basePath) + @"\lastbeen")) {
                lastBeen();
            }

            String[] directories = Directory.GetDirectories(currentPath);
            if (directories.Length == 0) {
                currentPath = basePath + @"\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\" + DateTime.Now.Day;
                createFolder(currentPath);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Check portfolio
            stockButton.Enabled = inPortfolio;
            portfolioButton.Enabled = !inPortfolio;

            //Check delete
            int directoryCount = Directory.GetDirectories(currentPath).Length;
            if (directoryCount == 0)
            {
                deleteButton.Enabled = false;
            }
            else {
                deleteButton.Enabled = true;
            }

            //check back
            backButton.Enabled = inPortfolio;

            //check action
            int directoryCount1 = Directory.GetDirectories(currentPath).Length;
            if (directoryCount1 > 0 && inPortfolio)
            {
                actionButton.Enabled = true;
            }
            else
            {
                actionButton.Enabled = false;
            }
        }

        private void portfolioButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            Form2.form = this;
            form2.ShowDialog();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this?\t\t\t\t\t\t", "Delete", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                deleteFolder(currentPath + @"\" + dataGridView1.SelectedCells[1].Value.ToString());
                refreshDataGridView();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!inPortfolio) {
                currentPath += @"\" + dataGridView1.SelectedCells[1].Value.ToString();
                inPortfolio = true;
                refreshDataGridView();
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            currentPath = Path.GetDirectoryName(currentPath);
            inPortfolio = false;
            refreshDataGridView();
        }

        private void stockButton_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            Form3.form = this;
            form3.ShowDialog();
        }

        private void actionButton_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            Form4.form = this;
            form4.ShowDialog();
        }

        private void dateButton_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            Form5.form = this;
            form5.ShowDialog();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(currentPath);
        }
    }
}
