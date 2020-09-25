using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentProcessingv2
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        double cD, aD;
        double[] freq;
        int aV, nextCustomerTime, queue, N;
        Agent[] agents;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            queue = 0;
            N = 0;
            cD = (double)numericUpDown1.Value;
            aD = (double)numericUpDown2.Value;
            aV = (int)numericUpDown3.Value;
            agents = new Agent[aV];
            for (int i = 0; i < agents.Length; i++)
            {
                agents[i] = new Agent();
            }
            freq = new double[aV + 1];
            double ro = cD / aD;
            double[] probs = new double[aV + 1];
            for (int k = 0; k <= aV; k++)
            {
                probs[0] += Math.Pow(ro, k) / fact(k);
            }
            probs[0] += Math.Pow(ro, aV + 1) / (fact(aV) * (aV - ro));
            probs[0] = 1 / probs[0];
            for (int k = 0; k <= aV; k++)
            {
                probs[k] = Math.Pow(ro, k) / fact(k) * probs[0];
            }

            chart1.Series[0].Points.Clear();
            for (int i = 0; i < probs.Length; i++)
            {
                chart1.Series[0].Points.AddXY(i, probs[i]);
            }
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            nextCustomerTime += (int)(cD * Math.Exp(-cD * rnd.NextDouble()));
            for (int i = 0; i < aV; i++)
            {
                if (!agents[i].busy)
                {
                    agents[i].busy = true;
                    agents[i].busyTime = (int)(aD * Math.Exp(-aD * rnd.NextDouble()));
                    getDataToScreen();
                    return;
                }
            }
            queue++;
            double minT = double.MaxValue;
            int ind = -1;
            for (int i = 0; i < aV; i++)
            {
                if (agents[i].busyTime < minT)
                {
                    minT = agents[i].busyTime;
                    ind = i;
                }
            }
            if (nextCustomerTime < minT)
            {
                minT = nextCustomerTime;
            }
            else
            {
                agents[ind].busy = false;
                if (queue > 0)
                {
                    agents[ind].busy = true;
                    agents[ind].busyTime = (int)(aD * Math.Exp(-aD * rnd.NextDouble()));
                    queue--;
                }
            }
            for (int i = 0; i < aV; i++)
            {
                agents[i].busyTime -= minT;
                if (agents[i].busyTime <= 0)
                {
                    agents[i].busy = false;
                }
            }
            getDataToScreen();
        }
        private void getDataToScreen() 
        {
            listBox1.Items.Clear();
            for (int i = 0; i < aV; i++)
            {
                if (agents[i].busy)
                {
                    listBox1.Items.Add((i + 1) + " " + ("Busy") + " Time left: " + agents[i].busyTime);
                }
                else
                {
                    listBox1.Items.Add((i + 1) + " " + ("Free") + " Time left: " + agents[i].busyTime);
                }
            }
            double[] probs = new double[aV + 1];
            int a = 0;
            for (int i = 0; i < agents.Count(); i++)
            {
                if (agents[i].busy)
                    a++;
            }
            freq[a]++;
            N++;
            for (int i = 0; i <= aV; i++)
            {
                probs[i] = freq[i] / N;
            }
            chart1.Series[1].Points.Clear();
            for (int i = 0; i < probs.Length; i++)
            {
                chart1.Series[1].Points.AddXY(i, probs[i]);
            }
        }
        public int fact(int n)
        {
            if (n == 1 || n == 0)
            {
                return 1;
            }
            else
            {
                return n * fact(n - 1);
            }
        }
    }
}
