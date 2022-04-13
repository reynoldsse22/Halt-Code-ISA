using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    class ReservationStation
    {
        public string stationName;        //Name of reservation station
        public bool Busy { get; set; }  //Is there an instruction within the station
        public Instruction instruction { get; set; } //Current instruction
        public int Vk { get; set; } //Register values the instruction needs
        public int Vj { get; set; }
        public string Qk { get; set; } //Names of reservation stations that hold needed register values
        public string Qj { get; set; }
        public int Address { get; set; }

        public int arrayIndex { get; set; }

        //Initialize all values as empty except for the names (which should be hard coded)
        public ReservationStation(string Name)
        {
            stationName = Name;
            Busy = false;
            instruction = new Instruction();
            Vk = 0;
            Vj = 0;
            Qk = "0";
            Qj = "0";
            Address = 0;
        }


    }
}
