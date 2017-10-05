using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower().Equals("help"))
            {
                Console.WriteLine("Hola");
                Console.WriteLine("Uso: .\\RSA.exe [-d] [-c] [-f <archivo>]");
                Console.WriteLine("");
                Console.WriteLine("\t-d\tDescifra un archivo");
                Console.WriteLine("\t-c\tCifra un archivo");
                Console.WriteLine("\t-f\tEspecifica la ruta y nombre del archivo");
            }
            else if (args.Length == 3)
            {
                switch (args[0].ToLower())
                {
                    case "-d":
                        if (args[1].ToLower().Equals("-f"))
                        {

                        }
                        else
                        {
                            Console.WriteLine("Por favor ingrese una opción correcta. Consulte la opción 'help' para ayuda");
                        }
                        break;
                    case "-c":
                        if (args[1].ToLower().Equals("-f"))
                        {

                        }
                        else
                        {
                            Console.WriteLine("Por favor ingrese una opción correcta. Consulte la opción 'help' para ayuda");
                        }
                        break;
                    default:
                        Console.WriteLine("Por favor ingrese una opción correcta. Consulte la opción 'help' para ayuda");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Por favor ingrese una opción correcta. Consulte la opción 'help' para ayuda");
            }
            Console.ReadLine();
        }

        static int generatePrime()
        {
            Random rnd = new Random();
            int number = rnd.Next(1000000, 10000000);
            while (!isPrime(number))
            {
                number = number+1;
            } 
            return number;
        }

        static bool isPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }
    }
}
