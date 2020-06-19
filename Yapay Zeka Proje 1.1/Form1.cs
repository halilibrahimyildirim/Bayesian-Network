using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yapay_Zeka_Proje_1._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //choose train file button
        private void button1_Click(object sender, EventArgs e)
        {
            string fileName;
            label1.Text = label3.Text = label2.Text = "File Name: ";
            button2.Enabled = button3.Enabled = button4.Enabled = false;
            (BayesianNetwork.train, BayesianNetwork.fields, fileName) = FileOperations.Read();
            string[] parsed = fileName.Split('\\');
            label1.Text = "File Name: " + parsed[parsed.Length - 1];
            if (fileName != "")
                button2.Enabled = true;
        }

        //choose test file button
        private void button2_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            string fileName;
            (BayesianNetwork.test, BayesianNetwork.fields, fileName) = FileOperations.Read();
            string[] parsed = fileName.Split('\\');
            label2.Text = "File Name: " + parsed[parsed.Length - 1];
            if (fileName != "")
                button3.Enabled = true;
        }

        //choose parent info file button
        private void button3_Click(object sender, EventArgs e)
        {
            string fileName;
            fileName = FileOperations.ReadParentInfo();
            string[] parsed = fileName.Split('\\');
            label3.Text = "File Name: " + parsed[parsed.Length - 1];
            if (fileName != "")
                button4.Enabled = true;
        }

        //start button
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            int tpCount, tnCount;
            double tpRate, tnRate, acc;
            BayesianNetwork.CreateAllCPT();
            //BayesianNetwork.CalculateMetrics(BayesianNetwork.train); //for training metrics to check
            (tpCount, tnCount, tpRate, tnRate, acc) = BayesianNetwork.CalculateMetrics(BayesianNetwork.test);
            FileOperations.WriteAllCPT(BayesianNetwork.allCPT);
            FileOperations.WriteMetrics(tpCount, tnCount, tpRate, tnRate, acc);
            listBox1.Items.Add("Results");
            listBox1.Items.Add("True Positive Count; " + tpCount);
            listBox1.Items.Add("True Negative Count; " + tnCount);
            listBox1.Items.Add("True Positive Rate; " + tpRate);
            listBox1.Items.Add("True Negative Rate; " + tnRate);
            listBox1.Items.Add("Accuracy; " + acc);
            MessageBox.Show("Results created in debug directory.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //for using dot instead of comma
            //https://stackoverflow.com/questions/9160059/set-up-dot-instead-of-comma-in-numeric-values
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }
    }
}
