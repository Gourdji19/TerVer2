using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
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
            dataGridView1.ColumnCount = Convert.ToInt32(textBox2.Text);
            elem = new GenValues((float)Convert.ToDouble(textBox1.Text), Convert.ToInt32(textBox2.Text));
            for (int i = 0; i < Convert.ToInt32(textBox2.Text); i++)
                elem.GenVal(elem.GetRandomValue());
            elem.GetVal();

            for (int i = 0; i < Convert.ToInt32(textBox2.Text); i++)
            {  
                dataGridView1.Rows[0].Cells[i].Value = "x" + (i + 1);
                dataGridView1.Rows[1].Cells[i].Value =  elem.val[i].ToString();
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

            //--------------------------------------------------------------------------------------------------
            //выборочная функция распределения
            chart2.Series[0].Points.Clear();
            chart2.Series[0].LegendText = "выборочная";
            chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            double Xmin = 0;
            double Xmax = double.Parse(textBox2.Text);
            double step = 1;
            int count = (int)Math.Ceiling((Xmax - Xmin) / step);
            double[] x1 = new double[count + 1];
            double[] k1 = new double[count + 1];

            x1[0] = Xmin;
            k1[0] = 0;
            x1[count] = Xmin + step * (count);
            k1[count] = 1;
            for (int i = 1; i < count; i++)
            {
                x1[i] = Xmin + step * i;
                k1[i] = elem.functionDisribution2(elem.val[i], (float)Convert.ToDouble(textBox1.Text));
            }

            chart2.ChartAreas[0].AxisX.Minimum = Xmin;
            chart2.ChartAreas[0].AxisX.Maximum = Xmax;
            chart2.ChartAreas[0].AxisX.MajorGrid.Interval = step;
            chart2.Series[0].Points.DataBindXY(x1, k1);

            //--------------------------------------------------------------------------------------------------
            //теоретическая функция распределения
            chart2.Series.Add("теоретическая");
            chart2.Series["теоретическая"].Points.Clear();
            chart2.Series["теоретическая"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            Xmin = 0;
            Xmax = double.Parse(textBox2.Text);
            step = 1;
            double[] x2 = new double[count + 1];
            double[] k2 = new double[count + 1];

            x2[0] = Xmin;
            x2[count] = Xmin + step * (count);
            k2[0] = 0;
            k2[count] = 1;
            for (int i = 1; i < count; i++)
            {
                x2[i] = Xmin + step * i;
                k2[i] = elem.functionDisribution2((float)(i * Xmax), (float)Convert.ToDouble(textBox1.Text));
            }
            chart2.Series["теоретическая"].Points.DataBindXY(x2, k2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int count = Int32.Parse(textBox2.Text);
            double step = Math.Round(Math.Round(elem.val[count - 1], 3) / Int32.Parse(textBox3.Text), 3); 
            chart1.Series[0].LegendText = "Элементов на интервале"; 
            chart1.Series[0].Points.Clear(); 
            int []gist = new int[Int32.Parse(textBox3.Text) + 1]; 
            Array.Sort(elem.val); 
            int k = 0; 
            for (int i = 0; i < count; i++) 
            { 
                if ((elem.val[i] > k * step) && (elem.val[i] < (k + 1) * step)) 
                { 
                    gist[k]++; 
                } 
                else 
                { 
                    k++; 
                    i--; 
                }
            } 

            for (int j = 0; j < Int32.Parse(textBox3.Text); j++) 
            { 
                chart1.Series[0].Points.Add(gist[j]); 
            }

            chart1.DataBind();    
 
            //таблица результатов
            //значения
            dataGridView3.RowCount = 3;
            dataGridView3.ColumnCount = Convert.ToInt32(textBox3.Text);
            for(int i = 0; i < Convert.ToInt32(textBox3.Text); i++)
            {
                double value1 = Math.Round(elem.GetMinPeriod() + step * (i + (1/2)), 3);
                double value2 = Math.Round(elem.functionDistributionDensity2(value1, Convert.ToDouble(textBox1.Text)), 3);
                double value3 = Math.Round((gist[i] / (Convert.ToInt32(textBox3.Text) * step)), 3);
                dataGridView3.Rows[0].Cells[i].Value = value1.ToString();
                dataGridView3.Rows[1].Cells[i].Value = value2.ToString();
                dataGridView3.Rows[2].Cells[i].Value = value3.ToString();
            }
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

        private void chart2_Click(object sender, EventArgs e)
        {

        }

    }
}
