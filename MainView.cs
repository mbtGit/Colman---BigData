using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

namespace BigData
{
    public partial class MainView : Form
    {
        public List<String> Symbols;
        private int CompletedFiles = 0;
        private Object lockThis = new Object();
        private UserInfo INFO;
        private bool IsCalcFinished = false;
        private int nNumOfFeatures;
        private List<int> DataIndexes = new List<int>();
        private Dictionary<string, DataTable> StocksDictionary;
        public MainView()
        {
            InitializeComponent();
            this.Symbols = this.DoanloadSymbolsNames();
            this.StocksDictionary = new Dictionary<string, DataTable>();
            this.INFO = new UserInfo();
        }

        private List<string> DoanloadSymbolsNames()
        {
            List<string> strToReturn = new List<string>();

            // Send the request Asynchronously
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.nasdaqtrader.com/symboldirectory/nasdaqlisted.txt");

            using (WebResponse response = request.GetResponse())
            {
                // Read response xml
                using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                {
                    string[] strAllSymbols = responseReader.ReadToEnd().Split('\n');
                    for (int nSymbolIndex = 1; nSymbolIndex < strAllSymbols.Length; nSymbolIndex++)
                    {
                        strToReturn.Add(strAllSymbols[nSymbolIndex].Split('|')[0]);
                    }
                }
            }

            return (strToReturn);
        }
        private void Action(object sender, EventArgs e)
        {
            // Change window height and status pannel
            this.pic2.ImageLocation = "Icons\\Dis_connect.png";
            this.pic3.ImageLocation = "Icons\\Dis_brain.png";
            this.pic4.ImageLocation = "Icons\\Dis_graph.png";
            this.pic5.ImageLocation = "Icons\\loading.gif";
            this.IsCalcFinished = false;
            this.Height = 225;

            // Creating the Data folder
            if (Directory.Exists("Data"))
                Directory.Delete("Data", true);
            Directory.CreateDirectory("Data");

            // Download stocks files
            Task.Factory
                .StartNew(DownloadStocksData)
                .ContinueWith((antecedent) => ProcessStocks())
                .ContinueWith((antecedent) => SendDataToServer(antecedent))
                .ContinueWith((antecedent) => Compile())
                .ContinueWith((antecedent) => GetDataFromServer())
                .ContinueWith((antecedent) => Finish());
        }

        private void    DownloadStocksData()
        {
            Parallel.For(0, (int)nudStocksCount.Value, nSymbolIndex => 
            {
                string strSymbol = this.Symbols[nSymbolIndex];

                try
                {
                    // Download the stock file
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(string.Format("http://ichart.yahoo.com/table.csv?s={0}", strSymbol), 
                                            string.Format("Data\\{0}.csv", strSymbol));
                    }
                }
                catch
                {
                    MessageBox.Show(string.Format("גיבור, נתוני המניה {0} לא זמינים... נסה שוב מאוחר יותר", strSymbol));
                }
                finally
                {
                    lock (this.lockThis)
                    {
                        this.CompletedFiles++;
                    }
                }
            });

            // Wait till all files finish download
            while (this.nudStocksCount.Value != CompletedFiles) { Thread.Sleep(150); }
        }
        private void    NormalizeTable(DataTable dtTable, int nNumOfRows, List<int> lstColsIndexes, int nMinRange, int nMaxRange)
        {
            Dictionary<int, double> dicMin = new Dictionary<int, double>();
            Dictionary<int, double> dicMax = new Dictionary<int, double>();
            double dCurr;

            // Run over each col
            foreach (int nColIndex in lstColsIndexes)
            {
                dicMin.Add(nColIndex, double.MaxValue);
                dicMax.Add(nColIndex, double.MinValue);

                // Run over each row
                for (int nRowIndex = 1; nRowIndex < nNumOfRows; nRowIndex++)
                {
                    dCurr = (double)dtTable.Rows[nRowIndex][nColIndex];

                    // Find min\max
                    if (dicMax[nColIndex] < dCurr) { dicMax[nColIndex] = dCurr; }
                    if (dicMin[nColIndex] > dCurr) { dicMin[nColIndex] = dCurr; }
                }   
            }


            //Parallel.For(0, lstColsIndexes.Count, nCol => 
            for (int nCol = 0; nCol < lstColsIndexes.Count; nCol++)
            {
                //Parallel.For(1, nNumOfRows, nRowIndex =>
                for (int nRowIndex = 1; nRowIndex < nNumOfRows; nRowIndex++)
                {
                    int nColIndex = lstColsIndexes[nCol];
                    dtTable.Rows[nRowIndex][nColIndex] = (((double)dtTable.Rows[nRowIndex][nColIndex] - dicMin[nColIndex]) * (nMaxRange - nMinRange)) / (dicMax[nColIndex] - dicMin[nColIndex]);
                }//);
            }//;//);
        }
        private List<string>  ProcessStocks()
        {
            this.pic2.ImageLocation = "Icons\\connect.png";

            if (cbOpen.Checked)  DataIndexes.Add(1);
            if (cbHigh.Checked)  DataIndexes.Add(2);
            if (cbLow.Checked)   DataIndexes.Add(3);
            if (cbClose.Checked) DataIndexes.Add(4);
            int nNumOfDays = (int)this.nudDays.Value + 1;
            nNumOfFeatures = DataIndexes.Count;

            // Creat single data string for the Cludera
            StringBuilder sbToReturn = new StringBuilder();
            DataTable dtCurrStock = null;
            List<string> lstToReturn = new List<string>();
            int nStocksCounter = 10; 

            // Run over the stocks
            foreach (string strCurrSymbol in this.Symbols.GetRange(0, (int)this.nudStocksCount.Value))
            {
                dtCurrStock = Excel.LoadTable("Data", string.Format("{0}.csv", strCurrSymbol));

                if (dtCurrStock != null)
                {
                    sbToReturn.Append(string.Format("{0} ", strCurrSymbol));

                    // Normlize data
                    this.NormalizeTable(dtCurrStock, nNumOfDays, DataIndexes, 0, 1);

                    // Run over the rows
                    for (int nRowIndex = 1; nRowIndex < nNumOfDays; nRowIndex++)
                    {
                        // Run over the cols by the user selection
                        foreach (int nDataIndex in DataIndexes)
                        {
                            sbToReturn.Append(string.Format("{0},", dtCurrStock.Rows[nRowIndex][nDataIndex]));
                        }
                    }

                    sbToReturn.Remove(sbToReturn.Length - 1, 1);
                    sbToReturn.Append('\n');
                }

                nStocksCounter--;
                if (nStocksCounter == 0)
                {
                    lstToReturn.Add(sbToReturn.ToString());
                    nStocksCounter = 10;
                    sbToReturn.Clear();
                }
            }

            return (lstToReturn);
        }
        private void    PrepareServer()
        {
            // Prepare the server for the run, Delete folders and recreate them
            string[] initVMCommands = new string[]
                    {
                        "rm -r stocksProj",
                        "mkdir stocksProj",
                        "mkdir stocksProj/Stocks",
                        "mkdir stocksProj/Java",
                        "mkdir stocksProj/Java/Classes"
                    };
            
            // Set the info for the server approach
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            INFO.Host = ConfigurationManager.AppSettings["HostIP"];
            INFO.Pass = ConfigurationManager.AppSettings["HostPass"];
            INFO.User = ConfigurationManager.AppSettings["HustUserName"];

            // Run the commands
            SSH.RunCommands(INFO, initVMCommands);
        }
        private void    SendDataToServer(Task<List<string>> prev)
        {
            List<string> lstDataToSend = prev.Result;
            if (Directory.Exists("DataToSend")) { Directory.Delete("DataToSend", true); }
            Directory.CreateDirectory("DataToSend");
            int nIndex = 0;
            foreach (string strDataToSend in lstDataToSend)
            {
                File.WriteAllText(string.Format("DataToSend\\data{0}.csv", nIndex++), strDataToSend);
            }

            // Prepare server for first use
            this.PrepareServer();

            // Upload the stocks to the server 
            string[] stocksFiles = Directory.GetFiles("DataToSend");
            SSH.TransferFilesFromWinToLin(INFO, stocksFiles, "stocksProj/Stocks/");


            // Send java files to server
            string[] initJavaFilesCommands = Directory.GetFiles("..\\..\\JavaFiles");
            SSH.TransferFilesFromWinToLin(INFO, initJavaFilesCommands, "stocksProj/Java");
            

            // Hadoop folders delete and recreate
            string[] initHadoopCommands = new string[]
                    {
                       "hadoop fs -rm -r stocksproj",
                       "hadoop fs -mkdir stocksproj/inputfiles",
                       "hadoop fs -put stocksProj/Stocks/* stocksproj/inputfiles"
                    };

            SSH.RunCommands(INFO, initHadoopCommands);
        }  
        private void    Compile()
        {
            this.pic3.ImageLocation = "Icons\\brain.png";

            // Hadoop compile and create jar
            string[] runHadoopCommands = new string[] {
                "javac -cp /usr/lib/hadoop/*:/usr/lib/hadoop/client-0.20/* -d stocksProj/Java/Classes stocksProj/Java/*.java",
                "jar -cvf stocksProj/final.jar -C stocksProj/Java/Classes/ ."
            };

            SSH.RunCommands(INFO, runHadoopCommands);

            // Run finalProj in hadoop
            string runCommand = "hadoop jar stocksProj/final.jar solution.FinalProj stocksproj/inputfiles stocksproj/output.txt "
                                    + int.Parse(this.nudDays.Text) * nNumOfFeatures * 0.4 + " " + this.nudK.Value;

            SSH.RunCommands(INFO, new string[] { runCommand });
        }
        private void    GetDataFromServer()
        {
            this.pic4.ImageLocation = "Icons\\graph.png";

            // get final file from hadoop to vm
            string[] getFileToVmFromHadoop = new string[] { 
                "hadoop fs -get stocksproj/output.txt stocksProj/output.txt" 
            };

            SSH.RunCommands(INFO, getFileToVmFromHadoop);

            // Download the file to local machine
            if (File.Exists("results.txt")) File.Delete("results.txt");
            SSH.TransferLinToWin(INFO, "stocksProj/output.txt", "results.txt");
        }
        private void    Finish()
        {
            DataTable dtCurrStock = null;
            
            // Run over the stocks
            foreach (string strCurrSymbol in this.Symbols.GetRange(0, (int)this.nudStocksCount.Value))
            {
                dtCurrStock = Excel.LoadTable("Data", string.Format("{0}.csv", strCurrSymbol));
                
                if (dtCurrStock != null)
                {
                    StocksDictionary.Add(strCurrSymbol, dtCurrStock);
                }
            }    

            IsCalcFinished = true;
            this.pic5.ImageLocation = "Icons\\results.png";
        }
        private void    OpenResultsWin(object sender, EventArgs e)
        {
            if (this.IsCalcFinished)
            {
                ClustersView cv = new ClustersView(StocksDictionary, DataIndexes, (int)this.nudDays.Value, File.ReadAllLines("results.txt"), false);
                cv.Show();
            }
        }
    }
}
