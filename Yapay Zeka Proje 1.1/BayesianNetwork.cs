using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yapay_Zeka_Proje_1._1
{
    class BayesianNetwork
    {
        public static Field[] fields;
        public static string[][] train;
        public static string[][] test;
        public static CPT[] allCPT;
        private static int classIndex;

        /*
            CPT Structure
            -------------
            Dictionary<string[X], Dictionary<string[Y], double[Z]>>
            X -> values of parent nodes in current record, combined with '|' symbol(example: rent|bad| for personal_status)
            Y -> value of current node in current record (example: male div/sep or male single for personal_status)
            Z -> probability of that combination

            for root node only
            X -> *
             */

        //create all cpt tables
        public static void CreateAllCPT()
        {
            int i, j, k;
            int fieldCount = fields.Length;
            allCPT = new CPT[fieldCount];
            Dictionary<string, int> counts;
            string temp;
            for (i = 0; i < fieldCount; i++)
            {
                allCPT[i] = new CPT(fields[i]);
                counts = new Dictionary<string, int>();
                if (fields[i].parents[0] == -1)//for root node
                {
                    classIndex = i;
                    temp = "*";
                    for (j = 0; j < train.Length; j++)
                    {
                        if (!allCPT[i].map.ContainsKey(temp))
                        {
                            allCPT[i].map[temp] = new Dictionary<string, double>();
                            allCPT[i].map[temp][train[j][i]] = 1;
                            counts[temp] = 1;
                        }
                        else
                        {
                            if (!allCPT[i].map[temp].ContainsKey(train[j][i]))
                                allCPT[i].map[temp][train[j][i]] = 1;
                            else
                                allCPT[i].map[temp][train[j][i]]++;
                            counts[temp]++;
                        }
                    }
                }
                else
                {
                    for (j = 0; j < train.Length; j++)
                    {
                        temp = "";
                        for (k = 0; k < fields[i].parents.Count; k++)
                        {
                            temp += train[j][fields[i].parents[k]] + "|";
                        }
                        if (!allCPT[i].map.ContainsKey(temp))
                        {
                            allCPT[i].map[temp] = new Dictionary<string, double>();
                            allCPT[i].map[temp][train[j][i]] = 1;
                            counts[temp] = 1;
                        }
                        else
                        {
                            if (!allCPT[i].map[temp].ContainsKey(train[j][i]))
                                allCPT[i].map[temp][train[j][i]] = 1;
                            else
                                allCPT[i].map[temp][train[j][i]]++;
                            counts[temp]++;
                        }
                    }
                }
                foreach (KeyValuePair<string, int> kVP in counts)
                {
                    for (j = 0; j < allCPT[i].map[kVP.Key].Count; j++)
                    {
                        KeyValuePair<string, double> tempKey;
                        tempKey = allCPT[i].map[kVP.Key].ElementAt(j);
                        allCPT[i].map[kVP.Key][tempKey.Key] /= kVP.Value;
                    }
                }
            }
        }
        //Big Float https://github.com/Osinko/BigFloat
        //calculate metrics by creating confusion matrix using bayesian formula
        public static (int,int,double,double,double) CalculateMetrics(string[][] data)
        {
            int i, j, k;
            string temp, newClass = "";
            BigFloat prob, denom, maxProb, val;
            int[,] confusionMatrix = new int[2, 2];
            Dictionary<string, BigFloat> probs = new Dictionary<string, BigFloat>();
            for (i = 0; i < data.Length; i++)
            {
                denom = 0; maxProb = 0;
                foreach (KeyValuePair<string, double> kVP in allCPT[classIndex].map["*"])
                {
                    prob = 1;
                    for (j = 0; j < fields.Length; j++)
                    {
                        if(j!=classIndex)
                        {
                            temp = "";
                            for (k = 0; k < fields[j].parents.Count; k++)
                            {
                                if (fields[j].parents[k] == classIndex)
                                    temp += kVP.Key + "|";
                                else
                                    temp += data[i][fields[j].parents[k]] + "|";
                            }
                            if(allCPT[j].map[temp].ContainsKey(data[i][j]))
                                prob *= (decimal)allCPT[j].map[temp][data[i][j]];
                        }
                    }
                    prob *= (decimal)kVP.Value;
                    probs[kVP.Key] = prob;
                    denom += prob;
                }
                foreach (KeyValuePair<string, BigFloat> kVP in probs)
                {
                    val = kVP.Value / denom;
                    if (val >= maxProb)
                    {
                        maxProb = val;
                        newClass = kVP.Key;
                    }
                }
                //can be changed in the future for more dynamic version like parent info
                if (newClass == "good")
                {
                    if (data[i][classIndex] == "good")
                        confusionMatrix[1,1]++;
                    else
                        confusionMatrix[1, 0]++;
                }
                else
                {
                    if (data[i][classIndex] == "bad")
                        confusionMatrix[0, 0]++;
                    else
                        confusionMatrix[0, 1]++;
                }   
            }
            int tpCount = confusionMatrix[0, 0], tnCount = confusionMatrix[1, 1];
            double tpRate = confusionMatrix[0, 0] * 1.0 / (confusionMatrix[0, 0] + confusionMatrix[1, 0]),
                tnRate= confusionMatrix[1, 1] * 1.0 / (confusionMatrix[0, 1] + confusionMatrix[1, 1]),
                acc= (confusionMatrix[1, 1] + confusionMatrix[0, 0]) * 1.0 / data.Length ;
            return (tpCount, tnCount, tpRate, tnRate, acc);
        }
    }
}
