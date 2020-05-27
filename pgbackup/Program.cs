using System;

namespace pgbackup
{
    class Program
    {
        internal static string DBname = null;
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
                xmlgen.Setparamfoxml();
                Function.Sortbackup();
            }
        }
        /*
         * Отправка почты
         */
        internal static async void SendMessage()
        {
            Email emailService = new Email();
            await emailService.SendEmailAsync(Function.GetEmail, "Резервное копирование", Function.Message);
        }
    }
}
