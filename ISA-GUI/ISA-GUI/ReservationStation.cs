using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    class ReservationStation
    {
        string stationName;        //Name of reservation station
        bool Busy;          //Is there an instruction within the station
        Instruction instruction;     //Current instruction
        int Vk;             //Register values the instruction needs
        int Vj;             
        string Qk;          //Names of reservation stations that hold needed register values
        string Qj;
        int Address;

        //Initialize all values as empty except for the names (which should be hard coded)
        public ReservationStation(string Name)
        {
            stationName = Name;
            Busy = false;
            instruction = new Instruction();
            Vk = 0;
            Vj = 0;
            Qk = "";
            Qj = "";
            Address = 0;
        }


    }
}
