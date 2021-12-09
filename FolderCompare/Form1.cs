using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderCompare
{
    public partial class Form1 : Form
    {
        private BindingSource bindingSource1 = new BindingSource();
        private List<FileCompareResult> FileCompareResults = new List<FileCompareResult>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog2.SelectedPath;               
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
           
            FileCompareResults.Clear();
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please Select Source Folder");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Please Select Destination Folder");
                return;
            }

            CompareFiles(textBox1.Text, textBox2.Text);
            CompareDirectories(textBox1.Text, textBox2.Text);
         
            dataGridView1.DataSource = FileCompareResults;    
            if(FileCompareResults.Count > 0)
            {
                dataGridView1.Columns[0].Width = 500;
                dataGridView1.Columns[1].Width = 100;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.HeaderCell.Value = (row.Index + 1).ToString();
                }
            }
        }

        private void CompareFiles(string srcPath,string destPath)
        {
            string[] fileEntriesSrc = new string[] { };
            string[] fileEntriesdest = new string[] { };

            if (!string.IsNullOrEmpty(srcPath))
            {
                fileEntriesSrc = Directory.GetFiles(srcPath);
            }
            if (!string.IsNullOrEmpty(destPath))
            {
                fileEntriesdest = Directory.GetFiles(destPath);
            }
            foreach (string filename in fileEntriesSrc)
            {
                string root = Path.GetFileName(filename);

                if (root != null)
                {
                    if(!FileCompareResults.Any(l => l.FullName == filename.Replace(textBox1.Text + "\\", "")))
                    FileCompareResults.Add(new FileCompareResult { FullName = filename.Replace(textBox1.Text + "\\",""), Status = "Left" });
                }
            }

            foreach (string filename in fileEntriesdest)
            {
                string root = Path.GetFileName(filename);

                if (root != null)
                {
                    var record = FileCompareResults.Where(l => l.FullName == filename.Replace(textBox2.Text + "\\", "")).FirstOrDefault();
                    if (record != null && !string.IsNullOrEmpty(record.FullName))
                    {
                        record.Status = "Both";
                    }
                    else
                    {
                        if (!FileCompareResults.Any(l => l.FullName == filename.Replace(textBox2.Text + "\\", "")))
                            FileCompareResults.Add(new FileCompareResult { FullName = filename.Replace(textBox2.Text + "\\", ""), Status = "Right" });
                    }
                }
            }
        }

        private void CompareDirectories(string srcPath, string destPath)
        {
            string[] srcdirs = Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories);
            string[] destcdirs = Directory.GetDirectories(destPath, "*", SearchOption.AllDirectories);

            foreach (string dirname in srcdirs)
            {
                string root = Path.GetFileName(dirname);

                if (root != null)
                {
                    FileCompareResults.Add(new FileCompareResult { FullName = root, Status = "Left" });
                    CompareFiles(dirname, "");
                }
            }

            foreach (string dirname in destcdirs)
            {
                string root = Path.GetFileName(dirname);

                if (root != null)
                {
                    var record = FileCompareResults.Where(l => l.FullName == root).FirstOrDefault();
                    if (record != null && !string.IsNullOrEmpty(record.FullName))
                    {
                        record.Status = "Both";
                        CompareFiles(srcPath +"\\" + root, dirname);
                    }
                    else
                    {
                        FileCompareResults.Add(new FileCompareResult { FullName = root, Status = "Right" });
                        CompareFiles("",dirname);
                    }
                }
            }
        }

        public class FileCompareResult
        {
            public string FullName { get; set; }
            public string Status { get; set; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
