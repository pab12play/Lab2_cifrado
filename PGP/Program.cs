using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGP
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower().Equals("help"))
            {
                Console.WriteLine("Hola");
                Console.WriteLine("Uso: .\\PGP.exe [-d] [-c] [-f <archivo>]");
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
                            string text = File.ReadAllText(args[2]);
                            string nuevaLlave = RandomString(50);
                            string textoEncriptado = encriptarAES(text,nuevaLlave,128);
                            string llaveEncriptada = encriptarRSA(Encoding.ASCII.GetBytes(nuevaLlave));
                            File.WriteAllText(Path.GetFileNameWithoutExtension(args[2]) + ".cif", llaveEncriptada+"\n"+textoEncriptado);
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

        static string encriptarAES(string plaintext, string password, int nBits)
        {
            const int blockSize = 16;
            if (!(nBits == 128 || nBits == 192 || nBits == 256))
            {
                Console.WriteLine("Key size is not 128 / 192 / 256");
            }
            int nBytes = nBits / 8;
            byte[] pwBytes = new byte[nBytes];
            for (int i = 0; i < nBytes; i++)
            {
                pwBytes[i] = i < password.Length ? Convert.ToByte(password[i]) : (byte)0;
            }

            Aes aes = new Aes();
            byte[] key = aes.cipher(pwBytes, aes.keyExpansion(pwBytes));
            key = key.Concat(key.Slice(0, nBytes - 16)).ToArray(); 

            
            byte[] counterBlock = new byte[(blockSize)];

            uint nonce = 0, nonceMs = 0, nonceSec = 0, nonceRnd = 0;

            for (int i = 0; i < 2; i++) counterBlock[i] = Convert.ToByte((nonceMs >> i * 8) & 0xff);
            for (int i = 0; i < 2; i++) counterBlock[i + 2] = Convert.ToByte((nonceRnd >> i * 8) & 0xff);
            for (int i = 0; i < 4; i++) counterBlock[i + 4] = Convert.ToByte((nonceSec >> i * 8) & 0xff);


            string ctrTxt = "";
            for (int i = 0; i < 8; i++) ctrTxt += (char)counterBlock[i];

            byte[,] keySchedule = aes.keyExpansion(key);

            double blockCount = Math.Ceiling(((double)plaintext.Length / blockSize));
            string ciphertext = "";

            for (int b = 0; b < blockCount; b++)
            {
                for (int c = 0; c < 4; c++) counterBlock[15 - c] = Convert.ToByte((uint)(b >> c * 8) & 0xff);
                for (int c = 0; c < 4; c++) counterBlock[15 - c - 4] = Convert.ToByte((uint)(b / 0x100000000 >> c * 8));

                byte[] cipherCntr = aes.cipher(counterBlock, keySchedule); 
                
                int blockLength = b < blockCount - 1 ? blockSize : (plaintext.Length - 1) % blockSize + 1;
                char[] cipherChar = new char[(blockLength)];

                for (int i = 0; i < blockLength; i++)
                {
                    cipherChar[i] = Convert.ToChar(cipherCntr[i] ^ plaintext[(b * blockSize + i)]);
                    cipherChar[i] = (cipherChar[i]);
                }
                ciphertext = ciphertext + string.Join("", cipherChar);
            }
            ciphertext = Base64Encode(ctrTxt + ciphertext);


            return ciphertext;
        }


        static string desencriptarAES(string ciphertext, string password, int nBits)
        {
            const int blockSize = 16;
            if (!(nBits == 128 || nBits == 192 || nBits == 256))
            {
                Console.WriteLine("Key size is not 128 / 192 / 256");
            }
            ciphertext = Base64Decode(ciphertext);
            int nBytes = nBits / 8; 
            byte[] pwBytes = new byte[(nBytes)];
            for (int i = 0; i < nBytes; i++)
            { 
                pwBytes[i] = i < password.Length ? Convert.ToByte(password[(i)]) : Convert.ToByte(0);
            }

            Aes aes = new Aes();
            byte[] key = aes.cipher(pwBytes, aes.keyExpansion(pwBytes));
            key = key.Concat(key.Slice(0, nBytes - 16)).ToArray(); 
            
            byte[] counterBlock = new byte[10000];
            string ctrTxt = ciphertext.Slice(0, 8);
            for (int i = 0; i < 8; i++) counterBlock[i] = Convert.ToByte(ctrTxt[(i)]);
            
            byte[,] keySchedule = aes.keyExpansion(key);
            
            double nBlocks = Math.Ceiling((double)(ciphertext.Length - 8) / blockSize);
            string[] ct = new string[((int)nBlocks)];
            for (int b = 0; b < nBlocks; b++) ct[b] = ciphertext.Slice(8 + b * blockSize, 8 + b * blockSize + blockSize);
            string[] ciphertext1 = ct;
            
            string plaintext = "";

            for (int b = 0; b < nBlocks; b++)
            {
                for (int c = 0; c < 4; c++)
                {
                    byte byte1 = Convert.ToByte(((uint)(b) >> c * 8) & 0xff);
                    counterBlock[15 - c] = Convert.ToByte(((uint)(b) >> c * 8) & 0xff);
                }
                for (int c = 0; c < 4; c++)
                {
                    double byte1 = ((double)(b + 1) / 0x100000000 - 1);
                    byte byte2 = Convert.ToByte(((uint)byte1 >> c * 8) & 0xff);
                    counterBlock[15 - c - 4] = byte2;
                }

                byte[] cipherCntr = aes.cipher(counterBlock, keySchedule);  // encrypt counter block

                char[] plaintxtByte = new char[(ciphertext1[b].Length)];
                for (int i = 0; i < ciphertext1[b].Length; i++)
                {
                    plaintxtByte[i] = Convert.ToChar(cipherCntr[i] ^ Convert.ToByte(ciphertext1[b][(i)]));
                }
                plaintext = plaintext + string.Join("", plaintxtByte);

            }
            return plaintext;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        
        static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static string encriptarRSA(byte[] text)
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
            List<byte> message = new List<byte>();
            foreach (byte b in text)
            {
                double letra = Math.Pow(b, publicKey.E) % publicKey.N;
                byte[] intBytes = BitConverter.GetBytes((int)letra);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                message.AddRange(intBytes);
            }
            string llavePublica = publicKey.E + "\n" + publicKey.N;
            string llavePrivada = privateKey.D + "\n" + privateKey.N;
            File.WriteAllText("llavePublica.key", llavePublica);
            File.WriteAllText("llavePrivada.key", llavePrivada);
            return string.Join("",message);
        }

        static void desencriptarRSA(string file)
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



    public static class Extensions
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            if (end > source.Length) end = source.Length;
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
    }
}
