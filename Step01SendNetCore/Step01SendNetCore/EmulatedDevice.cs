using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Step01SendNetCore
{
    class EmulatedDevice
    {
        string m_devConnection;
        DeviceClient dc;
        Random rnd = new Random();

        public EmulatedDevice(string cnnString)
        {
            m_devConnection = cnnString;
            dc = DeviceClient.CreateFromConnectionString(m_devConnection,
                Microsoft.Azure.Devices.Client.TransportType.Mqtt);
        }

        private Microsoft.Azure.Devices.Client.Message createMessage(string devprefix="")
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = new TimeSpan(now.Year, now.Minute, now.Second, now.Millisecond);
            POI p = Data.LocGdansk[rnd.Next(Data.LocGdansk.Length)];
            int devid = rnd.Next(Data.DeviceNames.Length);
            MAllNum mallnum = new MAllNum()
            {
                DeviceName = devprefix + Data.DeviceNames[devid],
                Light = 1000 * Math.Cos(ts.TotalMilliseconds / (175 + devid*4) ) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30),
                Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / (275 + devid * 4)) * Math.Sin(ts.TotalMilliseconds / (560 + devid * 8)) + rnd.Next(30),
                Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / (60 + devid * 4)) * Math.Sin(ts.TotalMilliseconds / (120 + devid * 4)) + rnd.Next(30),
                Pressure = (float)(1000 * Math.Cos(ts.TotalMilliseconds / (180 + devid * 4)) * Math.Sin(ts.TotalMilliseconds / (1310 + devid * 14)) + rnd.Next(30)),
                Temperature = (float)(1000 * Math.Cos(ts.TotalMilliseconds / (180 + devid * 4)) * Math.Sin(ts.TotalMilliseconds / (110 + devid * 6)) + rnd.Next(30)),
                ADC3 = rnd.Next(1000),
                ADC4 = rnd.Next(1000),
                ADC5 = rnd.Next(1000),
                ADC6 = rnd.Next(1000),
                ADC7 = rnd.Next(1000),
                Altitude = (float)(1000 * Math.Cos(ts.TotalMilliseconds / (480 + devid * 4)) * Math.Sin(ts.TotalMilliseconds / (2000 + devid * 22)) + rnd.Next(30)),
                Latitude = p.X,
                Longitude = p.Y
            };
            var messageString = JsonConvert.SerializeObject(mallnum);
            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString));
            return message;
        }

        public async Task<int> SendMessagesAsync(string devprefix="", int maxCount = 1000000)
        {
            Console.WriteLine($"{devprefix} - {Thread.CurrentThread.ManagedThreadId } - {maxCount}");
            for (int i = 0; i < maxCount; i++)
            {
                var msg = createMessage(devprefix);
                await dc.SendEventAsync(msg);
                await Task.Delay(500+rnd.Next(500));
                if (i%20 == 0)
                {
                    Console.WriteLine($"{devprefix} - {Thread.CurrentThread.ManagedThreadId } - {i}");
                }
            }
            return maxCount;
        }

    }
}
