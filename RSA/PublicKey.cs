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

        public PublicKey(int p, int q)
        {
            this.p = p;
            this.q = q;
        }

        public int P { get => p; set => p = value; }
        public int Q { get => q; set => q = value; }
        public int N { get => n; set => n = value; }
        public int E { get => e; set => e = value; }
    }
}
