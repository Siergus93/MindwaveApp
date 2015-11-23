using System;
using libUCLA;

namespace ReceivingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ULoader_JSON config = new ULoader_JSON("config.json");
                UReceiver uReceiver = config.GetReceiver("input1");
                uReceiver.DataReceived += new UReceiveHandler(OnData);
                uReceiver.Receive();

            }
            catch(UException exc)
            {
                Console.Write(exc.Message);
            }

            Console.ReadKey();
        }

        private static void OnData(object sender, UDataReceivedArgs data)
        {
            foreach (byte b in data.Buffer)
            {
                Console.Write(b + "\t");
            }
        }
    }
}
