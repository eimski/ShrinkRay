using System;
using System.IO;

namespace shrinkray
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).ToString();
            path = path.Replace("\\bin", string.Empty);
            Stream fs = new FileStream(path + "\\epdbuffer.txt", FileMode.Open);

            byte[] buf = new byte[fs.Length];
            
            fs.Read(buf, 0, buf.Length);
            fs.Close();

            Console.WriteLine("length of original data: " + buf.Length);
            var compressData = Compress(buf);
            Console.WriteLine("length of compressed data: " + compressData.Length);
            var inflateData = Inflate(compressData);
            Console.WriteLine("length of inflated data: " + inflateData.Length);
            Console.Read();

        }

        public static byte[] Compress(byte[] data)
        {
            byte[] buf = new byte[data.Length];
            byte prevbyte = data[0];
            int count = 0;
            BitConverter.GetBytes(data.Length).CopyTo(buf,0);
            int count2 = 4;//byte 0~4 for length info

            for (int i = 0; i < data.Length; i++)
            {
                count++;
                if (data[i] != prevbyte)
                {
                    byte[] x = BitConverter.GetBytes(i);
                    Array.Copy(x, 0, buf, count2, 4 );
                    buf[count2 + 4] = prevbyte;

                    count2 += 5;
                    prevbyte = data[i];
                    count = 0;

                }
            }
            byte[] compress = new byte[count2];
            Array.Copy(buf, compress, count2);
            
            return compress;

        }

        public static byte[] Inflate(byte[] data)
        {
            byte[] num = new byte[4];
            Array.Copy(data, num, 4);
            int bufPos = 0;

            int length = BitConverter.ToInt32(num, 0);

            byte[] buf = new byte[length];
           
            for(int count=4; count < data.Length; count++)
            {
                Array.Copy(data, count, num, 0, 4);
                int valuecount = BitConverter.ToInt32(num, 0);
                count += 4;
                byte value = data[count];//value is value of data, valuecount is number of repeated value in the buffer                                               

                for(int i = bufPos; i < length; i++)
                {
                    buf[i] = value;

                }

                bufPos = bufPos + valuecount;

            }
            return buf;

        }
    }
}
