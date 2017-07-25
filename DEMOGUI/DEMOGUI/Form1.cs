using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.Internals;
using GMap.NET.ObjectModel;
using GMap.NET.Projections;
using GMap.NET.Properties;
using GMap.NET.WindowsForms;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DEMOGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool running;
        public Configuration config;
        public List<List<double>> coords;
        public List<List<double>> linearCoords;
        public List<List<int>> data;
        public List<List<int>> linearData;
        public PointLatLng map1Center;
        public int numSelectedPoints;
        public string statsPath;
        public string outPath;
        public bool existingConfig;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!hasWriteAccessToFolder(Directory.GetCurrentDirectory()))
            {
                MessageBox.Show("The current folder is write-protected. Please run this program as Administrator or change the install path of DEMO.", "Permission Denied");
            }


            this.Icon = new Icon("Resources/globe_icon.ico");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = true;
            this.MaximizeBox = false;
            EnableTab(tabPageBinary, false);
            EnableTab(tagPagePerformance, false);
            tabControl1.Width = this.Width;
            tabControl1.Height = this.Height;
            btnStart.Enabled = false;
            config = new Configuration();
            config.Save();
            running = false;          
         
            demoWorker.DoWork += demoWorker_DoWork;
            demoWorker.RunWorkerCompleted += demoWorker_RunWorkerCompleted;
            demoWorker.ProgressChanged += demoWorker_ProgressedChanged;
            demoWorker.WorkerReportsProgress = true;
            demoWorker.WorkerSupportsCancellation = true;

            loadXml(ref existingConfig);

            if (existingConfig)
                initForms();
        
            verifyLicense(config.License);

            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Enabled = true;
            progressBar1.MarqueeAnimationSpeed = 100;

            lblStatus.Text = progressBar1.Value + "%";

            toolTip.InitialDelay = 100;
            toolTip.AutoPopDelay = 5000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            if (existingConfig)
            {
                statsPath = System.IO.Directory.GetCurrentDirectory() + "\\" + config.OutDir.Remove(0, 2).Replace("/", "").Replace(" ", "") + "\\stats.out";
                outPath = System.IO.Directory.GetCurrentDirectory() + "\\" + config.OutDir.Remove(0, 2).Replace("/", "").Replace(" ", "") + "\\final_nondom_pop.out";
            }

            //GMAP Settings
            gmap1.MapProvider = GoogleMapProvider.Instance;
            gmap1.Manager.Mode = AccessMode.ServerOnly;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap1.MinZoom = 1;
            gmap1.MaxZoom = 20;
            gmap1.Zoom = 20;
            gmap1.CanDragMap = true;
            gmap1.DragButton = MouseButtons.Left;
            gmap1.CanDragMap = true;
            gmap1.MapScaleInfoEnabled = true;
            gmap1.DragButton = MouseButtons.Left;

            gmap2.MapProvider = GoogleMapProvider.Instance;
            gmap2.Manager.Mode = AccessMode.ServerOnly;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap2.MinZoom = 1;
            gmap2.MaxZoom = 20;
            gmap2.Zoom = 20;
            gmap2.CanDragMap = true;
            gmap2.DragButton = MouseButtons.Left;
            gmap2.CanDragMap = true;
            gmap2.MapScaleInfoEnabled = true;
            gmap2.DragButton = MouseButtons.Left;

            importDataPoints();
        }

        delegate void SetTextCallback(string text);

        private bool hasWriteAccessToFolder(string folderPath)
        {
            try
            {               
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lblStatus.Text = text;
            }
        }
    
        #region GUI Interaction

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;     
            config.SaveToXml();
            progressBar1.Value = 0;

            lblStatus.Text = progressBar1.Value + "%";

            demoWorker.RunWorkerAsync();
            button1.Visible = true;
            running = true;
        }

        private int mkStrWidth(CheckedListBox clb)
        {
            Font fnt = new Font("Microsoft Sans Serif", 14F);
            int result = 0;
            foreach (string str in clb.Items)
            {
                int width = TextRenderer.MeasureText(str, fnt).Width;
                if (width > result)
                {
                    result = width;
                }
            }
            return result;
        }


        public void refreshComboList(List<int> list, ComboBox cb)
        {
            cb.DataSource = list;
        }

        private void txtndata_Leave(object sender, EventArgs e)
        {
            if (txtndata.Text == string.Empty)
                return;

            var ndata = Convert.ToInt32(txtndata.Text);
            var list = new List<int>();

            for (int i = 1; i <= ndata; i++)
            {
                list.Add(i);
            }

            refreshComboList(list, cbDemo);
        }

        private void txtncon_TextChanged(object sender, EventArgs e)
        {
            if (txtncon.Text == string.Empty)
                return;

            var ndata = Convert.ToInt32(txtncon.Text);
            var list = new List<int>();

            for (int i = 1; i <= ndata; i++)
            {
                list.Add(i);
            }

            refreshComboList(list, cbCon);
        }

        private void txtnobj_Leave(object sender, EventArgs e)
        {
            if (txtnobj.Text == string.Empty)
                return;

            var nobj = Convert.ToInt32(txtnobj.Text);
            var list = new List<int>();

            for (int i = 1; i <= nobj; i++)
            {
                list.Add(i);
            }

            refreshComboList(list, cbobj);
            refreshComboList(list, cbPerf);
        }

        private void txtNreal_Leave(object sender, EventArgs e)
        {
            if (txtNreal.Text == string.Empty)
                return;

            var ndata = Convert.ToInt32(txtNreal.Text);
            var list = new List<int>();

            for (int i = 1; i <= ndata; i++)
            {
                list.Add(i);
            }

            refreshComboList(list, cbReal);
        }

        private void txtNbin_Leave(object sender, EventArgs e)
        {
            if (txtNbin.Text == string.Empty)
                return;

            var ndata = Convert.ToInt32(txtNbin.Text);
            var list = new List<int>();

            for (int i = 1; i <= ndata; i++)
            {
                list.Add(i);
            }

            refreshComboList(list, cbBin);
        }

        private void cbDemo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbobj.SelectedIndex == -1)
                return;

            lblDemoTag.Text = "Objective " + (cbDemo.SelectedIndex + 1).ToString() + " Tag";
            lblDemoVal.Text = "Objective " + (cbDemo.SelectedIndex + 1).ToString() + " Value";

            if (config.AObj1.Count() > cbobj.SelectedIndex)
                txtaobj1.Text = config.AObj1.ToArray()[cbDemo.SelectedIndex].ToString().Replace(" ", "");

            if (config.AObj2.Count() > cbobj.SelectedIndex)
                txtaobj2.Text = config.AObj2.ToArray()[cbDemo.SelectedIndex].ToString().Replace(" ", "");


            saveDemoParameters();
        }

        private void cbCon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCon.SelectedIndex == -1)
                return;

            lblConNum.Text = "Constraint " + (cbCon.SelectedIndex + 1).ToString() + " Number";
            lblConTot.Text = "Constraint " + (cbCon.SelectedIndex + 1).ToString() + " Total";
            lblConInd.Text = "Constraint " + (cbCon.SelectedIndex + 1).ToString() + " Indicator";

            if (config.ConInd.Count() > cbCon.SelectedIndex)
                txtconind.Text = config.ConInd.ToArray()[cbCon.SelectedIndex];

            if (config.ConNum.Count() > cbCon.SelectedIndex)
                txtconnum.Text = config.ConNum.ToArray()[cbCon.SelectedIndex].ToString().Replace(" ", "");

            if (config.ConTot.Count() > cbCon.SelectedIndex)
                txtcontot.Text = config.ConTot.ToArray()[cbCon.SelectedIndex].ToString().Replace(" ", "");

            saveDemoParameters();
        }

        private void cbobj_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbobj.SelectedIndex == -1)
                return;

            lblObjTag.Text = "Objective " + (cbobj.SelectedIndex + 1).ToString() + " Tag";
            lblObjVal.Text = "Objective " + (cbobj.SelectedIndex + 1).ToString() + " Value";
            lblObjEps.Text = "Objective " + (cbobj.SelectedIndex + 1).ToString() + " EPS";

            if (config.ObjTags.Count() > cbobj.SelectedIndex)
                txtObjTag.Text = config.ObjTags.ToArray()[cbobj.SelectedIndex].Replace(" ", "");

            if (config.ObjVal.Count() > cbobj.SelectedIndex)
                txtObjVal.Text = config.ObjVal.ToArray()[cbobj.SelectedIndex].Replace(" ", "");

            if (config.ObjEps.Count() > cbobj.SelectedIndex)
                txtObjEps.Text = config.ObjEps.ToArray()[cbobj.SelectedIndex].ToString().Replace(" ", "");


            saveObjectiveParameters();
        }

        private void cbReal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbReal.SelectedIndex == -1)
                return;

            lblRealTag.Text = "Variable " + (cbReal.SelectedIndex + 1).ToString() + " Tag";
            lblRealMin.Text = "Variable " + (cbReal.SelectedIndex + 1).ToString() + " Min";
            lblRealMax.Text = "Variable " + (cbReal.SelectedIndex + 1).ToString() + " Max";

            if (config.RealTag != null)
            {
                if (config.RealTag.Count() > cbReal.SelectedIndex)
                    txtRealTag.Text = config.RealTag.ToArray()[cbReal.SelectedIndex].Replace(" ", "");
            }

            if (config.RealMin != null)
            {
                if (config.RealMin.Count() > cbReal.SelectedIndex)
                    txtRealMin.Text = config.RealMax.ToArray()[cbReal.SelectedIndex].ToString().Replace(" ", "");
            }

            if (config.RealMax != null)
            {
                if (config.RealMax.Count() > cbReal.SelectedIndex)
                    txtRealMax.Text = config.RealMin.ToArray()[cbReal.SelectedIndex].ToString().Replace(" ", "");
            }

            //saveRealVariableParameters();
        }

        private void cbBin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBin.SelectedIndex == -1)
                return;

            lblBinTag.Text = "Variable " + (cbBin.SelectedIndex + 1).ToString() + " Tag";
            lblBinMin.Text = "Variable " + (cbBin.SelectedIndex + 1).ToString() + " Min";
            lblBinMax.Text = "Variable " + (cbBin.SelectedIndex + 1).ToString() + " Max";
            lblBinBit.Text = "Variable " + (cbBin.SelectedIndex + 1).ToString() + " Bin";

            if (config.BinBit.Count() > cbBin.SelectedIndex)
                txtBinBit.Text = config.BinBit.ToArray()[cbBin.SelectedIndex].ToString().Replace(" ", "");

            if (config.BinTag.Count() > cbBin.SelectedIndex)
                txtBinTag.Text = config.BinTag.ToArray()[cbBin.SelectedIndex].ToString().Replace(" ", "");

            if (config.BinMin.Count() > cbBin.SelectedIndex)
                txtBinMin.Text = config.BinMin.ToArray()[cbBin.SelectedIndex].ToString().Replace(" ", "");

            if (config.BinMax.Count() > cbBin.SelectedIndex)
                txtBinMax.Text = config.BinMax.ToArray()[cbBin.SelectedIndex].ToString().Replace(" ", "");

            saveBinaryVariableParamters();
        }

        private void cbPerf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPerf.SelectedIndex == -1)
                return;

            lblDivGrid.Text = "Div Grid " + (cbPerf.SelectedIndex + 1 >= 3 ? 3 : cbPerf.SelectedIndex + 1).ToString();
            lblEperfEps.Text = "EPERF EPS " + (cbPerf.SelectedIndex + 1).ToString();

            savePerformanceMetrics();

            txtPDivGrid.Text = config.DivGrid.ToArray()[cbPerf.SelectedIndex > 3 ? 3 : cbPerf.SelectedIndex].ToString().Replace(" ", "");
            txtPEperfEps.Text = config.EperfEps.ToArray()[cbPerf.SelectedIndex].ToString().Replace(" ", "");
        }

        #endregion

        #region Save Methods

        private void btnSave_Click(object sender, EventArgs e)
        {
            //use tabControl1.SelectedIndex to determine which variables to save
            if (tabControl1.SelectedIndex == 0)
            {
                saveBaseParameters();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                saveObjectiveParameters();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                saveDemoParameters();
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                saveRealVariableParameters();
            }
            else if (tabControl1.SelectedIndex == 4)
            {
                saveBinaryVariableParamters();
            }
            else if (tabControl1.SelectedIndex == 5)
            {
                saveTerminationParameters();
            }
            else if (tabControl1.SelectedIndex == 6)
            {
                savePerformanceMetrics();
            }
            else if (tabControl1.SelectedIndex == 7)
            {
                saveLocalFileOutput();
            }

            config.SaveToXml();
        }

        private void saveBaseParameters()
        {
            if (txtBaseHboa.Text == string.Empty)
                txtBaseHboa.Text = "0";
            if (txtBaseMinPop.Text == string.Empty)
                txtBaseMinPop.Text = "0";
            if (txtBaseInitPop.Text == string.Empty)
                txtBaseInitPop.Text = "0";
            if (txtBaseMaxPop.Text == string.Empty)
                txtBaseMaxPop.Text = "0";
            if (txtBasePopFactor.Text == string.Empty)
                txtBasePopFactor.Text = "0.0";
            if (txtBaseMaxGen.Text == string.Empty)
                txtBaseMaxGen.Text = "0";
            if (txtBaseNumConds.Text == string.Empty)
                txtBaseNumConds.Text = "0";
            if (txtBaseNumBin.Text == string.Empty)
                txtBaseNumBin.Text = "0";
            if (txtBaseNumReal.Text == string.Empty)
                txtBaseNumReal.Text = "0";

            config.Save(parallel: txtBaseParallel.Text, debug: txtBaseDebug.Text, hboa: txtBaseHboa.Text, initPop: Convert.ToInt32(txtBaseInitPop.Text),
                   minPop: Convert.ToInt32(txtBaseMinPop.Text), maxPop: Convert.ToInt32(txtBaseMaxPop.Text), popScheme: txtBasePopScheme.Text, popFactor:
                   Convert.ToDouble(txtBasePopFactor.Text), maxGens: Convert.ToInt32(txtBaseMaxGen.Text), nObj: Convert.ToInt32(txtBaseNumObj.Text),
                   nCons: Convert.ToInt32(txtBaseNumConds.Text), nReal: Convert.ToInt32(txtBaseNumReal.Text), nBin: Convert.ToInt32(txtBaseNumBin.Text));
        }

        private void saveObjectiveParameters()
        {
            config.NObj = Convert.ToInt32(txtnobj.Text);
            var index = cbobj.SelectedIndex;

            if (config.ObjTags == null)
            {
                config.ObjTags = new List<string>(new string[config.NObj]);
                config.ObjEps = new List<double>(new double[config.NObj]);
                config.ObjVal = new List<string>(new string[config.NObj]);
            }

            while (config.ObjTags.Count <= index)
            {
                config.ObjTags.Add(string.Empty);
            }

            while (config.ObjVal.Count <= index)
            {
                config.ObjVal.Add(string.Empty);
            }

            while (config.ObjEps.Count <= index)
            {
                config.ObjEps.Add(0);
            }


            config.ObjTags[index] = txtObjTag.Text;

            if (txtObjEps.Text != string.Empty)
                config.ObjEps[index] = Convert.ToDouble(txtObjEps.Text);

            config.ObjVal[index] = txtObjVal.Text;

            config.Save(nObj: txtnobj.Text != string.Empty ? Convert.ToInt32(txtnobj.Text) : 0);
        }

        private void saveDemoParameters()
        {
            config.NData = txtSeriesLength.Text != string.Empty ? Convert.ToInt32(txtSeriesLength.Text) : config.NData;
            config.NAObj = txtndata.Text != string.Empty ? Convert.ToInt32(txtndata.Text) : config.NAObj;
            config.NConst = txtncon.Text != string.Empty ? Convert.ToInt32(txtncon.Text) : config.NConst;
            config.InputEntropyDir = txtEntropyInput.Text;
            config.InputObjDir = txtObjInput.Text;

            var index1 = cbDemo.SelectedIndex;
            var index2 = cbCon.SelectedIndex;

            if (config.NAObj > 0)
            {
                if (config.AObj1 == null || config.AObj1.Count < config.NAObj)
                {
                    config.AObj1 = new List<int>(new int[config.NAObj]);
                    config.AObj2 = new List<int>(new int[config.NAObj]);
                }

                if (index1 == -1)
                    return;

                config.AObj1[index1] = txtaobj1.Text != string.Empty ? Convert.ToInt32(txtaobj1.Text) : 0;
                config.AObj2[index1] = txtaobj2.Text != string.Empty ? Convert.ToInt32(txtaobj2.Text) : 0;
            }

            if (config.NConst > 0)
            {
                if (config.ConNum == null)
                {
                    config.ConNum = new List<int>(new int[config.NConst]);
                    config.ConTot = new List<int>(new int[config.NConst]);
                    config.ConInd = new List<string>(new string[config.NConst]);
                }

                if (index2 == -1)
                    return;

                if (config.ConNum.Count > index2) config.ConNum[index2] = txtconnum.Text != string.Empty ? Convert.ToInt32(txtconnum.Text) : 0;
                if (config.ConInd.Count > index2) config.ConInd[index2] = txtconind.Text;
                if (config.ConTot.Count > index2) config.ConTot[index2] = txtcontot.Text != string.Empty ? Convert.ToInt32(txtcontot.Text) : 0;
            }
        }

        private void saveRealVariableParameters()
        {
            config.NReal = txtNreal.Text != string.Empty ? Convert.ToInt32(txtNreal.Text) : 0;

            if (config.NReal > 0)
            {
                config.RealTag = config.RealTag == null ? new List<string>(new string[config.NReal]) : config.RealTag;
                config.RealMin = config.RealMin == null ? new List<double>(new double[config.NReal]) : config.RealMin;
                config.RealMax = config.RealMax == null ? new List<double>(new double[config.NReal]) : config.RealMax;

                var index = cbReal.SelectedIndex;

                config.RealTag[index] = txtRealTag.Text;

                int min = 0;
                int max = 0;

                if (int.TryParse(txtRealMin.Text, out min))
                    config.RealMin[index] = Convert.ToInt32(txtRealMin.Text);

                if (int.TryParse(txtRealMax.Text, out max))
                    config.RealMax[index] = Convert.ToInt32(txtRealMax.Text);
            }

            config.Save(pseudoBinary: txtRealPseudo.Text, realLimits: txtRealLimits.Text, realCrossProb: Convert.ToDouble(txtRealCross.Text),
                realMutProb: Convert.ToDouble(txtRealMut.Text), distIndexSBX: Convert.ToInt32(txtRealSbx.Text), distIndexPoly: Convert.ToInt32(txtRealPoly.Text));

        }

        private void saveBinaryVariableParamters()
        {
            config.NBin = txtNbin.Text != string.Empty ? Convert.ToInt32(txtNbin.Text) : 0;

            if (config.NBin > 0)
            {
                config.BinTag = config.BinTag == null ? new List<string>(new string[config.NBin]) : config.BinTag;
                config.BinMin = config.BinMin == null ? new List<int>(new int[config.NBin]) : config.BinMin;
                config.BinMax = config.BinMax == null ? new List<int>(new int[config.NBin]) : config.BinMax;
                config.BinBit = config.BinBit == null ? new List<int>(new int[config.NBin]) : config.BinBit;

                var index = cbBin.SelectedIndex;

                while (config.BinTag.Count <= index)
                {
                    config.BinTag.Add(string.Empty);
                }

                while (config.BinBit.Count <= index)
                {
                    config.BinBit.Add(0);
                }

                while (config.BinMin.Count <= index)
                {
                    config.BinMin.Add(0);
                }

                while (config.BinMax.Count <= index)
                {
                    config.BinMax.Add(0);
                }


                config.BinTag[index] = txtBinTag.Text;

                int min = 0;
                int max = 0;

                if (int.TryParse(txtBinMin.Text, out min))
                    config.BinMin[index] = Convert.ToInt32(txtBinMin.Text);

                if (int.TryParse(txtBinMax.Text, out max))
                    config.BinMax[index] = Convert.ToInt32(txtBinMax.Text);

                if (int.TryParse(txtBinMax.Text, out max))
                    config.BinBit[index] = Convert.ToInt32(txtBinBit.Text);
            }

            config.Save(binLimits: txtBinLimit.Text == "1" ? true : false, binCrossProb: Convert.ToDouble(txtBinCross.Text), binCrossType: txtBinType.Text,
                binMutProb: Convert.ToDouble(txtBinMut.Text), binMutType: txtBinMutType.Text);
        }

        public void saveTerminationParameters()
        {
            config.Save(interRun: txtTInterRun.Text, interDelta: Convert.ToDouble(txtTInterDelta.Text), intraRun: txtTIntraRun.Text,
                intraDelta: Convert.ToDouble(txtTIntraDelta.Text), maxNfe: Convert.ToInt32(txtTMaxNfe.Text), maxTime: Convert.ToDouble(txtTMaxTime.Text),
                maxEpert: Convert.ToDouble(txtTMaxEperf.Text));
        }

        public void savePerformanceMetrics()
        {

            config.DivGrid = config.DivGrid == null ? new List<int>(new int[3]) : config.DivGrid;
            config.EperfEps = config.EperfEps == null ? new List<Double>(new Double[4]) : config.EperfEps;

            var index = cbPerf.SelectedIndex;

            int div = 0;
            double eperf = 0;

            while (config.DivGrid.Count <= index)
                config.DivGrid.Add(0);

            while (config.EperfEps.Count <= index)
                config.DivGrid.Add(0);

            if (int.TryParse(txtPDivGrid.Text, out div) && index <= 3)
                config.DivGrid[index] = Convert.ToInt32(txtPDivGrid.Text);

            if (double.TryParse(txtPEperfEps.Text, out eperf))
                config.EperfEps[index] = Convert.ToDouble(txtPEperfEps.Text);

            config.Save(conv: txtPConv.Text, div: txtPDiv.Text, eperf: txtPEperf.Text, edomEperf: txtPEdom.Text, eind: txtPEind.Text,
                eindError: Convert.ToDouble(txtPEindError.Text), metricRef: txtPMetricRef.Text);
        }

        public void saveLocalFileOutput()
        {
            config.OutDir = txtOutDir.Text;
            config.OutHeaders = txtOutHead.Text;
            config.OutInterval = Convert.ToInt32(txtOutInt.Text);
            config.OutAll = Convert.ToInt32(txtOutInt.Text) > 1 ? true : false;
            config.NondomInterval = Convert.ToInt32(txtNondom.Text);
            config.Nondom = Convert.ToInt32(txtNondom.Text) > 1 ? true : false;
            config.NondomFinal = txtNondomFinal.Text == "1" ? true : false;
            config.AllFinal = txtAllFinal.Text == "1" ? true : false;
            config.StatsInterval = Convert.ToInt32(txtStatsInt.Text);
            config.Stats = config.StatsInterval != 0 ? true : false;
            config.TimerInterval = Convert.ToInt32(txtTimerInt.Text);
            config.Timer = config.TimerInterval != 0 ? true : false;
            config.RsStats = txtRsStats.Text == "1" ? true : false;
            config.VtkInterval = Convert.ToInt32(txtVtkInt.Text);
            config.UseVtk = config.VtkInterval > 0 ? true : false;
            config.SmallFlag = txtVtkFlag.Text;
        }

#endregion

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.FileName = "Configuration.xml";
            saveFileDialog.OverwritePrompt = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK || saveFileDialog.FileName != string.Empty)
            {
                config.SaveToXml(saveFileDialog.FileName);
            }
        }

        private void demoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!demoWorker.CancellationPending)
            {
                //call DEMO.EXE
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "DEMO.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "configuration.xml";
                startInfo.RedirectStandardOutput = true;

                if (File.Exists(statsPath))
                    File.Delete(statsPath);

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        var running = true;

                        while (running)
                        {
                            var subRun = false;

                            foreach (Process clsProcess in Process.GetProcesses())
                            {
                                if (clsProcess.ProcessName.Equals("DEMO"))
                                {
                                    subRun = true;
                                }
                            }

                            running = subRun;

                            if (!File.Exists(statsPath))
                                continue;

                            FileStream logFileStream = new FileStream(statsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader logFileReader = new StreamReader(logFileStream);
                            string line = "";

                            while (!logFileReader.EndOfStream)
                            {
                                line = logFileReader.ReadLine();
                            }

                            logFileReader.Close();
                            logFileStream.Close();


                            var values = line.Split('\t');
                            var progress = values.Count() >= 7 ? values[6] : progressBar1.Value.ToString();
                            var prog = Convert.ToInt32(progress);
                            var val = ((100 * prog) / config.MaxNfe) > 100 ? 100 : ((100 * prog) / config.MaxNfe);
                            demoWorker.ReportProgress(val);
                        }

                        exeProcess.WaitForExit();
                    }
                }
                catch
                {

                }

                demoWorker.ReportProgress(100);
                demoWorker.CancelAsync();
                //SetText("100%");
            }

        } 

        private void demoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.Enabled = true;

            if (running)
            {
                progressBar1.Value = progressBar1.Maximum;
                lblStatus.Text = "100%";
            }
            else
            {
                progressBar1.Value = progressBar1.Minimum;
                lblStatus.Text = "0%";
            }

            button1.Visible = false;

            var weightedPoints = new List<List<double>>();
            string pathDir = config.OutDir;

            if (config.OutDir.ElementAt(0) == 32 && config.OutDir.ElementAt(1) == 46)
            {
                pathDir = pathDir.Replace('/', '\\');
                pathDir = pathDir.Replace('.', '\\');
                pathDir = Directory.GetCurrentDirectory() + pathDir;
            }
            
            DemoUtil.ImportDataFromCS(pathDir + "\\final_nondom_pop.out", weightedPoints, numSelectedPoints);

            if (weightedPoints.Count() > 0)
            {
                assignMapWeights(weightedPoints);
            }

            var a = new List<double>();
            var b = new List<double>();
            var c = new List<double>();


            DemoUtil.ImportGraphDataFromCS(pathDir + "\\final_nondom_pop.out", a, b, c);

            var x = a.ToArray();
            var y = b.ToArray();
            var z = c.ToArray();

            for (var i = 0; i < a.Count(); i++)
            {
                outputChart.Series["OutputXY"].Points.AddXY(x[i], y[i]);
                outputChart.Series["OutputXZ"].Points.AddXY(x[i], z[i]);
                outputChart.Series["OutputYZ"].Points.AddXY(y[i], z[i]);

                outputChart.Series["OutputXY"].BorderWidth = 10;
                outputChart.Series["OutputXZ"].BorderWidth = 10;
                outputChart.Series["OutputYZ"].BorderWidth = 10;

                
            }

            tabControl2.SelectedIndex = 2;
            centerMap1();
        }

        private void demoWorker_ProgressedChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
            progressBar1.Refresh();
            lblStatus.Text = progressBar1.Value + "%";
        }

        private void txtBaseNumObj_TextChanged(object sender, EventArgs e)
        {
            txtnobj.Text = txtBaseNumObj.Text;
        }

        private void txtnobj_TextChanged(object sender, EventArgs e)
        {
            txtBaseNumObj.Text = txtnobj.Text;
        }

        private void txtOutDir_MouseDown(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (fbd.SelectedPath != string.Empty)
            {
                txtOutDir.Text = fbd.SelectedPath;
                config.OutDir = fbd.SelectedPath;
            }
        }

        private void loadXml(ref bool existingConfig)
        {
            if (File.Exists("configuration.xml"))
            {
                existingConfig = true;

                try
                {
                    var doc = XDocument.Load("configuration.xml");

                    config.LinearInput = doc.Root.Element("linearInput") != null ? doc.Root.Element("linearInput").Value : "";
                    config.InputEntropyDir = doc.Root.Element("entropyDir").Value;
                    config.InputObjDir = doc.Root.Element("objDir").Value;
                    config.License = doc.Root.Element("license").Value;
                    config.Debug = doc.Root.Element("debug_mode").Value;
                    config.Hboa = doc.Root.Element("use_hboa").Value;
                    config.InitPop = Convert.ToInt32(doc.Root.Element("initial_popsize").Value);
                    config.MinPop = Convert.ToInt32(doc.Root.Element("min_popsize").Value);
                    config.MaxPop = Convert.ToInt32(doc.Root.Element("max_popsize").Value);
                    config.PopScheme = doc.Root.Element("popsize_scheme").Value;
                    config.PopFactor = Convert.ToDouble(doc.Root.Element("pop_scaling_factor").Value);
                    config.MaxGens = Convert.ToInt32(doc.Root.Element("max_gens").Value);
                    config.TestProblem = doc.Root.Element("test_problem").Value;

                    config.NObj = Convert.ToInt32(doc.Root.Element("nobj").Value);
                    config.ObjTags = new List<string>();
                    config.ObjVal = new List<string>();
                    config.ObjEps = new List<double>();

                    for (var i = 1; i <= config.NObj; i++)
                    {
                        config.ObjTags.Add(doc.Root.Element("obj_tag" + i).Value);
                        config.ObjVal.Add(doc.Root.Element("obj_val" + i).Value);
                        config.ObjEps.Add(Convert.ToDouble(doc.Root.Element("obj_eps" + i).Value));
                    }

                    config.NConst = Convert.ToInt32(doc.Root.Element("ncons").Value);
                    config.ConNum = new List<int>();
                    config.ConTot = new List<int>();
                    config.ConInd = new List<string>();

                    for (var i = 1; i <= config.NConst; i++)
                    {
                        if (doc.Root.Element("con_num" + i) != null)
                            config.ConNum.Add(Convert.ToInt32(doc.Root.Element("con_num" + i).Value));

                        if (doc.Root.Element("con_tot" + i) != null)
                            config.ConTot.Add(Convert.ToInt32(doc.Root.Element("con_tot" + i).Value));

                        if (doc.Root.Element("con_ind" + i) != null)
                            config.ConInd.Add(doc.Root.Element("con_ind" + i).Value);
                    }

                    config.NReal = Convert.ToInt32(doc.Root.Element("nreal").Value);
                    config.PseudoBinary = doc.Root.Element("real_pseudo_binary").Value;
                    config.RealLimits = doc.Root.Element("real_limits").Value;
                    config.RealTag = new List<string>();
                    config.RealMin = new List<double>();
                    config.RealMax = new List<double>();

                    if (config.RealLimits.ToLower() == " same ")
                    {
                        config.RealTag.Add(doc.Root.Element("real_tag1").Value);
                        config.RealMin.Add(Convert.ToDouble(doc.Root.Element("real_min1").Value));
                        config.RealMax.Add(Convert.ToDouble(doc.Root.Element("real_max1").Value));
                    }
                    else
                    {
                        for (int i = 1; i <= config.NReal; i++)
                        {
                            config.RealTag.Add(doc.Root.Element("real_tag" + i).Value);
                            config.RealMin.Add(Convert.ToInt32(doc.Root.Element("real_min" + i).Value));
                            config.RealMax.Add(Convert.ToInt32(doc.Root.Element("real_max" + i).Value));
                        }
                    }

                    config.RealCrossProb = doc.Root.Element("real_cross_prob") != null ? Convert.ToDouble(doc.Root.Element("real_cross_prob").Value) : 0;
                    config.RealMutProb = doc.Root.Element("real_mut_prob") != null ? Convert.ToDouble(doc.Root.Element("real_mut_prob").Value) : 0;
                    config.DistIndexSBX = doc.Root.Element("dist_index_sb") != null ? Convert.ToInt32(doc.Root.Element("dist_index_sb").Value) : 0;
                    config.DistIndexPoly = doc.Root.Element("dist_index_poly") != null ? Convert.ToInt32(doc.Root.Element("dist_index_poly").Value) : 0;

                    config.NBin = doc.Root.Element("nbin") != null ? Convert.ToInt32(doc.Root.Element("nbin").Value) : 0;
                    config.BinLimits = doc.Root.Element("bin_limits") != null ? doc.Root.Element("bin_limits").Value.ToLower() == " same " ? true : false : false;

                    config.BinTag = new List<string>();
                    config.BinBit = new List<int>();
                    config.BinMin = new List<int>();
                    config.BinMax = new List<int>();

                    if (config.BinLimits)
                    {
                        config.BinTag.Add(doc.Root.Element("bin_tag1") != null ? doc.Root.Element("bin_tag1").Value : string.Empty);
                        config.BinBit.Add(doc.Root.Element("bin_bit1") != null ? Convert.ToInt32(doc.Root.Element("bin_bit1").Value) : 0);
                        config.BinMin.Add(doc.Root.Element("bin_min1") != null ? Convert.ToInt32(doc.Root.Element("bin_min1").Value) : 0);
                        config.BinMax.Add(doc.Root.Element("bin_max1") != null ? Convert.ToInt32(doc.Root.Element("bin_max1").Value) : 0);
                    }
                    else
                    {
                        for (var i = 1; i <= config.NBin; i++)
                        {
                            config.BinTag.Add(doc.Root.Element("bin_tag" + i) != null ? doc.Root.Element("bin_tag" + i).Value : string.Empty);
                            config.BinBit.Add(doc.Root.Element("bin_bit" + i) != null ? Convert.ToInt32(doc.Root.Element("bin_bit" + i).Value) : 0);
                            config.BinMin.Add(doc.Root.Element("bin_min" + i) != null ? Convert.ToInt32(doc.Root.Element("bin_min" + i).Value) : 0);
                            config.BinMax.Add(doc.Root.Element("bin_max" + i) != null ? Convert.ToInt32(doc.Root.Element("bin_max" + i).Value) : 0);
                        }

                    }

                    config.BinCrossProb = doc.Root.Element("bin_cross_prob") != null ? Convert.ToDouble(doc.Root.Element("bin_cross_prob").Value) : 0;
                    config.BinCrossType = doc.Root.Element("bin_cross_type") != null ? doc.Root.Element("bin_cross_type").Value : string.Empty;
                    config.BinMutProb = doc.Root.Element("bin_mut_prob") != null ? Convert.ToDouble(doc.Root.Element("bin_mut_prob").Value) : 0;
                    config.BinMutType = doc.Root.Element("bin_mut_type") != null ? doc.Root.Element("bin_mut_type").Value : string.Empty;

                    config.InterRun = doc.Root.Element("inter_run") != null ? doc.Root.Element("inter_run").Value : string.Empty;
                    config.InterDelta = doc.Root.Element("inter_delta") != null ? Convert.ToInt32(doc.Root.Element("inter_delta").Value) : 0;
                    config.IntraRun = doc.Root.Element("intra_run") != null ? doc.Root.Element("intra_run").Value : string.Empty;
                    config.IntraDelta = doc.Root.Element("intra_delta") != null ? Convert.ToInt32(doc.Root.Element("intra_delta").Value) : 0;

                    //init_lag_win not used
                    //init_lag_win_runs not used
                    //final_lag_win not used

                    config.MaxNfe = doc.Root.Element("max_nfe") != null ? Convert.ToInt32(doc.Root.Element("max_nfe").Value) : 0;
                    config.MaxTime = doc.Root.Element("max_time") != null ? Convert.ToDouble(doc.Root.Element("max_time").Value) : 0;
                    config.MaxEperf = doc.Root.Element("max_eperf") != null ? Convert.ToDouble(doc.Root.Element("max_eperf").Value) : 0;

                    config.Conv = doc.Root.Element("conv") != null ? doc.Root.Element("conv").Value : string.Empty;
                    config.Div = doc.Root.Element("div") != null ? doc.Root.Element("div").Value : string.Empty;
                    config.Eperf = doc.Root.Element("eperf") != null ? doc.Root.Element("eperf").Value : string.Empty;
                    config.EdomEperf = doc.Root.Element("edom_eperf") != null ? doc.Root.Element("edom_eperf").Value : string.Empty;
                    config.Eind = doc.Root.Element("eind") != null ? doc.Root.Element("eind").Value : string.Empty;
                    config.EindError = doc.Root.Element("eind_error") != null ? Convert.ToDouble(doc.Root.Element("eind_error").Value) : 0;
                    config.MetricRef = doc.Root.Element("metric_ref") != null ? doc.Root.Element("metric_ref").Value : string.Empty;

                    config.DivGrid = new List<int>();
                    config.EperfEps = new List<double>();

                    for (var i = 1; i <= 4; i++)
                    {
                        if (i < 4)
                            config.DivGrid.Add(doc.Root.Element("div_grid" + i) != null ? Convert.ToInt32(doc.Root.Element("div_grid" + i).Value) : 0);

                        config.EperfEps.Add(doc.Root.Element("eperf_eps" + i) != null ? Convert.ToDouble(doc.Root.Element("eperf_eps" + i).Value) : 0);
                    }

                    config.OutDir = doc.Root.Element("out_dir") != null ? doc.Root.Element("out_dir").Value : string.Empty;
                    config.OutHeaders = doc.Root.Element("out_headers") != null ? doc.Root.Element("out_headers").Value : string.Empty;
                    config.OutInterval = doc.Root.Element("out_all") != null ? Convert.ToInt32(doc.Root.Element("out_all").Value) : 0;
                    config.OutAll = config.OutInterval > 0 ? true : false;
                    config.NondomInterval = doc.Root.Element("out_nondom") != null ? Convert.ToInt32(doc.Root.Element("out_nondom").Value) : 0;
                    config.Nondom = config.NondomInterval > 0 ? true : false;
                    config.AllFinal = doc.Root.Element("out_all_final") != null ? doc.Root.Element("out_all_final").Value == " on " ? true : false : false;
                    config.NondomFinal = doc.Root.Element("out_nondom_final") != null ? doc.Root.Element("out_nondom_final").Value == " on " ? true : false : false;
                    config.StatsInterval = doc.Root.Element("out_stats") != null ? Convert.ToInt32(doc.Root.Element("out_stats").Value) : 0;
                    config.Stats = config.StatsInterval > 0 ? true : false;
                    config.RsStats = doc.Root.Element("out_rs") != null ? doc.Root.Element("out_rs").Value == " on " ? true : false : false;
                    config.TimerInterval = doc.Root.Element("out_timer") != null ? Convert.ToInt32(doc.Root.Element("out_timer").Value) : 0;
                    config.Timer = config.TimerInterval > 0 ? true : false;
                    config.VtkInterval = doc.Root.Element("out_vtk") != null ? Convert.ToInt32(doc.Root.Element("out_vtk").Value) : 0;
                    config.UseVtk = config.VtkInterval > 0 ? true : false;
                    config.SmallFlag = doc.Root.Element("out_vtk_sflag") != null ? doc.Root.Element("out_vtk_sflag").Value : string.Empty;

                    config.NData = doc.Root.Element("ndata") != null ? Convert.ToInt32(doc.Root.Element("ndata").Value) : 0;
                    config.NAObj = doc.Root.Element("num_additional_objectives") != null ? Convert.ToInt32(doc.Root.Element("num_additional_objectives").Value) : 0;
                    config.AObj1 = new List<int>();
                    config.AObj2 = new List<int>();

                    for (var i = 1; i <= config.NAObj; i++)
                    {
                        config.AObj1.Add(doc.Root.Element("aobj1_" + i) != null ? Convert.ToInt32(doc.Root.Element("aobj1_" + i).Value) : 0);
                        config.AObj2.Add(doc.Root.Element("aobj" + i + "_2") != null ? Convert.ToInt32(doc.Root.Element("aobj" + i + "_2").Value) : 0);
                    }

                    if (config.RealLimits == " same ")
                        cbReal.Enabled = false;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            else
            {
                existingConfig = false;
                bool dummy = true;
                config.SaveToXml();
                loadXml(ref dummy);
            }
        }

        private void initForms()
        {
            // TODO add ternary logic for each statement

            // this is not great
            try
            {

                //txtBaseDebug.Text = config.Debug.Replace(" ", "");
                txtLinear.Text = config.LinearInput.Replace(" ", "");
                txtEntropyInput.Text = config.InputEntropyDir.Replace(" ", "");
                txtObjInput.Text = config.InputObjDir.Replace(" ", "");
                txtBaseHboa.Text = config.Hboa.Replace(" ", "");
                txtBaseInitPop.Text = config.InitPop.ToString().Replace(" ", "");
                txtBaseMaxGen.Text = config.MaxGens.ToString().Replace(" ", "");
                txtBaseMaxPop.Text = config.MaxPop.ToString().Replace(" ", "");
                txtBaseMinPop.Text = config.MinPop.ToString().Replace(" ", "");
                txtBaseNumBin.Text = config.NBin.ToString().Replace(" ", "");
                txtBaseNumConds.Text = config.NCons.ToString().Replace(" ", "");
                txtBaseNumObj.Text = config.NObj.ToString().Replace(" ", "");
                txtBaseNumReal.Text = config.NReal.ToString().Replace(" ", "");
                txtBaseNumTag.Text = config.NTagAlong.ToString().Replace(" ", "");
                txtBasePopFactor.Text = config.PopFactor.ToString().Replace(" ", "");
                txtBasePopScheme.Text = config.PopScheme.Replace(" ", "");
                txtTestProblem.Text = config.TestProblem;

                var list = new List<int>();

                for (int i = 1; i <= config.NObj; i++)
                    list.Add(i);

                refreshComboList(list, cbobj);
                list.Clear();

                txtSeriesLength.Text = config.NData.ToString().Replace(" ", "");
                txtndata.Text = config.NAObj.ToString().Replace(" ", "");

                for (int i = 1; i <= config.NAObj; i++)
                    list.Add(i);

                refreshComboList(list, cbDemo);
                list.Clear();

                txtncon.Text = config.NConst.ToString().Replace(" ", "");

                for (int i = 1; i <= config.NConst; i++)
                    list.Add(i);

                refreshComboList(list, cbCon);
                list.Clear();

                txtNreal.Text = config.NReal.ToString().Replace(" ", "");

                for (int i = 1; i <= config.NReal; i++)
                    list.Add(i);

                refreshComboList(list, cbReal);
                list.Clear();

                txtRealPseudo.Text = config.PseudoBinary.Replace(" ", "");
                txtRealCross.Text = config.RealCrossProb.ToString().Replace(" ", "");
                txtRealDec.Enabled = false;
                txtRealLimits.Text = config.RealLimits.Replace(" ", "");
                txtRealMut.Text = config.RealMutProb.ToString().Replace(" ", "");
                txtRealPoly.Text = config.DistIndexPoly.ToString().Replace(" ", "");
                txtRealSbx.Text = config.DistIndexSBX.ToString().Replace(" ", "");

                txtNbin.Text = config.NBin.ToString().Replace(" ", "");
                txtBinLimit.Text = config.BinLimits ? "same" : "different";
                txtBinCross.Text = config.BinCrossProb.ToString().Replace(" ", "");
                txtBinType.Text = config.BinCrossType.Replace(" ", "");
                txtBinMut.Text = config.BinMutProb.ToString().Replace(" ", "");
                txtBinMutType.Text = config.BinMutType.Replace(" ", "");

                for (var i = 1; i <= config.NBin; i++)
                    list.Add(i);

                refreshComboList(list, cbBin);
                list.Clear();

                txtTInterDelta.Text = config.InterDelta.ToString().Replace(" ", "");
                txtTInterRun.Text = config.InterRun.ToString().Replace(" ", "");
                txtTIntraDelta.Text = config.IntraDelta.ToString().Replace(" ", "");
                txtTIntraRun.Text = config.IntraRun.ToString().Replace(" ", "");
                txtTMaxEperf.Text = config.MaxEperf.ToString().Replace(" ", "");
                txtTMaxNfe.Text = config.MaxNfe.ToString().Replace(" ", "");
                txtTMaxTime.Text = config.MaxTime.ToString().Replace(" ", "");

                txtPConv.Text = config.Conv.Replace(" ", "");
                txtPDiv.Text = config.Div.Replace(" ", "");
                txtPEperf.Text = config.Eperf.ToString().Replace(" ", "");
                txtPEdom.Text = config.EdomEperf.ToString().Replace(" ", "");
                txtPEind.Text = config.Eind.ToString().Replace(" ", "");
                txtPEindError.Text = config.EindError.ToString().Replace(" ", "");
                txtPMetricRef.Text = config.MetricRef.ToString().Replace(" ", "");

                for (var i = 1; i <= 4; i++)
                    list.Add(i);

                refreshComboList(list, cbPerf);
                list.Clear();

                txtTimerInt.Text = config.TimerInterval.ToString().Replace(" ", "");
                txtOutHead.Text = config.OutHeaders.Replace(" ", "");
                txtOutInt.Text = config.OutInterval.ToString();
                txtOutDir.Text = config.OutDir.Remove(0, 1).Remove(config.OutDir.ToString().Length - 2, 1);
                txtNondom.Text = config.NondomInterval.ToString().Replace(" ", "");
                txtAllFinal.Text = config.AllFinal ? "on" : "off";
                txtNondomFinal.Text = config.NondomFinal ? "on" : "off";
                txtStatsInt.Text = config.StatsInterval.ToString().Replace(" ", "");
                txtVtkInt.Text = config.VtkInterval.ToString().Replace(" ", "");
                txtVtkFlag.Text = config.SmallFlag.Replace(" ", "");
                txtRsStats.Text = config.RsStats.ToString().Replace(" ", "");
            }
            catch { }

        }

        private void txtRealLimits_TextChanged(object sender, EventArgs e)
        {
            if (txtRealLimits.Text.Contains("same") == false)
                cbReal.Enabled = true;
            else
                cbReal.Enabled = false;
        }

        private void addPointMap1(string inputLat, string inputLng, GMarkerGoogleType colour)
        {

            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(Convert.ToDouble(inputLat), Convert.ToDouble(inputLng)),
              colour);


            markersOverlay.Markers.Add(marker);

            if (gmap1.Overlays.Count == 0)
                gmap1.Overlays.Add(markersOverlay);
            else
            {
                foreach (var markers in gmap1.Overlays.FirstOrDefault().Markers)
                {
                    if (markers.Position.Lat == Convert.ToDouble(inputLat) && markers.Position.Lng == Convert.ToDouble(inputLng))
                    {
                        return;
                    }
                }

                gmap1.Overlays.FirstOrDefault().Markers.Add(marker);
            }
        }

        private void addPointMap2(string inputLat, string inputLng)
        {

            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(Convert.ToDouble(inputLat), Convert.ToDouble(inputLng)),
              GMarkerGoogleType.green);


            markersOverlay.Markers.Add(marker);
            //markersOverlay.Markers.Add(marker1);
            //markersOverlay.Markers.Add(marker2);

            if (gmap2.Overlays.Count == 0)
                gmap2.Overlays.Add(markersOverlay);
            else
            {
                foreach (var markers in gmap2.Overlays.FirstOrDefault().Markers)
                {
                    if (markers.Position.Lat == Convert.ToDouble(inputLat) && markers.Position.Lng == Convert.ToDouble(inputLng))
                    {
                        return;
                    }
                }

                gmap2.Overlays.FirstOrDefault().Markers.Add(marker);
            }
        }


        private void centerMap1()
        {
            gmap1.Zoom = 15;

            if (gmap1.Overlays.FirstOrDefault() == null)
            {
                return;
            }
                        
            var posLat = Math.Abs(Convert.ToDecimal(gmap1.Position.Lat));
            var posLng = Math.Abs(Convert.ToDecimal(gmap1.Position.Lng));
            var viewLat = Convert.ToDecimal(gmap1.ViewArea.HeightLat);
            var viewLng = Convert.ToDecimal(gmap1.ViewArea.WidthLng);
            decimal x = 0;
            decimal y = 0;

            if (gmap1.Overlays.Count == 0 || gmap1.Overlays.FirstOrDefault().Markers == null)
                return;

            foreach (var marker in gmap1.Overlays.Where(z => z.Id == "markers").FirstOrDefault().Markers)
            {
                var lng = Math.Abs(Convert.ToDecimal(marker.Position.Lng));
                var lat = Math.Abs(Convert.ToDecimal(marker.Position.Lat));
                x += Convert.ToDecimal(marker.Position.Lng);
                y += Convert.ToDecimal(marker.Position.Lat);
                var zoomOut = true;

                while (zoomOut)
                {
                    if ((posLng + viewLng >= lng && posLng - viewLng <= lng) && (posLat + viewLat >= lat && posLat - viewLat <= lat))
                    {
                        zoomOut = false;
                    }

                    if (zoomOut)
                    {
                        gmap1.Zoom = gmap1.Zoom - 1;

                        posLat = Math.Abs(Convert.ToDecimal(gmap1.Position.Lat));
                        posLng = Math.Abs(Convert.ToDecimal(gmap1.Position.Lng));
                        viewLat = Convert.ToDecimal(gmap1.ViewArea.HeightLat);
                        viewLng = Convert.ToDecimal(gmap1.ViewArea.WidthLng);
                    }


                }
            }

            gmap1.Zoom = gmap1.Zoom - 1;
            var markersCount = gmap1.Overlays.Where(z => z.Id == "markers").FirstOrDefault().Markers.Count;

            x = x / markersCount;
            y = y / markersCount;

            gmap1.Position = new PointLatLng(Convert.ToDouble(y), Convert.ToDouble(x));
        }

        private void centerMap2()
        {
            if (gmap2.Overlays.FirstOrDefault() == null)
                return;

            gmap2.Zoom = 20;

            if (gmap2.Overlays.FirstOrDefault() == null)
                return;


            gmap2.Position = gmap2.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position;
            var posLat = Math.Abs(Convert.ToDecimal(gmap2.Position.Lat));
            var posLng = Math.Abs(Convert.ToDecimal(gmap2.Position.Lng));
            var viewLat = Convert.ToDecimal(gmap2.ViewArea.HeightLat);
            var viewLng = Convert.ToDecimal(gmap2.ViewArea.WidthLng);
            decimal x = 0;
            decimal y = 0;

            if (gmap2.Overlays.Count == 0 || gmap2.Overlays.FirstOrDefault().Markers == null)
                return;


            foreach (var marker in gmap2.Overlays.FirstOrDefault().Markers)
            {
                var lng = Math.Abs(Convert.ToDecimal(marker.Position.Lng));
                var lat = Math.Abs(Convert.ToDecimal(marker.Position.Lat));
                x += Convert.ToDecimal(marker.Position.Lng);
                y += Convert.ToDecimal(marker.Position.Lat);
                var zoomOut = true;

                while (zoomOut)
                {
                    if ((posLng + viewLng >= lng && posLng - viewLng <= lng) && (posLat + viewLat >= lat && posLat - viewLat <= lat))
                    {
                        zoomOut = false;
                    }

                    if (zoomOut)
                    {
                        gmap2.Zoom = gmap2.Zoom - 1;

                        posLat = Math.Abs(Convert.ToDecimal(gmap2.Position.Lat));
                        posLng = Math.Abs(Convert.ToDecimal(gmap2.Position.Lng));
                        viewLat = Convert.ToDecimal(gmap2.ViewArea.HeightLat);
                        viewLng = Convert.ToDecimal(gmap2.ViewArea.WidthLng);
                    }


                }
            }

            gmap2.Zoom = gmap2.Zoom - 1;

            x = x / gmap2.Overlays.FirstOrDefault().Markers.Count;
            y = y / gmap2.Overlays.FirstOrDefault().Markers.Count;

            gmap2.Position = new PointLatLng(Convert.ToDouble(y), Convert.ToDouble(x));
        }


        private void btnCenter_Click(object sender, EventArgs e)
        {
            centerMap1();
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == null)
                return String.Empty;

            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void verifyLicense(string input)
        {
            string license = input;

            if (Base64Decode(license).Contains("tethys") == true)
                return;

            LicenseForm licForm = new LicenseForm();
            this.Enabled = false;

            var result = licForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.Enabled = true;
                config.License = licForm.license;
                config.SaveToXml();
            }
        }

        private void txtEntropyInput_MouseDown(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "Comma Separated Value(.csv)|*.csv|All Files (*.*)|*.*";
            fbd.FilterIndex = 1;
            DialogResult result = fbd.ShowDialog();

            if (fbd.FileName != string.Empty)
            {
                txtEntropyInput.Text = fbd.FileName;
                config.InputEntropyDir = fbd.FileName;
            }

            importDataPoints();
        }

        private void txtObjInput_MouseDown(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            DialogResult result = fbd.ShowDialog();

            if (fbd.FileName != string.Empty)
            {
                txtObjInput.Text = fbd.FileName;
                config.InputObjDir = fbd.FileName;
            }
        } 

        private void txtTestProblem_TextChanged(object sender, EventArgs e)
        {
            config.TestProblem = txtTestProblem.Text;
        }

        public static void EnableTab(TabPage page, bool enable) 
        {
            foreach (Control ctl in page.Controls)
            {
                ctl.Enabled = enable;
            }
        }

        private void txtLinear_MouseDown(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "Comma Separated Value(.csv)|*.csv";
            fbd.FilterIndex = 1;
            DialogResult result = fbd.ShowDialog();

            if (fbd.FileName != string.Empty)
            {
                txtLinear.Text = fbd.FileName;
                config.LinearInput = txtLinear.Text;
            }
        }

        private void btnLinearImport_Click(object sender, EventArgs e)
        {
            if (!File.Exists(config.LinearInput))
            {
                MessageBox.Show("Unable to read Linear Input file: " + config.LinearInput, "I/O Error");
                return;
            }

            if (linearCoords == null)
                linearCoords = new List<List<Double>>();

            if (linearData == null)
                linearData = new List<List<int>>();

            int dummyVal = 0;

            if (File.Exists(config.LinearInput))
            {
                DemoUtil.ImportDataFromCSV(config.LinearInput, linearCoords, linearData, ref dummyVal);
                addPointsMap2(linearCoords);
                centerMap2();
            }      

            double[,] linearDataIn = new double[linearData.Count, linearData[0].Count];

            var ranks = DemoUtil.LinearRank(linearDataIn, linearData.Count, linearData[0].Count);
            


        } 

        private void assignMap1Weights()
        {
            var weightedPoints = new List<List<double>>();
            string pathDir = config.OutDir;

            if (config.OutDir.ElementAt(0) == 32 && config.OutDir.ElementAt(1) == 46)
            {
                pathDir = pathDir.Replace('/', '\\');
                pathDir = pathDir.Replace('.', '\\');
                pathDir = pathDir.Replace("\\ ", "\\");
                pathDir = Directory.GetCurrentDirectory() + pathDir;
                pathDir = pathDir.Replace(" \\", "\\");

            }

            DemoUtil.ImportDataFromCS(pathDir + "final_nondom_pop.out", weightedPoints, numSelectedPoints);

            if (weightedPoints.Count() > 0)
            {
                assignMapWeights(weightedPoints);
            }

            var a = new List<double>();
            var b = new List<double>();
            var c = new List<double>();


            DemoUtil.ImportGraphDataFromCS(pathDir + "final_nondom_pop.out", a, b, c);

            var x = a.ToArray();
            var y = b.ToArray();
            var z = c.ToArray();

            for (var i = 0; i < a.Count(); i++)
            {
                outputChart.Series["OutputXY"].Points.AddXY(x[i], y[i]);
                outputChart.Series["OutputXZ"].Points.AddXY(x[i], z[i]);
                outputChart.Series["OutputYZ"].Points.AddXY(y[i], z[i]);

                outputChart.Series["OutputXY"].BorderWidth = 10;
                outputChart.Series["OutputXZ"].BorderWidth = 10;
                outputChart.Series["OutputYZ"].BorderWidth = 10;


            }

            tabControl2.SelectedIndex = 2;
            centerMap1();
        }

        private void addPointsMap1(List<List<double>> input)
        {
            var pointsList = new List<PointLatLng>();

            for (var i = 0; i < input[0].Count; i++)
            {
                addPointMap1(input[0][i].ToString(), input[1][i].ToString(), GMarkerGoogleType.green);
                pointsList.Add(new PointLatLng(input[0][i], input[1][i]));
            }

            map1Center = pointsList.FirstOrDefault();
            listBoxPoints.DataSource = pointsList;
            assignMap1Weights();
            centerMap1();            
        }

        private void addPointsMap2(List<List<double>> input)
        {
            var pointsList = new List<PointLatLng>();

            for (var i = 0; i < input[0].Count; i++)
            {
                addPointMap2(input[0][i].ToString(), input[1][i].ToString());
                pointsList.Add(new PointLatLng(input[0][i], input[1][i]));
            }

            listBoxLinear.DataSource = pointsList;

            centerMap2();
        }

        private void importDataPoints()
        {
            //coords = DemoUtil.ImportCoordsFromCSV(config.InputEntropyDir);
            var coords = new List<List<double>>();
            var data = new List<List<int>>();

            DemoUtil.ImportDataFromCSV(config.InputEntropyDir, coords, data, ref numSelectedPoints);

            //basic implementation, should be using entropy calc
            if (coords.Count() == 0)
                return;

            addPointsMap1(coords);
            createEntropyFile(data);
            centerMap1();
            //to here

            int[,] dataArray = new int[data.Count, data[0].Count];

            //DemoUtil.LinearRank(dataArray, data.Count, data[0].Count);

            if (listBoxPoints.Items.Count >= 2)
                btnStart.Enabled = true;
        }


        //private void btnImportData_Click(object sender, EventArgs e)
        //{
        //    //coords = DemoUtil.ImportCoordsFromCSV(config.InputEntropyDir);
        //    var coords = new List<List<double>>();
        //    var data = new List<List<int>>();

        //    DemoUtil.ImportDataFromCSV(config.InputEntropyDir, coords, data);

        //    //basic implementation, should be using entropy calc
        //    addPointsMap1(coords);
        //    createEntropyFile(data);
        //    centerMap1();
        //    //to here

        //    int[,] dataArray = new int[data.Count, data[0].Count];

        //    //DemoUtil.LinearRank(dataArray, data.Count, data[0].Count);

        //    if (listBoxPoints.Items.Count >= 2)
        //        btnStart.Enabled = true;


        //}

        private void createEntropyFile(List<List<int>> data)
        {
            string path = "EntropyTimeSeries.in";

            if (File.Exists(path))
                File.Delete(path);

            using (StreamWriter sw = File.CreateText(path))
            {
                for (var i = 0; i < data.Count; i++)
                {
                    for (var j = 0; j < data[i].Count; j++)
                    {
                        sw.Write("    " + data[i][j].ToString());
                    }
                }
            }
        }

        private void listBoxPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxPoints.SelectedIndex == -1)
                return;

            var center = (PointLatLng) listBoxPoints.Items[listBoxPoints.SelectedIndex];
            gmap1.Position = center;
        }

        private void btnCenterMap2_Click(object sender, EventArgs e)
        {
            centerMap2();
        }

        private void listBoxLinear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLinear.SelectedIndex == -1)
                return;

            var center = (PointLatLng)listBoxLinear.Items[listBoxLinear.SelectedIndex];
            gmap2.Position = center;
        }

        private void btnClear1_Click(object sender, EventArgs e)
        {
            removeAllMap1Points();
        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            removeAllMap2Points();
        }

        private void removeAllMap1Points()
        {
            listBoxPoints.DataSource = null;

            if (gmap1.Overlays.FirstOrDefault() != null)
            {
                gmap1.Overlays.Remove(gmap1.Overlays.FirstOrDefault());
                gmap1.Refresh();
            }
        }

        private void removeAllMap2Points()
        {
            listBoxLinear.DataSource = null;

            if (gmap2.Overlays.FirstOrDefault() != null)
            {
                gmap2.Overlays.Remove(gmap2.Overlays.FirstOrDefault());
                gmap2.Refresh();
            }
        }

        private void btnLoadDemoOutput_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "Output File(.out)|*.out|All Files (*.*)|*.*";
            fbd.FilterIndex = 1;
            DialogResult result = fbd.ShowDialog();

            if (fbd.FileName != string.Empty)
            {
                outPath = fbd.FileName;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("DEMO");
            foreach (Process p in localByName)
            {
                p.Kill();
            }

            running = false;
        }  

        private void assignMapWeights(List<List<double>> weights)
        {
            double[] weightSums = new Double[weights[0].Count()];
  
            foreach (var subList in weights)
            {
                var i = 0;

                foreach (var value in subList)
                {
                    weightSums[i] += value;
                    i++;
                }
            }

            for (var i = 0; i < weightSums.Count(); i++)
            {
                weightSums[i] = weightSums[i] / weights.Count();
            }
           
            var points = new List<PointLatLng>();

            if (gmap1.Overlays.FirstOrDefault() != null)
            {
                foreach (var markers in gmap1.Overlays.FirstOrDefault().Markers)
                {

                    foreach (var marker in markers.Overlay.Markers)
                    {
                        points.Add(marker.Position);
                    }
                }
            }
            

            var pointsArray = points.ToArray();
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMapOverlay polygons = new GMapOverlay("polygons");
            gmap1.Overlays.Remove(gmap1.Overlays.FirstOrDefault());

            for (var i = 0; i < weightSums.Count(); i++)
            {
                var colour = GMarkerGoogleType.green;
                var penColour = Pens.Green;

                if (weightSums[i] >= 0.9)
                {
                    colour = GMarkerGoogleType.red;
                    penColour = Pens.Red;
                }
                else if(weightSums[i] >= 0.5)
                {
                    colour = GMarkerGoogleType.yellow;
                    penColour = Pens.Yellow;
                }
                else if (weightSums[i] >= 0.2)
                {
                    colour = GMarkerGoogleType.blue;
                    penColour = Pens.Blue;
                }

                //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(pointsArray[i].Lat, pointsArray[i].Lng), colour);
                //markersOverlay.Markers.Add(marker);
                markersOverlay.Markers.Add(new GMapPoint(new PointLatLng(pointsArray[i].Lat, pointsArray[i].Lng), 25, penColour));
            }


            gmap1.Overlays.Add(polygons);
            gmap1.Overlays.Add(markersOverlay);

        }

        private void btnLoadDemoOutput_Click_2(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "Output file(.out)|*.out|All Files (*.*)|*.*";
            fbd.FilterIndex = 1;
            DialogResult result = fbd.ShowDialog();

            if (fbd.FileName != string.Empty)
            {
                var weightedPoints = new List<List<double>>();

                DemoUtil.ImportDataFromCS(fbd.FileName, weightedPoints, numSelectedPoints);

                if (weightedPoints.Count() > 0)
                {
                    assignMapWeights(weightedPoints);
                }

                var a = new List<double>();
                var b = new List<double>();
                var c = new List<double>();


                DemoUtil.ImportGraphDataFromCS(fbd.FileName, a, b, c);

                var x = a.ToArray();
                var y = b.ToArray();
                var z = c.ToArray();

                for (var i = 0; i < a.Count(); i++)
                {
                    outputChart.Series["OutputXY"].Points.AddXY(x[i], y[i]);
                    outputChart.Series["OutputXZ"].Points.AddXY(x[i], z[i]);
                    outputChart.Series["OutputYZ"].Points.AddXY(y[i], z[i]);
                }

                tabControl2.SelectedIndex = 2;
                centerMap1();
            }
        }
    }        
}
