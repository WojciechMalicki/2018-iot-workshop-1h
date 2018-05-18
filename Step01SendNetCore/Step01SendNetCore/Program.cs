using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Step01SendNetCore
{
    class Program
    {

        static void Main(string[] args)
        {
            int taskCnt = 10;
            //string cnn = "HostName=pltkdpepliot2016S1.azure-devices.net;DeviceId=PC;SharedAccessKey=MKFlutvnHn/WutJkDGbRPZ/PgGSQIKDmOhMtIivt198=";
	        string cnn = "HostName=iottk20180514.azure-devices.net;DeviceId=PC01;SharedAccessKey=TDvyUZ2ypwa+e5TxuuSM8Kk7ZWVoH+MYEw1XTL8JQic=";
            EmulatedDevice dev = new EmulatedDevice(cnn);
            Task<int>[] t = new Task<int>[taskCnt];

            Console.WriteLine("Start");
            for (int i = 0; i < taskCnt; i++)
            {
                t[i] = new Task<int>(delegate { return dev.SendMessagesAsync($"DEVTK").Result; });
                t[i].Start();
            }
            Task.WaitAll(t);
            Console.WriteLine("Finished");
        }
    }
}
