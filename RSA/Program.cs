using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] {"-c","-f","prueba.txt" };
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
                            cifrar(args[2]);
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

        static void cifrar(string file)
        {
            Random rnd = new Random();
            int primo1 = generatePrime(rnd.Next(100, 1000));
            int primo2 = generatePrime(rnd.Next(100, 1000));
            while (primo1 == primo2)
            {
                primo2 = generatePrime(rnd.Next(10, 100));
            }
            PublicKey publicKey = new PublicKey(primo1,primo2);
            PrivateKey privateKey = new PrivateKey(publicKey.Phi,publicKey.E);
            byte[] text = File.ReadAllBytes(file);
            string message = "";
            foreach(byte b in text)
            {
                message = message + b.ToString().PadLeft(3, '0');
            }
            message = "89";
            Console.WriteLine(message);
            BigInteger encrypt = BigInteger.Parse(message);
            encrypt = BigInteger.Pow(encrypt, publicKey.E);
            encrypt = encrypt % publicKey.N;
            Console.WriteLine(encrypt);
            
            BigInteger decrypt = BigInteger.Pow(encrypt, (int)privateKey.D);
            decrypt = decrypt % publicKey.N;
            Console.WriteLine(decrypt);
            Console.ReadLine();
        }

        static int generatePrime(int number)
        {
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
