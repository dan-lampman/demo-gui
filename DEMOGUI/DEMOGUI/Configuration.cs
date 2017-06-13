using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DEMOGUI
{
    public class Configuration
    {
        XDocument xmlDoc;
        public string License;
        public string LinearInput;

        #region Base Parameters
        public string Parallel { get; set; } //serial, ms, mp, parallel
        public string Debug { get; set; } //on, off
        public string Hboa { get; set; } //on, off
        public int InitPop { get; set; } //have to be a multiple of 4
        public int MinPop { get; set; } //have to be a multiple of 4
        public int MaxPop { get; set; } //have to be a multiple of 4
        public string PopScheme { get; set; } //injection, scaling
        public double PopFactor { get; set; } // 0 < x < 1
        public int MaxGens { get; set; }
        public int NObj { get; set; } //Number of objectives
        public int NCons { get; set; } //Number of conditions
        public int NReal { get; set; } //Number of real variables
        public int NBin { get; set; } //Number of bins
        public int NTagAlong { get; set; }
        #endregion

        #region Objective Parameters
        public List<string> ObjTags { get; set; } //string tag name
        public List<string> ObjVal { get; set; } //min or max
        public List<double> ObjEps { get; set; } //double value < 1
        #endregion

        #region Demo Constraints & Parameters
        public int NData { get; set; } //Time Series Length 
        public int NAObj { get; set; } //Number of additional objectives
        public int NConst { get; set; } //Number of constraints
        public List<int> AObj1 { get; set; } //Additional objectives left side
        public List<int> AObj2 { get; set; } //Additional objectives right side
        public List<int> ConNum { get; set; } //constraints number
        public List<int> ConTot { get; set; } //constraints total
        public List<string> ConInd { get; set; } //constraints indicator; hard, soft

        #endregion

        #region Real Variables
        public List<string> RealTag { get; set; } //Real tag values
        public List<double> RealMin { get; set; } //Real min values
        public List<double> RealMax { get; set; } //Real max values
        public string PseudoBinary { get; set; }
        public string RealLimits { get; set; } //same, different
        public string TestProblem { get; set; }
        public double RealCrossProb { get; set; } // 0.6 < x < 1.0
        public double RealMutProb { get; set; } //1 / nreal
        public int DistIndexSBX { get; set; }
        public int DistIndexPoly { get; set; }

        #endregion

        #region Binary Variables

        public List<string> BinTag { get; set; } //Bin tag values
        public List<int> BinMin { get; set; } //Bin min values
        public List<int> BinMax { get; set; } //Bin max values
        public List<int> BinBit { get; set; } //Bin bit values
        public bool BinLimits { get; set; } //Bin limit flag
        public double BinCrossProb { get; set; }
        public string BinCrossType { get; set; } //one_point, two_point, uniform
        public double BinMutProb { get; set; }
        public string BinMutType { get; set; } //one_bit, independent

        #endregion

        #region Termination Parameters

        public string InterRun { get; set; } //on, off
        public double InterDelta { get; set; }
        public string IntraRun { get; set; }
        public double IntraDelta { get; set; }
        public int MaxNfe { get; set; }
        public double MaxTime { get; set; }
        public double MaxEperf { get; set; }

        #endregion

        #region Performance Metrics

        public string Conv { get; set; } //on, off
        public string Div { get; set; } //on, off
        public string Eperf { get; set; } //on, off
        public string EdomEperf { get; set; } //on, off
        public string Eind { get; set; } //on, off
        public List<int> DivGrid { get; set; }
        public List<double> EperfEps { get; set; }
        public double EindError { get; set; }
        public string MetricRef { get; set; } //reference or target set file name for metric calculations
        #endregion

        #region Local File Input / Output

        public string InputEntropyDir { get; set; }
        public string InputObjDir { get; set; }
        public string OutDir { get; set; } //in->fio.resultsDir
        public string OutHeaders { get; set; } //on, off, in->RS_state
        public int OutInterval { get; set; } //in->fio.all_interval
        public bool OutAll { get; set; } //true if OutInterval is > 0, in->fio.use_all
        public int NondomInterval { get; set; } //in->fio.all_nondom_interval
        public bool Nondom { get; set; } //in->fio.use_all_nondom
        public bool NondomFinal { get; set; } //in->fio.use_final_nondom
        public bool AllFinal { get; set; } //in->fio.use_final
        public bool Stats { get; set; } //in->fio.use_stats
        public int StatsInterval { get; set; } //in->fio.stats_interval
        public bool Timer { get; set; } //in->fio.use_timers
        public int TimerInterval { get; set; } //in->fio.timer_interval
        public bool RsStats { get; set; } //in->fio.use_RS_stats
        public bool UseVtk { get; set; } //in->fio.use_vtk
        public int VtkInterval { get; set; } //in->fio.vtk_interval
        public string SmallFlag { get; set; } //small, large, in->fio.smallFlag

        #endregion

        public void Save(
            string linearInput = "", string inputEntropyDir = "", string inputObjDir = "", string license = "", string parallel = "", string debug = "", string hboa = "", int initPop = 0, int minPop = 0, int maxPop = 0, string popScheme = "", double popFactor = 0.0,
            int maxGens = 0, int nObj = 0, int nCons = 0, int nReal = 0, int nBin = 0, List<string> objTags = default(List<string>), List<string> objVal = default(List<string>), 
            List<double> objEps = default(List<double>), int nData = 0, int nAObj = 0, int nConst = 0, List<int> aObj1 = default(List<int>), List<int> aObj2 = default(List<int>), List<int> conNum = default(List<int>),
            List<int> conTot = default(List<int>), List<string> conInd = default(List<string>), List<string> realTag = default(List<string>), List<double> realMin = default(List<double>),
            List<double> realMax = default(List<double>), string pseudoBinary = "", string realLimits = "", string testProblem = "", double realCrossProb = 0.0, double realMutProb = 0.0, int distIndexSBX = 0,
            int distIndexPoly = 0, List<string> binTag = default(List<string>), List<int> binMin = default(List<int>), List<int> binMax = default(List<int>), List<int> binBit = default(List<int>),
            bool binLimits = false, double binCrossProb = 0.0, string binCrossType = "", double binMutProb = 0.0, string binMutType = "", string interRun = "",
            double interDelta = 0.0, string intraRun = "", double intraDelta = 0.0, int maxNfe = 0, double maxTime = 0.0, double maxEpert = 0.0, string conv = "",
            string div = "", string eperf = "", string edomEperf = "", string eind = "", List<int> divGrid = default(List<int>), List<double> eperfEps = default(List<double>),
            double eindError = 0.0, string metricRef = "", string resultsDir = "", bool rsStats = false, bool useAll = false, double allInterval = 0.0, bool useAllNondom = false, 
            double allNondomInterval = 0.0, bool useFinal = false, bool useFinalNondom = false, bool useStates = false, double statsInterval = 0.0, bool useTimers = false,
            double timersInterval = 0.0, bool useRsStats = false, bool useVtk = false, double vtkInterval = 0.0, bool smallFlag = false)
        {
            if (linearInput != string.Empty)
                LinearInput = linearInput;

            if (license != string.Empty)
                License = license;

            if (inputEntropyDir != string.Empty)
                InputEntropyDir = inputEntropyDir;

            if (inputObjDir != string.Empty)
                InputObjDir = inputObjDir;

            #region Base Parameter Save

            if (parallel != string.Empty)
                Parallel = parallel;

            if (debug != string.Empty)
                Debug = debug;

            if (hboa != string.Empty)
                Hboa = hboa;

            if (initPop != 0)
                InitPop = initPop;

            if (minPop != 0)
                MinPop = minPop;

            if (maxPop != 0)
                MaxPop = maxPop;

            if (popScheme != string.Empty)
                PopScheme = popScheme;

            if (popFactor != 0.0)
                PopFactor = popFactor;

            if (maxGens != 0)
                MaxGens = maxGens;

            if (nObj != 0)
                NObj = nObj;

            if (nCons != 0)
                NCons = nCons;

            if (nReal != 0)
                NReal = nReal;

            if (nBin != 0)
                NBin = nBin;

            #endregion

            #region Objective Parameters Save
            if (objTags != default(List<string>))
                ObjTags = objTags;

            if (objVal != default(List<string>))
                ObjVal = objVal;

            if (objEps != default(List<double>))
                ObjEps = objEps;

            #endregion

            #region Demo Constraints & Parameters Save

            if (nData != 0)
                NData = nData;

            if (nAObj != 0)
                NAObj = nAObj;

            if (nConst != 0)
                NConst = nConst;

            if (aObj1 != default(List<int>))
                AObj1 = aObj1;

            if (aObj2 != default(List<int>))
                AObj2 = aObj2;

            if (conNum != default(List<int>))
                ConNum = conNum;

            if (conTot != default(List<int>))
                ConTot = conTot;

            if (conInd != default(List<string>))
                ConInd = conInd;



            #endregion

            #region Real Variables Save
            if (realTag != default(List<string>))
                RealTag = realTag;

            if (realMin != default(List<double>))
                RealMin = realMin;

            if (realMax != default(List<double>))
                RealMax = realMax;

            if (pseudoBinary != string.Empty)
                PseudoBinary = pseudoBinary;

            if (realLimits != string.Empty)
                RealLimits = realLimits;

            if (realCrossProb != 0.0)
                RealCrossProb = realCrossProb;

            if (realMutProb != 0.0)
                RealMutProb = realMutProb;

            if (distIndexSBX != 0)
                DistIndexSBX = distIndexSBX;

            if (distIndexPoly != 0)
                DistIndexPoly = distIndexPoly;

            if (testProblem != string.Empty)
                TestProblem = testProblem;

            #endregion

            #region Binary Variables Save

            if (binTag != default(List<string>))
                BinTag = binTag;

            if (binMin != default(List<int>))
                BinMin = BinMin;

            if (binMax != default(List<int>))
                BinMax = binMax;

            if (binBit != default(List<int>))
                BinBit = binBit;

            if (binLimits != false)
                BinLimits = binLimits;

            if (binCrossProb != 0.0)
                BinCrossProb = binCrossProb;

            if (binCrossType != string.Empty)
                BinCrossType = binCrossType;

            if (binMutProb != 0.0)
                BinMutProb = binMutProb;

            if (binMutType != string.Empty)
                BinMutType = binMutType;


            #endregion

            #region Termination Parameters Save

            if (interRun != string.Empty)
                InterRun = interRun;

            if (interDelta != 0.0)
                InterDelta = interDelta;

            if (intraRun != string.Empty)
                IntraRun = intraRun;

            if (intraDelta != 0.0)
                IntraDelta = intraDelta;

            if (maxNfe != 0)
                MaxNfe = maxNfe;

            if (maxTime != 0.0)
                MaxTime = maxTime;

            if (maxEpert != 0.0)
                MaxEperf = maxEpert;

            #endregion

            #region Performance Metric Save

            if (conv != string.Empty)
                Conv = conv;

            if (div != string.Empty)
                Div = div;

            if (eperf != string.Empty)
                Eperf = eperf;

            if (edomEperf != string.Empty)
                EdomEperf = edomEperf;

            if (eind != string.Empty)
                Eind = eind;

            if (divGrid != default(List<int>))
                DivGrid = divGrid;

            if (eperfEps != default(List<double>))
                EperfEps = eperfEps;

            if (eindError != 0.0)
                EindError = eindError;

            if (metricRef != string.Empty)
                MetricRef = metricRef;


            #endregion

            #region Local File Output

            //doing this in actual save function instead

            #endregion

        }

        public void SaveToXml(string path = "")
        {
            string xmlInput = "<configuration> \n <parallel> serial </parallel>";

            xmlInput += AddNewPropertyValue("linearInput", LinearInput);
            xmlInput += AddNewPropertyValue("license", License);
            xmlInput += AddNewPropertyValue("entropyDir", InputEntropyDir);
            xmlInput += AddNewPropertyValue("objDir", InputObjDir);

            //population sizing
            xmlInput += AddNewPropertyValue("debug_mode", Debug);
            xmlInput += AddNewPropertyValue("use_hboa", Hboa);
            xmlInput += AddNewPropertyValue("initial_popsize", InitPop.ToString());
            xmlInput += AddNewPropertyValue("min_popsize", MinPop.ToString());
            xmlInput += AddNewPropertyValue("max_popsize", MaxPop.ToString());
            xmlInput += AddNewPropertyValue("popsize_scheme", PopScheme.ToString());
            xmlInput += AddNewPropertyValue("pop_scaling_factor", PopFactor.ToString());
            xmlInput += AddNewPropertyValue("max_gens", MaxGens.ToString());

            //Design Objectives
            xmlInput += AddNewPropertyValue("test_problem", TestProblem);
            xmlInput += AddNewPropertyValue("nobj", NObj.ToString());

            for (int i = 1; i <= NObj; i++)
            {
                xmlInput += AddNewPropertyValue("obj_tag" + i, ObjTags[i-1].ToString());
                xmlInput += AddNewPropertyValue("obj_val" + i, ObjVal[i-1].ToString());
                xmlInput += AddNewPropertyValue("obj_eps" + i, ObjEps[i-1].ToString());
            }

            xmlInput += AddNewPropertyValue("num_additional_objectives", NAObj.ToString());

            for (int i = 1; i <= NAObj; i++)
            {
                xmlInput += AddNewPropertyValue("aobj1_" + i, AObj1[i - 1].ToString());
                xmlInput += AddNewPropertyValue("aobj" + i + "_2", AObj2[i - 1].ToString());
            }

            xmlInput += AddNewPropertyValue("ncons", NConst.ToString());

            for (int i = 1; i <= NConst; i++)
            {
                xmlInput += AddNewPropertyValue("con_num" + i, ConNum[i-1].ToString());
                xmlInput += AddNewPropertyValue("con_tot" + i, ConTot[i-1].ToString());
                xmlInput += AddNewPropertyValue("con_ind" + i, ConInd[i-1].ToString());
            }

            //Real Decision Variables

            xmlInput += AddNewPropertyValue("nreal", NReal.ToString());
            xmlInput += AddNewPropertyValue("real_pseudo_binary", PseudoBinary.ToString());
            xmlInput += AddNewPropertyValue("real_limits", RealLimits.ToString());

            for (int i = 1; i <= RealTag.Count(); i++)
            {
                xmlInput += AddNewPropertyValue("real_tag" + i, RealTag[i-1].ToString());
                xmlInput += AddNewPropertyValue("real_min" + i, RealMin[i-1].ToString());
                xmlInput += AddNewPropertyValue("real_max" + i, RealMax[i-1].ToString());
            }            
            
            xmlInput += AddNewPropertyValue("real_cross_prob", RealCrossProb.ToString());
            xmlInput += AddNewPropertyValue("real_mut_prob", RealMutProb.ToString());
            xmlInput += AddNewPropertyValue("dist_index_sbx", DistIndexSBX.ToString());
            xmlInput += AddNewPropertyValue("dist_index_poly", DistIndexPoly.ToString());

            //Binary Decision Variables

            xmlInput += AddNewPropertyValue("nbin", NBin.ToString());
            xmlInput += AddNewPropertyValue("bin_limits", BinLimits == true ? " same " : " different ");

            for (int i = 1; i <= NBin; i++)
            {
                xmlInput += AddNewPropertyValue("bin_tag" + i, BinTag[i-1].ToString());
                xmlInput += AddNewPropertyValue("bin_bit" + i, BinBit[i - 1].ToString());
                xmlInput += AddNewPropertyValue("bin_min" + i, RealMin[i-1].ToString());
                xmlInput += AddNewPropertyValue("bin_max" + i, RealMax[i-1].ToString());
            }

            xmlInput += AddNewPropertyValue("bin_cross_prob", BinCrossProb.ToString());
            xmlInput += AddNewPropertyValue("bin_cross_type", BinCrossType.ToString());
            xmlInput += AddNewPropertyValue("bin_mut_prob", BinMutProb.ToString());
            xmlInput += AddNewPropertyValue("bin_mut_type", BinMutType.ToString());

            //Soft Termination Criteria

            xmlInput += AddNewPropertyValue("inter_run", InterRun.ToString());
            xmlInput += AddNewPropertyValue("inter_delta", InterDelta.ToString());
            xmlInput += AddNewPropertyValue("intra_run", IntraRun.ToString());
            xmlInput += AddNewPropertyValue("inter_delta", IntraDelta.ToString());

            //Hard Termination Criteria

            xmlInput += AddNewPropertyValue("max_nfe", MaxNfe.ToString());
            xmlInput += AddNewPropertyValue("max_time", MaxTime.ToString());
            xmlInput += AddNewPropertyValue("max_eperf", MaxEperf.ToString());

            //Performance Metrics

            xmlInput += AddNewPropertyValue("conv", Conv.ToString());
            xmlInput += AddNewPropertyValue("div", Div.ToString());
            xmlInput += AddNewPropertyValue("eperf", Eperf.ToString());
            xmlInput += AddNewPropertyValue("edom_eperf", EdomEperf.ToString());

            for (int i = 1; i <= DivGrid.Count; i++)
            {
                xmlInput += AddNewPropertyValue("div_grid" + i, DivGrid[i-1].ToString());
            }

            for (int i = 1; i <= EperfEps.Count; i++)
            {
                xmlInput += AddNewPropertyValue("eperf_eps" + i, EperfEps[i-1].ToString());
            }

            xmlInput += AddNewPropertyValue("eind", Eind.ToString());
            xmlInput += AddNewPropertyValue("eind_error", EindError.ToString());
            xmlInput += AddNewPropertyValue("metric_ref", MetricRef.ToString());

            //Local File Output

            xmlInput += AddNewPropertyValue("out_dir", OutDir.ToString());
            xmlInput += AddNewPropertyValue("out_headers", OutHeaders.ToString());
            xmlInput += AddNewPropertyValue("out_all", OutAll ? OutInterval.ToString() : "0");
            xmlInput += AddNewPropertyValue("out_nondom", Nondom ? NondomInterval.ToString() : "0");
            xmlInput += AddNewPropertyValue("out_all_final", AllFinal ? "on" : "off");
            xmlInput += AddNewPropertyValue("out_nondom_final", NondomFinal ? "on" : "off");
            xmlInput += AddNewPropertyValue("out_stats", StatsInterval.ToString());
            xmlInput += AddNewPropertyValue("out_rs", RsStats ? "on" : "off");
            xmlInput += AddNewPropertyValue("out_timer", TimerInterval.ToString());
            xmlInput += AddNewPropertyValue("out_vtk", VtkInterval.ToString());
            xmlInput += AddNewPropertyValue("out_vtk_sflag", SmallFlag.ToString());


            //DEMO Parameters

            xmlInput += AddNewPropertyValue("ndata", NData.ToString());

            

            //End Tag
            xmlInput += "\n </configuration>";

            var doc = XDocument.Parse(xmlInput);
            xmlDoc = doc;

            if (path == string.Empty)
                doc.Save(path + "configuration.xml");
            else
                doc.Save(path);
        }

        public string AddNewPropertyValue(string prop, string val)
        {
            if (val == null)
                return string.Empty;

            if (val.Contains(" ") == false)
                return "\n" + "<" + prop + "> " + val + " </" + prop + " >\n";

            return "\n" + "<" + prop + ">" + val + "</" + prop + " >\n";

        }
    }

   

}
