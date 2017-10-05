using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class PrivateKey
    {
        int phi;
        double d;
        int k;
        int e;

        public PrivateKey(int phi, int e)
        {
            this.Phi = phi;
            this.E = e;
            k = 2;
            D = (1 + (k * phi)) / e;
        }

        public int Phi { get => phi; set => phi = value; }
        public int E { get => e; set => e = value; }
        public int K { get => k; set => k = value; }
        public double D { get => d; set => d = value; }
    }
}
