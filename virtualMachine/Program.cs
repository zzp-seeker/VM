using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMachine
{
    class Program
    {
        public static string[] iSet;                   //指令集
        public static int iSetSum;                     //指令总数
        private static string[] AnalysisI(string x)    //解析指令
        {
            return x.Split(new char[] { ' '},StringSplitOptions.RemoveEmptyEntries);
        }
        

        static void Main(string[] args)
        {
            Operate op = new Operate();
            iSet = new string[5];   
            iSetSum = 0;

            //Console.WriteLine(String.Format("{0:0000000000000000}", Convert.ToInt32("40001", 16)));

            /*string t1 = "02AC0582A598C485";
            string[] t2 = { "02", "TB", "AD", "ED", "AS", "15", "65", "15" };

            string[] t1_r = op.SplitData(t1);
            foreach (string i in t1_r) Console.WriteLine(i);
            string t2_r = op.CombineData(t2);
            Console.WriteLine(t2_r);*/
           
            while (true)                      
            {
                string x = Console.ReadLine();            
                iSet[iSetSum] = x;
                iSetSum++;
                if (x == "STOP") break;
                if (iSetSum == iSet.Length) Array.Resize(ref iSet, iSet.Length + 5);   //动态增加指令集数组大小
            }
            while(!CPU.StopFlag)
            {
                //Console.WriteLine(CPU.PC);
                op.F(AnalysisI(iSet[CPU.PC-1]));
                CPU.PC++;
            }

            foreach(KeyValuePair<string,string> kvp in CPU.Address)
            {
                Console.WriteLine("address:{0} value:{1}", kvp.Key, kvp.Value);
            }







            Console.ReadKey();
        }
    }
}
