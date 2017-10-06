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
                            decifrar(args[2]);
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
            List<byte> message = new List<byte>();
            foreach (byte b in text)
            {
                double letra = Math.Pow(b, publicKey.E) % publicKey.N;
                byte[] intBytes = BitConverter.GetBytes((int)letra);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                message.AddRange(intBytes);
            }
            File.WriteAllBytes(Path.GetFileNameWithoutExtension(file) + ".cif", message.ToArray());
            string llavePublica = publicKey.E + "\n" + publicKey.N;
            string llavePrivada = privateKey.D + "\n" + privateKey.N;
            File.WriteAllText("llavePublica.key", llavePublica);
            File.WriteAllText("llavePrivada.key", llavePrivada);
        }

        static void decifrar(string file)
        {
            string[] llavePrivada = File.ReadLines("llavePrivada.Key").ToArray();
            PrivateKey privateKey = new PrivateKey(int.Parse(llavePrivada[1]), int.Parse(llavePrivada[0]));
            byte[] text = File.ReadAllBytes(file);
            string message = "";
            List<byte> temp = new List<byte>();
            int contador = 0;
            foreach (byte b in text)
            {
                if (contador >= 4)
                {
                    BigInteger decrypt = BigInteger.Pow(BitConverter.ToInt32(temp.ToArray(),0), privateKey.D) % privateKey.N;
                    message = message + (char)decrypt;
                    temp.Clear();
                    temp.Add(b);
                    contador = 1;
                }
                else
                {
                    temp.Add(b);
                    contador++;
                }
            }
            BigInteger decrypt1 = BigInteger.Pow(BitConverter.ToInt32(temp.ToArray(), 0), privateKey.D) % privateKey.N;
            message = message + (char)decrypt1;
            File.WriteAllText(Path.GetFileNameWithoutExtension(file) + ".txt", message);
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
