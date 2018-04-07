using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace virtualMachine
{
    class Operate
    {
        public string FoundAddress(string x)   //寻址  例:T00055:寻址到00055   TAX:设AX=00033 寻址到00033   AX:寻寄存器到AX
        {
            if(x!=null)
            {
                if (x[0] == 'T') { x = x.Substring(1); if (x.Length == 2&&x[1]=='X') x = CPU.Address[x]; }
            }
            return x;
        }
        public string FoundNum(string x)       //寻数  例:00055:数值00055  T00055:地址00055及后面7个的值  TAX  AX:寄存器里的值
        {
            if (x != null)
            {
                if (x[0] == 'T') {
                    x= x.Substring(1);
                    if (x.Length == 2&&x[1]=='X') { x = StdData(CPU.Address[x]); }
                    else x = StdData(x);
                }
                else if(x.Length==2&&x[1]=='X')
                    x = CPU.Address[x];
            }
            return x;
        }

        public string[] SplitData(string x)    //为了小端模式存储，需将16位16进制拆成8个2位16进制(8个8位，8个字节)数，存回数组
        {
            string[] z = new string[CPU.WORD_LENGTH / CPU.PER_STO_LENGTH];
            for (int i = 0; i < z.Length; i++)
            {
                z[i] = ""; z[i] += x[i * 2].ToString(); z[i] += x[i * 2 + 1].ToString();
            }
            return z;
        }
        public string CombineData(string[] x)  //将8个地址的数据合到一起
        {
            string z = "";
            foreach (string i in x) z += i;
            return z;
        }

        public string StdData(string x)        //给出地址取数一下取8个数据，给出寄存器则取寄存器里的数据，返回该取出的数据
        {
            x = FoundAddress(x);
            if (x.Length == 2&&x[1]=='X') return CPU.Address[x];
            else
            {
                int size = CPU.WORD_LENGTH / CPU.PER_STO_LENGTH;
                string[] z = new string[size];
                for(int i = size-1; i >= 0; i--)
                {
                    z[i] = CPU.Address[x];
                    x = Inc(x);
                }
                return CombineData(z);
            }
        }
        public void StoData(string x,string y)  //数据y存到地址x中,小端模式存储
        {
            x = FoundAddress(x); y = FoundNum(y);        
            if (x.Length == 2&&x[1]=='X') CPU.Address[x] = y;
            else
            {
                y = Add0To16(y);
                int size = CPU.WORD_LENGTH / CPU.PER_STO_LENGTH;                         
                string[] z = SplitData(y);
                for(int i=size-1;i>=0;i--)
                {
                    CPU.Address[x] = z[i];
                    x = Inc(x);
                }
            }
        }

        public string Add0To16(string x)  //加前导0进行格式化
        {
            if (x.Length < 16) //数据y必须是16位16进制数
            {
                string s = "";
                for (int i = 0; i < 16 - x.Length; i++) s += "0";
                x = s + x;
            }
            return x;
        }

        public string And(string x,string y)   //与操作
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            string result1 = "", result2 = "", result = "";
            foreach (char c in x)
                result1 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));  //16进制转2进制
            foreach (char c in y)
                result2 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));
            for(int i=0;i<result1.Length;i++)
            {
                if (result1[i] == '1' && result2[i] == '1') result += "1";
                else result += "0";
            }
            return String.Format("{0:X16}", Convert.ToUInt64(result, 2));         //2进制转16进制
        }
        public string Or(string x, string y)   //或操作
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            string result1 = "", result2 = "", result = "";
            foreach (char c in x)
                result1 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));  //16进制转2进制
            foreach (char c in y)
                result2 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));
            for (int i = 0; i < result1.Length; i++)
            {
                if (result1[i] == '0' && result2[i] == '0') result += "0";
                else result += "1";
            }
            return String.Format("{0:X16}", Convert.ToUInt64(result, 2));         //2进制转16进制
        }
        public string Xor(string x, string y)  //异或操作
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            string result1 = "", result2 = "", result = "";
            foreach (char c in x)
                result1 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));  //16进制转2进制
            foreach (char c in y)
                result2 += String.Format("{0:0000}", int.Parse(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)));
            for (int i = 0; i < result1.Length; i++)
            {
                if (result1[i] == result2[i]) result += "0";
                else result += "1";
            }
            return String.Format("{0:X16}", Convert.ToUInt64(result, 2));         //2进制转16进制
        }

        public string Add(string x,string y)   //加运算
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            ulong z = Convert.ToUInt64(x, 16) + Convert.ToUInt64(y, 16);
            return String.Format("{0:X16}", z);
        }
        public string Mul(string x, string y)   //乘运算
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            ulong z = Convert.ToUInt64(x, 16) * Convert.ToUInt64(y, 16);
            return String.Format("{0:X16}", z);
        }
        public string Div(string x, string y)   //除运算
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            ulong z = (Convert.ToUInt64(x, 16) / Convert.ToUInt64(y, 16)) % 0x10000;
            return String.Format("{0:X16}", z);
        }

        public string Inc(string x)   //自增运算
        {
            x = FoundNum(x);
            if(x.Length==5)
            {
                if (x == "FFFFF") return "00000";
                else return String.Format("{0:X5}", Convert.ToInt32(x, 16) + 0x1);
            }
            else
            {
                if (x == "FFFFFFFFFFFFFFFF") return "0000000000000000";
                else return String.Format("{0:X16}", Convert.ToUInt64(x, 16) + 0x1);
            }
            
        }
        public string Sub(string x)   //自减运算
        {
            x = FoundNum(x);
            if (x.Length == 5)
            {
                if (x == "00000") return "FFFFF";
                else return String.Format("{0:X5}", Convert.ToInt32(x, 16) - 0x1);
            }
            else
            {
                if (x == "0000000000000000") return "FFFFFFFFFFFFFFFF"; 
                else return String.Format("{0:X16}", Convert.ToUInt64(x, 16) - 0x1);
            }
        }    
        public string Lsh(string x)   //左移操作
        {
            x = Add0To16(FoundNum(x));
            return String.Format("{0:X16}", Convert.ToUInt64(x, 16) << 1);      
        }
        public string Rsh(string x)   //右移操作
        {
            x = Add0To16(FoundNum(x));
            return String.Format("{0:X16}", Convert.ToUInt64(x, 16) >> 1);
        }

        public int Cmp(string x, string y)   //比较得到SF值
        {
            x = Add0To16(FoundNum(x)); y = Add0To16(FoundNum(y));
            if (x.CompareTo(y) < 0) return 1;
            else return 0;
        }

        public void Push(string x)
        {
            for (int i = 0; i < 7; i++) CPU.SP = Sub(CPU.SP);
            StoData("T"+CPU.SP, x);
            CPU.SP = Sub(CPU.SP);
        }
        public void Pop(string x)
        {
            CPU.SP = Inc(CPU.SP);
            StoData(x, "T"+CPU.SP);
            for (int i = 0; i < 7; i++) CPU.SP = Inc(CPU.SP);
        }

        public void F(string[] x)
        {
            if (x[0] == "RUN") { CPU.RunFlag = true; } //开机操作  
            else if (x[0] == "STOP"){ CPU.StopFlag = true; }//关机操作
            else if (x[0] == "ECHO"){ Console.WriteLine(StdData(x[1])); }//输出操作
            else if (x[0] == "MOV") { StoData(x[1],x[2]); }//赋值操作
            else if (x[0] == "ADD") { StoData(x[1], Add(x[1], x[2])); }//加运算
            else if (x[0] == "INC") { StoData(x[1], Inc(x[1])); }//自增运算
            else if (x[0] == "SUB") { StoData(x[1], Sub(x[1])); }//自减运算
            else if (x[0] == "AND") { StoData(x[1], And(x[1],x[2])); }//与运算
            else if (x[0] == "OR")  { StoData(x[1], Or(x[1],x[2])); }//或运算
            else if (x[0] == "XOR") { StoData(x[1], Xor(x[1],x[2])); }//异或运算
            else if (x[0] == "LSH") { StoData(x[1], Lsh(x[1])); }//左移运算
            else if (x[0] == "RSH") { StoData(x[1], Rsh(x[1])); }//右移运算
            else if (x[0] == "MUL") { StoData(x[1], Mul(x[1],x[2])); }//乘运算
            else if (x[0] == "DIV") { StoData(x[1], Div(x[1],x[2])); }//除运算           
            else if (x[0] == "CMP") { CPU.SF = Cmp(x[1], x[2]); Console.WriteLine("SF:{0}", CPU.SF); }//比较操作
            else if (x[0] == "PUSH"){ Push(x[1]); }//压栈操作
            else if (x[0] == "POP") { Pop(x[1]); }//出栈操作
            else if (x[0] == "JMP") { CPU.PC = Convert.ToInt32(FoundNum(x[1]), 10) - 1; }//无条件跳转
            else if (x[0] == "JSA") { if (CPU.SF == 1) CPU.PC = Convert.ToInt32(FoundNum(x[1]), 10) - 1; }//SF=1时跳转
            else if (x[0] == "JSB") { if (CPU.SF == 0) CPU.PC = Convert.ToInt32(FoundNum(x[1]), 10) - 1; }//SF=0时跳转
            else if (x[0] == "INT") { Console.WriteLine("21"); }
            else if (x[0] == "RTI") { Console.WriteLine("22"); }
            else if (x[0] == "POT") { Console.WriteLine("23"); }
            else if (x[0] == "NEXT"){ Console.WriteLine("24"); }
            else if (x[0] == "CRA") { Console.WriteLine("25"); }
            else if (x[0] == "STO") { Console.WriteLine("26"); }
            else if (x[0] == "STD") { Console.WriteLine("27"); }
        }
    }
}
