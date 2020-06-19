using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Yapay_Zeka_Proje_1._1
{
    class FileOperations
    {
        //read all data, create data matrix and fields
        public static (string[][], Field[],string) Read()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string[] input, temp;
            int fieldCount, i, recordCount, j;
            Field[] fields = null;
            string[][] records = null;
            try
            {
                if (ofd.CheckFileExists && ofd.FileName != "")
                {
                    StreamReader sr = new StreamReader(ofd.FileName);
                    input = sr.ReadToEnd().Split('\n');
                    temp = input[0].Split(',');
                    fieldCount = temp.Length;
                    fields = new Field[fieldCount];
                    recordCount = input.Length - 1;
                    records = new string[recordCount][];
                    for (i = 0; i < fieldCount; i++)
                    {
                        temp[i] = temp[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                        fields[i] = new Field(temp[i]);
                    }
                    for (i = 1; i < recordCount+1; i++)
                    {
                        temp = input[i].Split(',');
                        records[i - 1] = new string[fieldCount];
                        for (j = 0; j < fieldCount; j++)
                        {
                            temp[j] = temp[j].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                            records[i - 1][j] = temp[j];
                        }
                    }
                }
                else
                    throw new FileNotFoundException("Please choose file");
            }
            catch (FileNotFoundException err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return (records, fields,ofd.FileName);
        }
        /*
            Parent Info File
            ----------------
            X Y,Z,.....
            X -> current field index (in train/test file by starting 0)
            Y,Z -> indexes of parents (in train/test file by starting 0)

            example for credit_history
            --------------------------
            0 7,8
            0 -> credit_history
            7 -> own_telephone
            8 -> class
             */

        //read parent info (tree structure)
        public static string ReadParentInfo()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string[] input,temp;
            int i,no,pNo,j;
            try
            {
                if (ofd.CheckFileExists && ofd.FileName != "")
                {
                    StreamReader sr = new StreamReader(ofd.FileName);
                    input = sr.ReadToEnd().Split('\n');
                    for(i=0;i<input.Length;i++)
                    {
                        temp = input[i].Split(' ');
                        no=int.Parse(temp[0]);
                        temp = temp[1].Split(',');
                        for(j=0;j<temp.Length;j++)
                        {
                            pNo = int.Parse(temp[j]);
                            BayesianNetwork.fields[no].parents.Add(pNo);
                        }
                    }
                }
                else
                    throw new FileNotFoundException("Please choose file");
            }
            catch (FileNotFoundException err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ofd.FileName;
        }
        //write all cpt tables to csv files
        public static void WriteAllCPT(CPT[] allCPT)
        {
            int i;
            string fileName, cols,row;
            for (i = 0; i < allCPT.Length; i++)
            {
                fileName = Directory.GetCurrentDirectory() + "\\cpt_" + allCPT[i].field.name + ".csv";
                StreamWriter sw = new StreamWriter(fileName);
                cols = "****,";
                Dictionary<string, bool> test = new Dictionary<string, bool>();
                foreach (KeyValuePair<string, Dictionary<string, double>> dict in allCPT[i].map)
                {
                    foreach (KeyValuePair<string, double> kVP in dict.Value)
                    {
                        if (!test.ContainsKey(kVP.Key))
                        {
                            test[kVP.Key] = true;
                            cols += kVP.Key + ",";
                        }
                    }
                }
                sw.WriteLine(cols);
                sw.Flush();
                foreach (KeyValuePair<string, Dictionary<string, double>> dict in allCPT[i].map)
                {
                    row = dict.Key+",";
                    Dictionary<string,double> temp=dict.Value;
                    foreach (KeyValuePair<string, bool> kVP in test)
                    {
                        if (!temp.ContainsKey(kVP.Key))
                        {
                            row += 0+",";
                        }
                        else
                        {
                            row += temp[kVP.Key] + ",";
                        }
                    }
                    sw.WriteLine(row);
                    sw.Flush();
                }
            }
        }
        //write all metrics to csv file
        public static void WriteMetrics(int tpCount,int tnCount,double tpRate,double tnRate,double acc)
        {
            string fileName = Directory.GetCurrentDirectory() + "\\metrics.csv";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("True Positive Count; " + tpCount);
            sw.WriteLine("True Negative Count; " + tnCount);
            sw.WriteLine("True Positive Rate; " + tpRate);
            sw.WriteLine("True Negative Rate; " + tnRate);
            sw.WriteLine("Accuracy; " + acc);
            sw.Flush();
        }
    }
}
