using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGP
{
    class PrivateKey
    {
        int n;
        int phi;
        int d;
        int e;

        public PrivateKey(int phi, int e,int n)
        {
            this.n = n;
            this.phi = phi;
            this.e = e;
            d = MultiplicativeInverse(e, phi);
        }

        public PrivateKey(int n, int d)
        {
            this.n = n;
            this.d = d;
        }

        public static int MultiplicativeInverse(int e, int fi)
        {
            double result;
            int k = 1;
            while (true)
            {
                result = (1 + (k * fi)) / (double)e;
                if ((Math.Round(result, 5) % 1) == 0) //integer
                {
                    return (int)result;
                }
                else
                {
                    k++;
                }
            }
        }

        public int Phi { get => phi; set => phi = value; }
        public int E { get => e; set => e = value; }
        public int D { get => d; set => d = value; }
        public int N { get => n; set => n = value; }
    }
}
