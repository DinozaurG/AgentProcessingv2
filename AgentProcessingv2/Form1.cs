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
        double cD, aD, T;
        double[] frec;
        int aV, nextCustomerTime, queue, N;
        Agent[] agents;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            queue = 0;
            T = 0;
            N = 0;
            cD = (double)numericUpDown1.Value;
            aD = (double)numericUpDown2.Value;
            aV = (int)numericUpDown3.Value;
            agents = new Agent[aV];
            for (int i = 0; i < agents.Length; i++)
            {
                agents[i] = new Agent();
            }
            frec = new double[aV + 1];
            double ro = cD / aD;
            double[] probs = new double[aV + 1];
            for (int k = 0; k <= aV; k++)
            {
                probs[0] += Math.Pow(ro, k) / fact(k);
            }
            probs[0] += Math.Pow(ro, aV + 1) / fact(aV) / (aV - ro);
            probs[0] = Math.Pow(probs[0], -1);
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
            foreach (Agent agent in agents)
            {
                if (!agent.isBusy)
                {
                    agent.isBusy = true;
                    agent.workTime = (int)(aD * Math.Exp(-aD * rnd.NextDouble()));
                }
            }
            queue++;
            double[] workTimesOfAgents = new double[aV];
            for (int i = 0; i < aV; i++)
            {
                workTimesOfAgents[i] = agents[i].workTime;
            }
            double minT = double.MaxValue;
            int ind = -1;
            for (int i = 0; i < aV; i++)
            {
                if (workTimesOfAgents[i] < minT)
                {
                    minT = workTimesOfAgents[i];
                    ind = i;
                }
            }
            if (nextCustomerTime < minT)
            {
                minT = nextCustomerTime;
            }
            else
            {
                if (agents[ind].onWork(queue, aD, rnd))
                {
                    queue--;
                }
            }
            T += minT;
            foreach (Agent agent in agents)
            {
                agent.workTime -= minT;
                if (agent.workTime == 0)
                {
                    agent.isBusy = false;
                }
            }
            listBox1.Items.Clear();
            for (int i = 0; i < aV; i++)
            {
                if (agents[i].isBusy)
                {
                    listBox1.Items.Add(i + " " + ("Busy") + " Time left: " + agents[i].workTime);
                }
                else
                {
                    listBox1.Items.Add(i + " " + ("Free") + " Time left: " + agents[i].workTime);
                }
            }
            double[] probs = new double[aV + 1];
            var c = agents.Where(agent => agent.isBusy == true).Count();
            frec[c]++;
            N++;
            for (int i = 0; i <= aV; i++)
            {
                probs[i] = frec[i] / N;
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
