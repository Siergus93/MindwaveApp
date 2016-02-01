using System;
using libUCLA;
using System.Threading;

namespace ReceivingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ULoader_JSON config = new ULoader_JSON("config.json");
            UReceiver uReceiver = config.GetReceiver("input1");
            while(true)
            {
                try
                {
                    uReceiver.DataReceived += new UReceiveHandler(OnData);
                    uReceiver.Receive();   
                }
                catch (UException exc)
                {
                    Console.Write(exc.Message);
                }
                Thread.Sleep(1000);
            }
        }

        private static void OnData(object sender, UDataReceivedArgs data)
        {
            Console.Write("Attention: " + BitConverter.ToSingle(data.Buffer, 0) + "T: " + DateTime.UtcNow.ToString() + "\r");
        }
    }
}
