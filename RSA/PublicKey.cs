using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class PublicKey
    {
        int p;
        int q;
        int n;
        int e;
        int phi;

        public PublicKey(int p, int q)
        {
            this.p = p;
            this.q = q;
            n = p * q;
            exponente();
        }

        public int P { get => p; set => p = value; }
        public int Q { get => q; set => q = value; }
        public int N { get => n; set => n = value; }
        public int E { get => e; set => e = value; }
        public int Phi { get => phi; set => phi = value; }

        private void exponente()
        {
            e = 2;
            Phi = (p - 1) * (q - 1);
            while (e < Phi)
            {
                if (gcd(e, Phi) == 1)
                {
                    break;
                }
                else
                {
                    e++;
                }
            }
        }

        private int gcd(int a, int b)
        {
            int aux;
            while (true)
            {
                aux = a % b;
                if (aux == 0)
                {
                    return b;
                }
                a = b;
                b = aux;
            }
        }
    }
}
