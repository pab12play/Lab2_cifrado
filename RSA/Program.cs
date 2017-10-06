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
            int primo1 = generatePrime(rnd.Next(10, 100));
            int primo2 = generatePrime(rnd.Next(10, 100));
            while (primo1 == primo2)
            {
                primo2 = generatePrime(rnd.Next(10, 100));
            }
            PublicKey publicKey = new PublicKey(primo1, primo2);
            PrivateKey privateKey = new PrivateKey(publicKey.Phi, publicKey.E, publicKey.N);
            byte[] text = File.ReadAllBytes(file);
            string message = "";
            foreach (byte b in text)
            {
                double letra = Math.Pow(b, publicKey.E) % publicKey.N;
                message = message + letra + (char)3;
            }
            Console.WriteLine(message);
            File.WriteAllText(Path.GetFileNameWithoutExtension(file) + ".cif", message);
            int intValue;
            byte[] intBytes = BitConverter.GetBytes(100000000);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            byte[] result = intBytes;
            File.WriteAllBytes("hola.txt", result);
            decifrar(Path.GetFileNameWithoutExtension(file) + ".cif", privateKey);
            Console.ReadLine();
        }

        static void decifrar(string file,PrivateKey privateKey)
        {
            string text = File.ReadAllText(file);
            string message = "";
            string temp = "";
            foreach (char b in text)
            {
                if (b == (char)3)
                {
                    BigInteger decrypt = BigInteger.Pow(BigInteger.Parse(temp), privateKey.D) % privateKey.N;
                    message = message + (char)decrypt;
                    temp = "";
                }
                else
                {
                    temp = temp + b;
                }
            }
            Console.WriteLine(message);
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
