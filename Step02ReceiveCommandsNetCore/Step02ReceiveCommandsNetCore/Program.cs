using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Step02ReceiveCommandsNetCore
{
    class Program
    {
        static DeviceClient dc;
        static async Task Main(string[] args)
        {
            string cnn = "HostName=iottk20180514.azure-devices.net;DeviceId=PC01;SharedAccessKey=TDvyUZ2ypwa+e5TxuuSM8Kk7ZWVoH+MYEw1XTL8JQic=";
            dc = DeviceClient.CreateFromConnectionString(cnn, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            //Methods
            await dc.SetMethodHandlerAsync("m1", method_m1, null);
            await dc.SetMethodHandlerAsync("m2", method_m2, null);
            await dc.SetMethodDefaultHandlerAsync(method_default, null);

            //State (property)
            await dc.SetDesiredPropertyUpdateCallbackAsync(propertyUpdateCallback, null);

            Console.WriteLine("Waiting");
            //Optional - waiting for message
            await waitForMessage();
            Console.ReadLine();
        }

        private static async Task waitForMessage()
        {
            while (true)
            {
                var msg = await dc.ReceiveAsync();
                if (msg != null)
                {
                    Console.WriteLine($"{msg.MessageId} - {Encoding.ASCII.GetString(msg.GetBytes())}");
                    if (msg.DeliveryCount > 1)
                    {
                        await dc.CompleteAsync(msg);
                        Console.WriteLine($"{msg.MessageId} - OK");

                    }
                    else
                    {
                        // await dc.AbandonAsync(msg); //Not working in MQTT (due to protocol characteristics)
                        // dc.RejectAsync - delivered, reject
                        Console.WriteLine($"{msg.MessageId} - AbadonAsync");
                    }

                }
            }
        }

        private static Task<MethodResponse> method_default(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine("method_default");
            return Task.FromResult<MethodResponse>(new MethodResponse(
                UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Method = "???", State = true, Msg = "OK" })), 0)
                );
        }

        private static async Task propertyUpdateCallback(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("propertyUpdateCallback");
            TwinCollection reportedProperties = new TwinCollection();
            foreach (dynamic item in desiredProperties)
            {
                Console.WriteLine($"{item} = {item.Value}");
                reportedProperties[item.Key] = item.Value;
            }
            reportedProperties["MyProp"] = 123;
            await dc.UpdateReportedPropertiesAsync(reportedProperties);

        }

        private static Task<MethodResponse> method_m1(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine("method_m1");
            return Task.FromResult<MethodResponse>(new MethodResponse(
                UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Method="m1", State = true, Msg = "OK" })),0)
                );
        }
        private static Task<MethodResponse> method_m2(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine("method_m2");
            return Task.FromResult<MethodResponse>(new MethodResponse(
                UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Method = "m2", State = false, Msg = "Not OK" })), 0)
                );
        }
    }
}
