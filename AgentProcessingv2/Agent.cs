using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentProcessingv2
{
    public class Agent
    {
        public double workTime = 0;
        public bool isBusy = false;
        public bool onWork(int queue, double aD, Random rnd)
        {
            isBusy = false;
            if (queue > 0)
            {
                isBusy = true;
                workTime = (int)(aD * Math.Exp(-aD * rnd.NextDouble()));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
