using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace pgbackup
{
    class xmlgen
    {
        public static string monthD = null;
        public static string storageDay = null;
        public static string storageMega = null;
        public static string storageDayLog = null;
        public static string minSizeB = null;
        public static string backupDirDay = null;
        public static string backupDirMonth = null;
        public static string backupMega = null;
        public static string FullLog = null;
        public static string ShortLog = null;
        public static string LogReglament = null;
        public static string DBnameMas = null;
        //почта
        public static string SendEmail = null;
        public static string SendEmailPass = null;
        public static string GetEmail = null;
        public static string Key = null;
        //
        public static string p;
        //при создании папки давать на неё права
        public static void StartXmlgen()
        {
            if (File.Exists(Function.param))
            {
                Console.WriteLine("Параметры уже заданы:");
                Check();
            }
            else
            {
                XmlGen();
            }
        }
        public static void XmlGen()
        {
            ConsoleKeyInfo key;
            var rule = @"[0-9]";
            var sb = "";
            XmlDocument XmlParam = new XmlDocument();
            XmlDeclaration XmlDec = XmlParam.CreateXmlDeclaration("1.0", null, null);
            XmlParam.AppendChild(XmlDec);
            //Корень <Options>
            XmlElement xOptions = XmlParam.CreateElement("Options");
            XmlParam.AppendChild(xOptions);
            //<Options><Storage></Storage></Options>
            XmlElement xStorage = XmlParam.CreateElement("Storage");
            xOptions.AppendChild(xStorage);
            //monthD
            Console.WriteLine("Количество дней хранения месячных копий\nEnter для значения по умолчанию: 320");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            monthD = sb;
            sb = "";
            if (monthD == "")
            {
                monthD = "320";
            }
            XmlElement xMonthD = XmlParam.CreateElement("monthD");
            xMonthD.InnerText = monthD;
            xStorage.AppendChild(xMonthD);
            //storageDay
            Console.WriteLine("\nКоличество дней хранения дневных копий\nEnter для значения по умолчанию: 20");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            storageDay = sb;
            sb = "";
            if (storageDay == "")
            {
                storageDay = "20";
            }
            XmlElement xStorageDay = XmlParam.CreateElement("storageDay");
            xStorageDay.InnerText = storageDay;
            xStorage.AppendChild(xStorageDay);
            //storageMega
            Console.WriteLine("\nКоличество дней хранения дневных копий в Mega\nEnter для значения по умолчанию: 1");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            storageMega = sb;
            sb = "";
            if (storageMega == "")
            {
                storageMega = "1";
            }
            XmlElement xStorageMega = XmlParam.CreateElement("storageMega");
            xStorageMega.InnerText = storageMega;
            xStorage.AppendChild(xStorageMega);
            //storageDayLog
            Console.WriteLine("\nКоличество дней хранения логов\nEnter для значения по умолчанию: 20");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            storageDayLog = sb;
            sb = "";
            if (storageDayLog == "")
            {
                storageDayLog = "20";
            }
            XmlElement xStorageDayLoga = XmlParam.CreateElement("storageDayLog");
            xStorageDayLoga.InnerText = storageDayLog;
            xStorage.AppendChild(xStorageDayLoga);
            //<Options><Minsize></Minsize></Options>
            XmlElement xMinsize = XmlParam.CreateElement("Minsize");
            xOptions.AppendChild(xMinsize);
            //minSizeB
            Console.WriteLine("\nМинимальный размер бекапа (в байтах)\nEnter для значения по умолчанию: 1000");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            minSizeB = sb;
            sb = "";
            if (minSizeB == "")
            {
                minSizeB = "1000";
            }
            XmlElement xMinSizeB = XmlParam.CreateElement("minSizeB");
            xMinSizeB.InnerText = minSizeB;
            xMinsize.AppendChild(xMinSizeB);
            //<Options><Path></Path></Options>
            XmlElement xPath = XmlParam.CreateElement("Path");
            xOptions.AppendChild(xPath);
            //backupDirDay
            p = Function.сurrentPath + "/day/";
            Console.WriteLine("\nПуть к папке с дневными бекапами \n Пример: /mnt/1CBackup/day/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            backupDirDay = Console.ReadLine();
            if (backupDirDay == "")
            {
                if (Directory.Exists(p))
                {
                    backupDirDay = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    backupDirDay = p;
                }
            }
            p = "";
            XmlElement xBackupDirDay = XmlParam.CreateElement("backupDirDay");
            xBackupDirDay.InnerText = backupDirDay;
            xPath.AppendChild(xBackupDirDay);
            //backupDirMonth
            p = Function.сurrentPath + "/month/";
            Console.WriteLine("Путь к папке с месячными бекапами \n Пример: /mnt/1CBackup/month/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            backupDirMonth = Console.ReadLine();
            if (backupDirMonth == "")
            {
                if (Directory.Exists(p))
                {
                    backupDirMonth = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    backupDirMonth = p;
                }
            }
            p = "";
            XmlElement xBackupDirMonth = XmlParam.CreateElement("backupDirMonth");
            xBackupDirMonth.InnerText = backupDirMonth;
            xPath.AppendChild(xBackupDirMonth);
            //backupMega
            p = Function.сurrentPath + "/Mega/";
            Console.WriteLine("Путь к папке для синхронизации в Mega \n Пример: /mnt/1CBackup/Mega/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            backupMega = Console.ReadLine();
            if (backupMega == "")
            {
                if (Directory.Exists(p))
                {
                    backupMega = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    backupMega = p;
                }
            }
            p = "";
            XmlElement xBackupMega = XmlParam.CreateElement("backupMega");
            xBackupMega.InnerText = backupMega;
            xPath.AppendChild(xBackupMega);
            //FullLog
            p = Function.сurrentPath + "/log/FullLog/";
            Console.WriteLine("Путь к папке с полными логами \n Пример: /mnt/1CBackup/log/FullLog/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            FullLog = Console.ReadLine();
            if (FullLog == "")
            {
                if (Directory.Exists(p))
                {
                    FullLog = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    FullLog = p;
                }
            }
            p = "";
            XmlElement xFullLog = XmlParam.CreateElement("FullLog");
            xFullLog.InnerText = FullLog;
            xPath.AppendChild(xFullLog);
            //ShortLog
            p = Function.сurrentPath + "/log/ShortLog/";
            Console.WriteLine("Путь к папке с краткими логами \n Пример: /mnt/1CBackup/log/ShortLog/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            ShortLog = Console.ReadLine();
            if (ShortLog == "")
            {
                if (Directory.Exists(p))
                {
                    ShortLog = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    ShortLog = p;
                }
            }
            p = "";
            XmlElement xShortLog = XmlParam.CreateElement("ShortLog");
            xShortLog.InnerText = ShortLog;
            xPath.AppendChild(xShortLog);
            //LogReglament
            p = Function.сurrentPath + "/log/ReglamentLog/";
            Console.WriteLine("Путь к папке с логами выполнения регламентных заданий \n Пример: /mnt/1CBackup/log/ReglamentLog/ \n Владелец папки должен быть postgres с правами на запись\nEnter для значения по умолчанию: " + p);
            LogReglament = Console.ReadLine();
            if (LogReglament == "")
            {
                if (Directory.Exists(p))
                {
                    LogReglament = p;
                }
                else
                {
                    Directory.CreateDirectory(p);
                    LogReglament = p;
                }
            }
            p = "";
            XmlElement xLogReglament = XmlParam.CreateElement("LogReglament");
            xLogReglament.InnerText = LogReglament;
            xPath.AppendChild(xLogReglament);
            //<Options><DataBase></DataBase></Options>
            XmlElement xDataBase = XmlParam.CreateElement("DataBase");
            xOptions.AppendChild(xDataBase);
            //DBnameMas
            Function.GetDBName();
            Console.WriteLine("Список доступных баз: " + Function.output + "\n Enter для заполнения по умолчанию\n Свои значения вводить через запятую, как в примере выше");
            DBnameMas = Console.ReadLine();
            if (DBnameMas == "")
            {
                DBnameMas = Function.output;
            }
            XmlElement xDBnameMas = XmlParam.CreateElement("DBnameMas");
            xDBnameMas.InnerText = DBnameMas;
            xDataBase.AppendChild(xDBnameMas);
            //уведомления на почту
            //<Options><Alert></Alert></Options>
            XmlElement xAlert = XmlParam.CreateElement("Alert");
            xOptions.AppendChild(xAlert);
            Console.WriteLine("Требуется ли отправка уведомлений на почту?\n");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            if (sb == "y" || sb == "" || sb == "Y")
            {
                /*
                 * Key - к статическому ключу добавляем текущую дату и время, отправляем шифровать.
                 * зашифрованный ключ записывается в Xml
                 * 
                 * логин и пароль шифруются получившимся ключом 
                 * 
                 * почта для уведомлений так же шифруется
                 * 
                 */
                Key = DateTime.Now.ToString() + Crypto.k1 + DateTime.Now.ToString();
                XmlElement xKey = XmlParam.CreateElement("Key");
                Key = Crypto.Encrypt(Key, Crypto.k1);
                xKey.InnerText = Key;
                xAlert.AppendChild(xKey);
                //
                XmlElement xAlertbool = XmlParam.CreateElement("Alertbool");
                xAlertbool.InnerText = "true";
                xAlert.AppendChild(xAlertbool);
                //
                Key = Crypto.Decrypt(Key, Crypto.k1);
                //
                Console.Write("Логин почты для отправки:");
                SendEmail = Console.ReadLine();
                XmlElement xSendEmail = XmlParam.CreateElement("SendEmail");
                xSendEmail.InnerText = Crypto.Encrypt(SendEmail, Key);
                xAlert.AppendChild(xSendEmail);
                //
                Console.Write("Пароль от почты для отправки:");
                SendEmailPass = Console.ReadLine();
                XmlElement xSendEmailPass = XmlParam.CreateElement("SendEmailPass");
                xSendEmailPass.InnerText = Crypto.Encrypt(SendEmailPass, Key);
                xAlert.AppendChild(xSendEmailPass);
                //
                Console.Write("Логин почты для отправки:");
                GetEmail = Console.ReadLine();
                XmlElement xGetEmail = XmlParam.CreateElement("GetEmail");
                xGetEmail.InnerText = Crypto.Encrypt(GetEmail, Key);
                xAlert.AppendChild(xGetEmail);
            }
            else if (sb == "n" || sb == "N")
            {
                XmlElement xAlertbool = XmlParam.CreateElement("Alertbool");
                xAlertbool.InnerText = "false";
                xAlert.AppendChild(xAlertbool);
            }
            XmlParam.Save(Function.param);
            Check();
        }
        public static void Check()
        {
            ConsoleKeyInfo key;
            var rule = @"[y,n]";
            var sb = "";
            var XmlDoc = XDocument.Load(Function.param);
            Console.WriteLine("\nПроверка введённых данных:");
            foreach (var x in XmlDoc.Descendants())
            {
                if (x.HasElements)
                    Console.WriteLine("\n{0}\n", x.Name);
                else
                    Console.WriteLine("{0}\t{1}", x.Name, x.Value);
            }
            Console.WriteLine("Данные введены верно? (y/n)");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (System.Text.RegularExpressions.Regex.IsMatch(key.KeyChar.ToString(), rule))
                {
                    sb += key.KeyChar; ;
                    Console.Write(key.KeyChar);
                }
            }
            if (sb == "y" || sb == "" || sb == "Y")
            {
                Console.WriteLine("\nНастройка произведена успешно!");
                Console.ReadKey();
            }
            else if (sb == "n" || sb == "N")
            {
                Console.WriteLine("\nРанее введённые данные будут сброшены. Для продолжения нажмите любую клавишу.");
                Console.ReadKey();
                File.Delete(Function.param);
                XmlGen();
            }
        }
        static void Setdefparamfoxml()
        { 
            XmlDocument XmlParam = new XmlDocument();
            XmlDeclaration XmlDec = XmlParam.CreateXmlDeclaration("1.0", null, null);
            XmlParam.AppendChild(XmlDec);
            //Корень <Options>
            XmlElement xOptions = XmlParam.CreateElement("Options");
            XmlParam.AppendChild(xOptions);
            //<Options><Storage></Storage></Options>
            XmlElement xStorage = XmlParam.CreateElement("Storage");
            xOptions.AppendChild(xStorage);
            //monthD
            XmlElement xMonthD = XmlParam.CreateElement("monthD");
            xMonthD.InnerText = "320";
            xStorage.AppendChild(xMonthD);
            //storageDay
            XmlElement xStorageDay = XmlParam.CreateElement("storageDay");
            xStorageDay.InnerText = "20";
            xStorage.AppendChild(xStorageDay);
            //storageMega
            XmlElement xStorageMega = XmlParam.CreateElement("storageMega");
            xStorageMega.InnerText = "1";
            xStorage.AppendChild(xStorageMega);
            //storageDayLog
            XmlElement xStorageDayLoga = XmlParam.CreateElement("storageDayLog");
            xStorageDayLoga.InnerText = "20";
            xStorage.AppendChild(xStorageDayLoga);
            //<Options><Minsize></Minsize></Options>
            XmlElement xMinsize = XmlParam.CreateElement("Minsize");
            xOptions.AppendChild(xMinsize);
            //minSizeB
            XmlElement xMinSizeB = XmlParam.CreateElement("minSizeB");
            xMinSizeB.InnerText = "1000";
            xMinsize.AppendChild(xMinSizeB);
            //<Options><Path></Path></Options>
            XmlElement xPath = XmlParam.CreateElement("Path");
            xOptions.AppendChild(xPath);
            //backupDirDay
            XmlElement xBackupDirDay = XmlParam.CreateElement("backupDirDay");
            xBackupDirDay.InnerText = Function.сurrentPath + "/day/";
            xPath.AppendChild(xBackupDirDay);
            if (!Directory.Exists(Function.сurrentPath + "/day"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/day");
            }
            //backupDirMonth
            XmlElement xBackupDirMonth = XmlParam.CreateElement("backupDirMonth");
            xBackupDirMonth.InnerText = Function.сurrentPath + "/month/";
            xPath.AppendChild(xBackupDirMonth);
            if (!Directory.Exists(Function.сurrentPath + "/month"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/month");
            }
            //backupMega
            XmlElement xBackupMega = XmlParam.CreateElement("backupMega");
            xBackupMega.InnerText = Function.сurrentPath + "/Mega/";
            xPath.AppendChild(xBackupMega);
            if (!Directory.Exists(Function.сurrentPath + "/Mega"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/Mega");
            }
            //FullLog
            XmlElement xFullLog = XmlParam.CreateElement("FullLog");
            xFullLog.InnerText = Function.сurrentPath + "/log/FullLog/";
            xPath.AppendChild(xFullLog);
            if (!Directory.Exists(Function.сurrentPath + "/log/FullLog"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/log/FullLog");
            }
            //ShortLog
            XmlElement xShortLog = XmlParam.CreateElement("ShortLog");
            xShortLog.InnerText = Function.сurrentPath + "/log/ShortLog/";
            xPath.AppendChild(xShortLog);
            if (!Directory.Exists(Function.сurrentPath + "/log/ShortLog"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/log/ShortLog");
            }
            //LogReglament
            XmlElement xLogReglament = XmlParam.CreateElement("LogReglament");
            xLogReglament.InnerText = Function.сurrentPath + "/log/ReglamentLog/";
            xPath.AppendChild(xLogReglament);
            if (!Directory.Exists(Function.сurrentPath + "/log/ReglamentLog"))
            {
                Directory.CreateDirectory(Function.сurrentPath + "/log/ReglamentLog");
            }
            //<Options><DataBase></DataBase></Options>
            XmlElement xDataBase = XmlParam.CreateElement("DataBase");
            xOptions.AppendChild(xDataBase);
            //DBnameMas
            Function.GetDBName();
            XmlElement xDBnameMas = XmlParam.CreateElement("DBnameMas");
            xDBnameMas.InnerText = Function.output;
            xDataBase.AppendChild(xDBnameMas);
            XmlParam.Save(Function.param);
        }
        public static void Setparamfoxml()
        {
            if (File.Exists(Function.param))
            {
                XmlDocument XmlParam = new XmlDocument();
                XmlParam.Load(Function.param);
                Function.monthD = Convert.ToDouble(XmlParam.SelectSingleNode("Options/Storage/monthD").InnerText);
                Function.storageDay = Convert.ToDouble(XmlParam.SelectSingleNode("Options/Storage/storageDay").InnerText);
                Function.storageMega = Convert.ToDouble(XmlParam.SelectSingleNode("Options/Storage/storageMega").InnerText);
                Function.storageDayLog = Convert.ToDouble(XmlParam.SelectSingleNode("Options/Storage/monthD").InnerText);
                Function.minSizeB = Convert.ToInt32(XmlParam.SelectSingleNode("Options/Minsize/minSizeB").InnerText);
                Function.backupDirDay = XmlParam.SelectSingleNode("Options/Path/backupDirDay").InnerText;
                Function.backupDirMonth = XmlParam.SelectSingleNode("Options/Path/backupDirMonth").InnerText;
                Function.backupMega = XmlParam.SelectSingleNode("Options/Path/backupMega").InnerText;
                Function.FullLog = XmlParam.SelectSingleNode("Options/Path/FullLog").InnerText;
                Function.ShortLog = XmlParam.SelectSingleNode("Options/Path/ShortLog").InnerText;
                Function.LogReglament = XmlParam.SelectSingleNode("Options/Path/LogReglament").InnerText;
                Function.DBnameMas = XmlParam.SelectSingleNode("Options/DataBase/DBnameMas").InnerText.Split(',');
                Function.Alertbool = XmlParam.SelectSingleNode("Options/Alert/Alertbool").InnerText;
                if (Function.Alertbool == "true")
                {
                    Key = Crypto.Decrypt(XmlParam.SelectSingleNode("Options/Alert/Key").InnerText, Crypto.k1);
                    Function.Alertbool = Crypto.Decrypt(XmlParam.SelectSingleNode("Options/Alert/Alertbool").InnerText, Key);
                    Function.SendEmail = Crypto.Decrypt(XmlParam.SelectSingleNode("Options/Alert/SendEmail").InnerText, Key);
                    Function.SendEmailPass = Crypto.Decrypt(XmlParam.SelectSingleNode("Options/Alert/SendEmailPass").InnerText, Key);
                    Function.GetEmail = Crypto.Decrypt(XmlParam.SelectSingleNode("Options/Alert/GetEmail").InnerText, Key);
                }
            }
            else
            {
                Setdefparamfoxml();        
            }
        }
    }
}