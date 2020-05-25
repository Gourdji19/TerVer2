using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Drawing.Drawing2D;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Modeling
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        GenValues elem;
        int[,] gist1;
        
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 2;
            int cols = 0;
            if (Convert.ToInt32(textBox2.Text) < 500)
                cols = Convert.ToInt32(textBox2.Text);
            else
                cols = 500;
            dataGridView1.ColumnCount = cols;
            elem = new GenValues((float)Convert.ToDouble(textBox1.Text), cols);
            for (int i = 0; i < cols; i++)
                elem.GenVal(elem.GetRandomValue());
            elem.GetVal();
            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = "x" + (i + 1);
                dataGridView1.Rows[1].Cells[i].Value = elem.val[i].ToString();
            }
            //заполнение таблицы числовых характеристик
            //значения
            dataGridView2.RowCount = 2;
            dataGridView2.ColumnCount = 8;
            dataGridView2.Rows[1].Cells[0].Value = elem.MathExpectation().ToString();
            dataGridView2.Rows[1].Cells[1].Value = elem.SampleMean().ToString();
            dataGridView2.Rows[1].Cells[2].Value = Math.Abs(elem.MathExpectation() - elem.SampleMean()).ToString();
            dataGridView2.Rows[1].Cells[3].Value = elem.SampleDispersion().ToString();
            dataGridView2.Rows[1].Cells[4].Value = elem.TheoreticalDispersion().ToString();
            dataGridView2.Rows[1].Cells[5].Value = Math.Abs(elem.SampleDispersion() - elem.TheoreticalDispersion()).ToString();
            dataGridView2.Rows[1].Cells[6].Value = elem.SampleMedian().ToString();
            dataGridView2.Rows[1].Cells[7].Value = elem.SampleScope().ToString();

            string s1 = "Eη",
                   s2 = "x",
                   s3 = "|Eη - x|",
                   s4 = "Dη",
                   s5 = "S2",
                   s6 = "|Dη - S2|",
                   s7 = "Me",
                   s8 = "R";
            dataGridView2.Rows[0].Cells[0].Value = s1;
            dataGridView2.Rows[0].Cells[1].Value = s2;
            dataGridView2.Rows[0].Cells[2].Value = s3;
            dataGridView2.Rows[0].Cells[3].Value = s4;
            dataGridView2.Rows[0].Cells[4].Value = s5;
            dataGridView2.Rows[0].Cells[5].Value = s6;
            dataGridView2.Rows[0].Cells[6].Value = s7;
            dataGridView2.Rows[0].Cells[7].Value = s8;

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(textBox1.Text);
            int N = Convert.ToInt32(textBox3.Text);
            int n = elem.val.Length;
            double[] interval = new double[N + 1];
            double[] value = new double[N];
            double[] f = new double[N];

            //строим функции распределения

            ZedGraph.PointPairList F_list = new ZedGraph.PointPairList();
            ZedGraph.PointPairList Fn_list = new ZedGraph.PointPairList();

            zedGraphControl1.GraphPane.Title.Text = "График функций распределения";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "X";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "F(x)";

            double D = 0.0;
            

            double h = elem.val[n - 1] / 1000.0;
            int sum = 0;
       
            for (int i = 0; i < 1000; i++)
            {
                sum = 0;
                for (int j = 0; j < n; j++)
                {
                    double temp = elem.val[0] + h * i;
                    if (elem.val[j] < elem.val[0] + h * i)
                        sum++;
                }
                Fn_list.Add(elem.val[0] + h * i, (double)sum / (double)n);
                F_list.Add(elem.val[0] + h * i, 1 - Math.Exp(-(elem.val[0] +h * i) * (elem.val[0]+h * i) / (2 * sigma * sigma)));
              
                D = Math.Max(D, Math.Abs((double)sum / (double)n - (1 - Math.Exp(-(elem.val[0] + h * i) * (elem.val[0] + h * i) / (2 * sigma * sigma)))));
            }
            zedGraphControl1.GraphPane.CurveList.Clear();

            textBox4.Text = D.ToString();
            ZedGraph.LineItem CurveF = zedGraphControl1.GraphPane.AddCurve("F", F_list, Color.FromName("Red"), ZedGraph.SymbolType.None);
            ZedGraph.LineItem CurveFn = zedGraphControl1.GraphPane.AddCurve("Fвыб", Fn_list, Color.FromName("Green"), ZedGraph.SymbolType.None);


            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int N = Convert.ToInt32(textBox3.Text);
            double h2 = (elem.val[elem.num - 1] - elem.val[0]) / N;
            double sigma = Convert.ToDouble(textBox1.Text);
            int n = elem.val.Length;
            double[] interval = new double[N + 1];
            double[] value = new double[N];
            double[] f = new double[N];
       

            for (int i = 0; i < N; i++)
            {
                interval[i] = elem.val[0] + (double)i * h2;


            }
            interval[N] = elem.val[n - 1];


            int sum2;
            for (int i = 0; i < N; i++)
            {
                sum2 = 0;
                for (int j = 0; j < n; j++)
                {
                    if ((interval[i] < elem.val[j]) && (elem.val[j] <= interval[i + 1]))
                        sum2++;
                }

                value[i] = (double)sum2 / (h2 * (double)n);
            }

            GraphPane pane1 = zedGraphControl2.GraphPane;
            pane1.CurveList.Clear();

            BarItem curve1 = pane1.AddBar(null, null, value, Color.SlateBlue);
            curve1.Bar.Fill.Type = FillType.Solid; 
            zedGraphControl2.GraphPane.Title.Text = "Гистограмма";

            pane1.YAxis.Scale.Min = 0.0;
            pane1.YAxis.Scale.Max = value.Max() + 0.001;
            pane1.BarSettings.MinClusterGap = 0.0f;
            
            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();

            //3 таблица

            double max = 0.0;
            for (int i = 0; i < N; i++)
            {
                dataGridView3.ColumnCount = N;
                dataGridView3.RowCount = 3;
                dataGridView3.Columns[i].HeaderText = string.Format("z" + (i + 1), i);
                dataGridView3.Rows[0].Cells[i].Value = interval[i] + h2 * 0.5;
                f[i] = ((interval[i] + h2 * 0.5) * Math.Exp(-(interval[i] + h2 * 0.5) * (interval[i] + h2 * 0.5) / (2 * sigma * sigma))) / (sigma * sigma);
                dataGridView3.Rows[1].Cells[i].Value = f[i];
                dataGridView3.Rows[2].Cells[i].Value = value[i];
                if (Math.Abs(value[i] - f[i]) > max)
                    max = Math.Abs(value[i] - f[i]);
            }
            textBox5.Text = max.ToString();
          
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {
             
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void zedGraphControl1_Load_1(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

    }
}
