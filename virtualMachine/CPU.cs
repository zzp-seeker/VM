using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMachine
{
    class CPU
    {
        public static bool RunFlag=false;
        public static bool StopFlag = false;

        public const int WORD_LENGTH = 64;    //字长64位
        public const int PER_STO_LENGTH = 8;  //每个地址存储位数

        public static int PC = 1;
        public static int SF = 0;           //状态字SF，当负时为1，其他为0
        public static string SP = "EFFFF";  //堆栈SP指针

        public static Dictionary<string, string> Address=new Dictionary<string, string>();  //寄存器AX,BX,CX,DX,EX,FX,GX,HX也在其中   
        
        private static void Init()
        {
            //初始化8个寄存器
            Address.Add("AX", "00000"); Address.Add("BX", "00000"); Address.Add("CX", "00000"); Address.Add("DX", "00000");
            Address.Add("EX", "00000"); Address.Add("FX", "00000"); Address.Add("GX", "00000"); Address.Add("HX", "00000");
        }     
    }
}
