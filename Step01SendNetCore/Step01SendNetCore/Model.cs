using System;
using System.Collections.Generic;
using System.Text;

namespace Step01SendNetCore
{
    public class POI
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public POI(string id, string desc, double x, double y)
        {
            ID = id;
            Description = desc;
            X = x;
            Y = y;
        }
    }

    public class MIoTBase
    {
        public DateTime Dt { get; } = DateTime.Now;
        public string MsgType { get; set; }
        public string DeviceName { get; set; }
        protected MIoTBase(string msgType)
        {
            Dt = DateTime.Now;
            MsgType = msgType;
        }
    }

    public class MAllNum: MIoTBase
    {
        public double Potentiometer1 { get; set; }
        public double Potentiometer2 { get; set; }
        public double Light { get; set; }
        public double ADC3 { get; internal set; }
        public double ADC4 { get; internal set; }
        public double ADC5 { get; internal set; }
        public double ADC6 { get; internal set; }
        public double ADC7 { get; internal set; }
        public float Altitude { get; internal set; }
        public float Pressure { get; internal set; }
        public float Temperature { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
        public MAllNum() : base("MAllNum") { }
    }

}
