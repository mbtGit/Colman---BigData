using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BigData
{
    public partial class ClustersView : Form
    {
        Dictionary<string, DataTable> AllSymbols;
        List<int> DataIndexes;
        int NumOfDays; 
       
        public ClustersView(Dictionary<string, DataTable> lstAllSymbols, List<int> lstDataIndexes, int nNumOfDays, string[] strClustersData, bool bCandleStickCharts)
        {
            InitializeComponent();
            this.NumOfDays = nNumOfDays;
            this.DataIndexes = lstDataIndexes;
            this.AllSymbols = lstAllSymbols;
            int nClustersCount = 1;
            int nNumOfStocks;
            strClustersData[0] = strClustersData[0].Remove(0, 2);

            // Run over all clusters
            foreach (string strCurrCluster in strClustersData)
            {
                TabPage tbNewTabCluster = new TabPage("קבוצה " +  nClustersCount);
                tbNewTabCluster.AutoScroll = true;
                tbNewTabCluster.AccessibleRole = AccessibleRole.ScrollBar;
                tabContainer.Controls.Add(tbNewTabCluster);
                tbNewTabCluster.Show();
                nNumOfStocks = 0;

                // Run over each stock
                foreach (string strCurrSymbol in strCurrCluster.Split(','))
                {
                    Chart chart = new Chart();
                    chart.Name = strCurrSymbol;
                    chart.Titles.Add(strCurrSymbol);
                    tbNewTabCluster.Controls.Add(chart);
                    chart.Width = this.tabContainer.Width;
                    chart.Height = 100;
                    chart.Top = 100 * nNumOfStocks; ;

                    // Make Chart for the current stock
                    this.MakeChart(chart, AllSymbols[strCurrSymbol], bCandleStickCharts);

                    chart.Show();
                    nNumOfStocks++;
                }

                nClustersCount++;
            }
        }

        public void MakeChart(Chart c, DataTable dtData, bool bCandleStickCharts)
        {
            c.ChartAreas.Add("1");
            c.Series.Add("1");
            c.Series["1"].XValueType = ChartValueType.Int32;
            c.Series["1"].ChartType = SeriesChartType.Line;
            c.Series["1"].BorderWidth = 3;
            c.Series["1"].BorderColor = Color.FromName("Orange");
            c.ChartAreas["1"].AxisY.Minimum = double.MaxValue;
            c.ChartAreas["1"].AxisY.Enabled = AxisEnabled.False;

            // CandleSticks Setup
            if ((bCandleStickCharts) && (DataIndexes.Count == 4))
            {
                c.Series["1"].ChartType = SeriesChartType.Candlestick;
                c.Series["1"].BorderWidth = 1;
                c.Series["1"]["OpenCloseStyle"] = "Triangle";
                c.Series["1"]["ShowOpenClose"] = "Both";
                c.Series["1"]["PointWidth"] = "0.8";
                c.Series["1"]["PriceUpColor"] = "Gray"; // up
                c.Series["1"]["PriceDownColor"] = "Pink"; // down
            }

            double dValue;
            double dTemp;
            int nCandleSticksIndex = 0;

            // Run over stock days
            for (int nDaysIndex = 1; nDaysIndex <= this.NumOfDays; nDaysIndex++)
            {
                dValue = 0;
                
                // Sample each price and make an avarage
                foreach (int nCurrColIndex in DataIndexes)
                {
                    dTemp = (double)dtData.Rows[nDaysIndex][nCurrColIndex];
                    
                    // If the configuration is for CandleSticks
                    if ((bCandleStickCharts) && (DataIndexes.Count == 4))
                    {
                        if (c.Series["1"].Points.Count == nCandleSticksIndex) c.Series["1"].Points.AddXY(nCandleSticksIndex, 0);
                        // adding date and high
                        if (nCurrColIndex == 2) c.Series["1"].Points[nCandleSticksIndex].YValues[0] = dTemp;
                        // adding low
                        else if (nCurrColIndex == 3) c.Series["1"].Points[nCandleSticksIndex].YValues[1] = dTemp;
                        // adding open
                        else if (nCurrColIndex == 1) c.Series["1"].Points[nCandleSticksIndex].YValues[2] = dTemp;
                        // adding close
                        else if (nCurrColIndex == 4) c.Series["1"].Points[nCandleSticksIndex].YValues[3] = dTemp;
                        // set the miinmum for the chart
                        if (c.ChartAreas["1"].AxisY.Minimum > dTemp)
                            c.ChartAreas["1"].AxisY.Minimum = dTemp;
                    }

                    dValue += dTemp;
                }

                nCandleSticksIndex++; 

                // If the configuration is not as CandleSticks
                if ((!bCandleStickCharts) || (DataIndexes.Count != 4))
                {
                    dValue /= DataIndexes.Count;
                    c.Series["1"].Points.AddXY(nDaysIndex, dValue);
                    if (c.ChartAreas["1"].AxisY.Minimum > dValue)
                        c.ChartAreas["1"].AxisY.Minimum = dValue;
                }
            }
        }
    }
}
