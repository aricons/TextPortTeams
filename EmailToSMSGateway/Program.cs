using System;

namespace EmailToSMSGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //MessageParser.ParseMessage(@"C:\Junk\MDaemon\FreeTextHub\ATTmd50000000640.msg");
                if (args.Length > 0)
                {
                    MessageParser.ParseMessage(args[0]);
                }
            }
            catch (Exception ex)
            {
                MessageParser.WriteDebugEntry("Exception: " + ex.Message);
            }
        }
    }
}
