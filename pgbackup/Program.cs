using System;

namespace pgbackup
{
    class Program
    {
        public static string DBname = null;
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "firstsetting")
            {
                xmlgen.StartXmlgen();
            }
            else if (args.Length == 1 && args[0] == "backup")
            {
                xmlgen.Setparamfoxml();
                foreach (string db in Function.DBnameMas)
                {
                    Function.DBname = db.Trim(' ');
                    Function.Backup();
                }
                Function.Sortbackup();
                Function.DelLog();
                SendMessage();
                //Function.CopyToMega();
            }
            else if (args.Length == 1 && args[0] == "backupdb")
            {
                Function.BackupDataBase();
            }
            else if (args.Length == 1 && args[0] == "scheduledtasks")
            {
                Function.ReglamentOne();
            }
            else if (args.Length == 1 && args[0] == "test")
            {
                //xmlgen.Setparamfoxml();
                //Console.WriteLine(Function.SendEmail);
                //Console.WriteLine(Function.GetEmail);
                //Console.WriteLine(Function.SendEmailPass);
                ////try
                ////{
                ////    SendMessage(Function.GetEmail);
                ////}
                ////catch (Exception e)
                ////{
                ////    Console.WriteLine(e);
                ////    Console.ReadLine();
                ////}
            }
        }
        /*
         * Отправка почты
         */
        public static async void SendMessage()
        {
            Email emailService = new Email();
            await emailService.SendEmailAsync(Function.GetEmail, "Резервное копирование", Function.Message);
        }
    }
}
