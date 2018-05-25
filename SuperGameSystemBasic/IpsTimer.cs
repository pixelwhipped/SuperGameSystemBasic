using System;
using System.Threading;

namespace SuperGameSystemBasic
{
    public class IpsTimer
    {
        public IpsTimer(BasicOne basicOne, IdeState state)
        {
            BasicOne = basicOne;
            RequiredState = state;
        }

        public int Updates { get; set; }
        public double Ips { get; set; }
        public Timer IpsTpmer { get; set; }

        public IdeState RequiredState { get; set; }
        public BasicOne BasicOne { get; set; }

        private void IpsUpdate(object state)
        {
            if (BasicOne.IdeState == RequiredState)
            {
                Ips = ((Math.Abs(Ips) < 0.001 ? Updates : Ips) + Updates) / 2f;
                Updates = 0;
                IpsTpmer = new Timer(IpsUpdate, null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
            }
            else
            {
                IpsTpmer = null;
            }
        }

        public void Update()
        {
            if (IpsTpmer == null)
                IpsTpmer = new Timer(IpsUpdate, null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
            Updates++;
        }
    }
}