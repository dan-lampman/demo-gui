using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEMOGUI
{
    public static class DemoUtil
    {
        public static int[] LinearRank(double[,] x, int ndata, int nstat)
        {
            //double[,] x = inputX; //This should come from function call or another read function //actual data values
            double[,] y = new double[ndata, nstat]; //Allocate simulated values for ndata (rows) and NStations(Col) //simulated values
            double[,] a = new double[ndata, nstat-1]; // A is the MLR predictor 
            var b = new double[nstat-1]; // MLR Beta coeff
            var hX = new double[nstat]; // Entropy of X[i]
            var hY = new double[nstat]; // Entropy of Y[i]
            var jH = new double[nstat]; // JOINT ENTROPY of (X[i],Y[i])
            var tI = new double[nstat]; // Transinformation of (X[i],Y[i])
            var jqobs2 = new double[ndata]; // Vector of the same as before...Here we assume no overrun condition unlike DEMO 
            var ranks = new int[nstat];

            // Here we iterate for each station. Use Multiple-Linear Regregression to
            // try and predict the values at Stn i, called Y, given information about the other
            // stations. The mutual information (total correlation) between X[i] and Y[i]
            // will determine how well known Y is given the information about the
            // other stations.
            for (int i = 0; i < nstat; i++)
            {
	            //Copy all data except station i to become the predictor; Not sure if this syntax actually works
	            for (int n = 0; n < nstat; n++)
	            {
		            var a_count = 0;

		            if(n != i)
		            {
                        var colValues = getColumnValues(x, i, nstat);
                        var aValues = new List<double>();

                        for (var z = 0; z < nstat; z++)
                        {
                            aValues.Add(colValues[z]);
                        }

                        for (var z = 0; z < aValues.Count(); z++)
                        {
                            a[n, a_count] = aValues.ToArray()[z];
                        }


                        ////set column values
                        //a[:,a_count] = x[:,i];
                        //a_count++;
		            }
                    
                    //set new a values here

                    


	            }
	            // Now do the MLR. Depending on the library we might need to train it first to get the b parameters 
	            //B[:] = MLR.TRAIN((double)A, X[:,i]) // This will depend on MLR implementation Y' = a1*b1+a2*b2+...an*bn
		            //Y[:,i] = MLR.RUN((double)A); 

                //iterate through each column of Y and set new column values here using LinearRegression()
                //assign back to Y

            } // End for each station loop

            /// Find out the mutual information(transinformation/total correlation)
            // between Y and Yprime. Note the terms above are used synonymously.
            // I believe this is strictly to confuse people.
    
            // The forumal used is TI = H(X)+H(Y) - H(X,Y);

            double[] hJ = new double[nstat];

            for(int i = 0; i < nstat; i++)
            {
                var xValues = getColumnValues(x, i, nstat);
                var yValues = getRowValues(y, i, ndata);
	            hX[i] = Entropy(xValues.ToArray(),ndata); // Find the single Entropy of X and Y
	            hY[i] = Entropy(yValues.ToArray(),ndata);
	
	            // Get JQOBS2 and then get its entropy which is joint entropy
	            for(int n=0;n<ndata;n++)
	            {		            
                    jqobs2[n] = x[n,i]*FindMaxYValue(y, i, nstat) + y[n,i]; // Note in DEMO this is harder because we use multiple stations
	            }

	            hJ[i] = Entropy(jqobs2, ndata); // Joint Entropy
	            tI[i] = hX[i] + hY[i] - hJ[i]; // Transinformation 

            }

            Array.Sort(ranks);

            return ranks;

        }

        public static double FindMaxYValue(double[,] y, int n, int nstat)
        {
            double max = 0;

            for (var i = 0; i < nstat; i++)
            {
                if (max < y[i, nstat - 1 < 0 ? 0 : nstat - 1])
                    max = y[i, nstat - 1 < 0 ? 0 : nstat - 1];
            }

            return max;
        }

        public static double Entropy(double[] x, int ndata)
        {
            // Declarations
            int nd, i, j, count; // Counters
            double sum; // Total entropy
            double freq; // Frequency of occurence
            bool[] unique_bool = new bool[ndata]; // Tracker for unique values;


            for (nd = 0; nd < ndata; nd++)
            {
                unique_bool[nd] = true;
            }

            count = 0;
            sum = 0.0;

            for (nd = 0; nd < ndata; nd++)
            {
                if (unique_bool[nd])  // If the value at nd is unique find how often they occur
                {
                    count = 0;
                    for (i = 0; i < x.Count(); i++) // Loop through the data
                    {
                        if (unique_bool[i]) // If the value has not been considered
                        {
                            if (x[i] == x[nd]) // If the current value equals the reference
                            {
                                count++;
                                unique_bool[i] = false; // Set to 0 so it won't be looked at again
                            }
                        }
                    }
                    freq = (double)count / (double)ndata;
                    sum = sum - freq * Math.Log(freq) / Math.Log(2);
                }
            }

            return sum;
        }


        public static void LinearRegression(double[] xVals, double[] yVals,
                                        int inclusiveStart, int exclusiveEnd,
                                        out double rsquared, out double yintercept,
                                        out double slope)
        {
            //Debug.Assert(xVals.Length == yVals.Length);
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        public static double[] getColumnValues(double[,] input, int colIndex, int size)
        {
            var output = new List<Double>();

            for (int i = 0; i < size; i++)
                output.Add(input[colIndex, i]);

            return output.ToArray();        
        }

        public static double[] getRowValues(double[,] input, int rowIndex, int size)
        {
            var output = new List<Double>();

            for (int i = 0; i < size; i++)
                output.Add(input[i, rowIndex]);

            return output.ToArray();
        }
        
        public static void ImportDataFromCSV(string argc, List<List<double>> coords, List<List<int>> data, ref int numSelectedPoints, int startIndex = -1, int endIndex = -1)
        {
            if (!File.Exists(argc))
            {
                MessageBox.Show(string.Format("Error: could not find input file {0}", argc), "Input Error", MessageBoxButtons.OK);
                return;
            }
             
            TextFieldParser parser = new TextFieldParser(@argc);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            var j = 0;

            while (!parser.EndOfData)
            {
                var strArray = parser.ReadLine().Split(',').ToList();
                
                if (j < 2)
                {
                    var newRow = strArray.Where(x => x != string.Empty)
                        .Select(x => double.Parse(x)).ToList();      
                   
                    coords.Add(newRow);

                    numSelectedPoints = newRow.Count();
                }
                else
                {
                    var newRow = strArray.Where(x => x != string.Empty)
                         .Select(x => int.Parse(x)).ToList();

                    data.Add(newRow);
                }

                j++;
     
            }
            parser.Close(); 
        }

        public static void ImportDataFromCS(string argc, List<List<double>> data, int numSelectedPoints)
        {
            if (!File.Exists(argc))
            {
                MessageBox.Show(string.Format("Error: could not find input file {0}", argc), "Input Error", MessageBoxButtons.OK);
                return;
            }

            TextFieldParser parser = new TextFieldParser(@argc);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");
            var i = 1;

            while (!parser.EndOfData)
            {
                var strArray = parser.ReadLine().Split('\t').ToList();

                if (i < 9)
                {
                    i++;
                    continue;
                }

                var newRow = strArray.Where(x => x != string.Empty)
                        .Select(x => double.Parse(x)).ToList();

                var parsedRow = new List<double>();

                for (var j = 12; j < 12 + numSelectedPoints; j++)
                {
                    parsedRow.Add(newRow[j]);
                }

                data.Add(parsedRow);

                i++;

            }
            parser.Close();
        }

        public static void ImportGraphDataFromCS(string argc, List<double> a, List<double> b, List<double> c)
        {
            if (!File.Exists(argc))
            {
                MessageBox.Show(string.Format("Error: could not find input file {0}", argc), "Input Error", MessageBoxButtons.OK);
                return;
            }

            TextFieldParser parser = new TextFieldParser(@argc);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");
            var i = 1;

            while (!parser.EndOfData)
            {
                var strArray = parser.ReadLine().Split('\t').ToList();

                if (i < 9)
                {
                    i++;
                    continue;
                }

                var newRow = strArray.Where(x => x != string.Empty)
                        .Select(x => double.Parse(x)).ToList();


                a.Add(newRow[5]);
                b.Add(newRow[6]);
                c.Add(newRow[7]);   
                i++;

            }
            parser.Close();
        }
        
    }


}
