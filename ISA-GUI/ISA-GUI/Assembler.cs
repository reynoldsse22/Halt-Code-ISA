using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    internal class Assembler
    {

        public Assembler()
        {

        }

        /**
		 * Method Name: getInput <br>
		 * Method Purpose: Gets the instructions from the user and splits it on the space
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 * Return: program - a list of three byte instructions
		 */
        public List<string> Assemble(List<string> assembly)
        {
            List<string> program = new List<string>();
            int instructionValue = 0;
            string pr = "";
            string machineOutput = "";
            try
            {
                foreach (string s in assembly)
                {
                    instructionValue = 0;
                    string str = s.ToLower();

                    string[] instruction = str.Split(' ');
                    if (instruction.Length < 2)
                    {
                        if (instruction[0].Equals("halt"))
                            instructionValue += 0;
                        else if (instruction[0].Equals("nop"))
                            instructionValue += 65536;
                        else
                            throw new Exception();
                        goto endOfInput;
                    }

                    string[] firstHalf = instruction[0].Split('.');

                    if (firstHalf[0].Equals("f"))
                    {
                        instructionValue += 4194304;
                        if (firstHalf.Length > 2)
                        {
                            if (firstHalf[2].Equals("s"))
                            {
                                instructionValue += 2097152;
                            }
                            else
                                throw new Exception();
                        }

                        instructionValue += decodeAssembly(firstHalf[1], instruction[1]);

                    }
                    else
                    {
                        if (firstHalf.Length > 1)
                        {
                            if (firstHalf[1].Equals("s"))
                            {
                                instructionValue += 2097152;
                            }
                            else
                                throw new Exception();
                        }
                        instructionValue += decodeAssembly(firstHalf[0], instruction[1]);
                    }
                endOfInput:
                    string bytes = instructionValue.ToString("X");
                    if (bytes.Equals("0"))
                        bytes = "000000";
                    else if (bytes.Equals("1"))
                        bytes = "000001";
                    if (bytes.Length == 5)
                    {
                        bytes = bytes.Insert(0, "0");
                    }
                    for (int i = 0; i < bytes.Length; i += 2)
                    {
                        program.Add(bytes.Substring(i, 2));
                    }
                }
            }
            catch (Exception)                                        //If any error in parsing, catch exception and pass to the parent
            {
                throw new Exception("Invalid Instruction!");
            }
            return program;
        }


        private int decodeAssembly(string firstHalf, string secondHalf)
        {
            int instructionValue = 0;
            //string[] operands = secondHalf.Split(',');

            if (firstHalf.Equals("br"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 131072;
            }
            else if (firstHalf.Equals("brne"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 196608;
            }
            else if (firstHalf.Equals("breq"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 262144;
            }
            else if (firstHalf.Equals("brlt"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 327680;
            }
            else if (firstHalf.Equals("brle"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 393216;
            }
            else if (firstHalf.Equals("brgt"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 458752;
            }
            else if (firstHalf.Equals("brge"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 524288;
            }
            else if (firstHalf.Equals("ldwm"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 589824;
            }
            else if (firstHalf.Equals("stwm"))
            {
                instructionValue += decodeHex(secondHalf);
                instructionValue += 655360;
            }
            else if (firstHalf.Equals("ldhl"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 1);
                instructionValue += decodeDecValue(split[1]);
                instructionValue += 720896;
            }
            else if (firstHalf.Equals("ldhh"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 1);
                instructionValue += decodeDecValue(split[1]);
                instructionValue += 786432;
            }
            else if (firstHalf.Equals("cmpi"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 1);
                instructionValue += decodeDecValue(split[1]);
                instructionValue += 851968;
            }
            else if (firstHalf.Equals("cmpr"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 3);
                instructionValue += decodeRegister(split[1], 4);
                instructionValue += 917504;
            }
            else if (firstHalf.Equals("mov"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 3);
                instructionValue += decodeRegister(split[1], 4);
                instructionValue += 983040;
            }
            else if (firstHalf.Equals("asl"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1048576;
            }
            else if (firstHalf.Equals("asr"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1114112;
            }
            else if (firstHalf.Equals("lsl"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1179648;
            }
            else if (firstHalf.Equals("lsr"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1245184;
            }
            else if (firstHalf.Equals("add"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1310720;
            }
            else if (firstHalf.Equals("sub"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1376256;
            }
            else if (firstHalf.Equals("mult"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1441792;
            }
            else if (firstHalf.Equals("div"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1507328;
            }
            else if (firstHalf.Equals("and"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1572864;
            }
            else if (firstHalf.Equals("or"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1638400;
            }
            else if (firstHalf.Equals("xor"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 3);
                instructionValue += decodeRegister(split[2], 4);
                instructionValue += 1703936;
            }
            else if (firstHalf.Equals("not"))
            {
                string[] split = secondHalf.Split(',');
                instructionValue += decodeRegister(split[0], 2);
                instructionValue += decodeRegister(split[1], 4);
                instructionValue += 1769472;
            }
            else
                throw new Exception();

            return instructionValue;
        }

        private int decodeHex(string hex)
        {
            int instructionValue = 0;
            if (hex[0] == '0' && hex[1] == 'x')
            {
                hex = hex.Substring(2);
                instructionValue += int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            }
            else
                throw new Exception();
            return instructionValue;
        }

        private int decodeRegister(string input, int bytePlace)
        {
            int instructionValue = 0;
            if (input[0] == 'r' || input[0] == 'f')
            {
                string sub = input.Substring(1);
                int value = int.Parse(sub);
                switch (bytePlace)
                {
                    case 1:
                        instructionValue += value << 12;
                        break;
                    case 2:
                        instructionValue += value << 8;
                        break;
                    case 3:
                        instructionValue += value << 4;
                        break;
                    case 4:
                        instructionValue += value;
                        break;
                }
            }
            else
                throw new Exception();
            return instructionValue;
        }

        private int decodeDecValue(string input)
        {
            int instructionValue = 0;
            if (input[0] == '#')
            {
                string sub = input.Substring(1);
                int value = int.Parse(sub);
                instructionValue += value;
            }
            else
                throw new Exception();
            return instructionValue;
        }
    }
}
