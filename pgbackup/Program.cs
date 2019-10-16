using System;
using System.Diagnostics;
using System.IO;

namespace pgbackup
{
    class Program
    {
        public static string[] dayD1 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };//число первого месячного бекапа
        public static string[] dayD2 = { "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };//число второго месячного бекапа
        public static double monthD = 320;//количество дней хранения месячных бекапов
        public static double storageDay = 20;//количество дней хранения бекапа
        public static double storageDayLog = 20;//время хранения логов
        public static int minSizeB = 1000;//минимальный размер бекапа в байтах
        public static string backupDirDay = "/mnt/1CBackup/day/";//путь к папке в которой лежат бекапы
        public static string backupDirMonth = "/mnt/1CBackup/month/";//путь к папке с месячными бекапами
        public static string FullLog = "/mnt/1CBackup/log/FullLog/";//путь к папке с логами
        public static string ShortLog = "/mnt/1CBackup/log/ShortLog/"; //путь к кратким логам
        public static string LogReglament = "/mnt/1CBackup/log/ReglamentLog/";//путь для лога регламентных
        public static string[] DBnameMas = new string[] { "RT", "UT11_new", "ZP5H", "ZP5U", "accounting" };//cписок баз данных
        //public static string[] DBnameMas = new string[] { "torgoviy_dom" };//cписок баз данных
        public static string DBname = null;

        static void Main(string[] args)
        {
            Start();
        }
        static void Start() //Функция для подготовки запуска, получение путей, расчёт переменных
        {
            foreach (string db in DBnameMas)
            {
                DBname = db;
                Backup();
            }
            Sortbackup();
            DelLog();
        }
        public static string Backup()
        {
            string LogDB = FullLog + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + ".log";
            string backupFile = backupDirDay + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + ".backup";
            string BackupString = "pg_dump -f " + backupFile + "  -F c -d " + DBname + " -v";
            string output;
            Process proc = new Process();
            Stopwatch stw = new Stopwatch();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + BackupString + " \"";
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            StreamReader reader = proc.StandardError;
            output = reader.ReadToEnd();
            proc.WaitForExit();
            TimeSpan ts = stw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                 ts.Hours, ts.Minutes, ts.Seconds,
                 ts.Milliseconds / 10);
            if (backupFile.Length < minSizeB)
            {
                File.AppendAllText(ShortLog + DBname + ".log", DateTime.Now.ToString("yyyy:MM:dd") + " Бекап выполнен успешно. Время выполнения: " + elapsedTime + "\r\n"); ;
            }
            else
            {
                File.AppendAllText(ShortLog + DBname + ".log", DateTime.Now.ToString("yyyy:MM:dd") + " Бекап не выполнен \r\n");
            }
            proc.Close();
            File.AppendAllText(LogDB, output);
            Reglament();
            return backupFile;
        }
        static void Sortbackup()
        {
            string currDay = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString();
            bool d1 = false;
            bool d2 = false;
            foreach (var fi in new DirectoryInfo(backupDirDay).EnumerateFiles("*.backup", SearchOption.AllDirectories))
            {
                string creatDay = fi.CreationTime.Day.ToString();//день создания файла дневного
                DateTime creatTime = fi.CreationTime;//дата создания файла
                if (DateTime.Now - creatTime > TimeSpan.FromDays(storageDay) || fi.Length < minSizeB) //если прошло больше 20 дней с момента создания
                {
                    fi.Delete();
                }
            }
            foreach (var fi in new DirectoryInfo(backupDirMonth).EnumerateFiles("*.backup", SearchOption.AllDirectories))
            {
                string creatDay = fi.CreationTime.Day.ToString();//день создания файла дневного
                DateTime creatTime = fi.CreationTime;//дата создания файла
                if (DateTime.Now - creatTime > TimeSpan.FromDays(monthD) || fi.Length < minSizeB) //если прошло больше 20 дней с момента создания
                {
                    fi.Delete();
                }
            }
            foreach (var fim in new DirectoryInfo(backupDirMonth).EnumerateFiles("*.backup", SearchOption.AllDirectories))
            {
                string creatDay = fim.CreationTime.Day.ToString() + fim.CreationTime.Month.ToString();
                if (d1 == false)
                {
                    foreach (var day in dayD1)
                    {
                        string daym = day + DateTime.Now.Month.ToString();
                        if (daym == creatDay)
                        {
                            d1 = true;
                            break;
                        }
                        else
                        {
                            d1 = false;
                        }
                    }
                }
                if (d1 == true)
                {
                    break;
                }
            }
            foreach (var fim in new DirectoryInfo(backupDirMonth).EnumerateFiles("*.backup", SearchOption.AllDirectories))
            {
                string creatDay = fim.CreationTime.Day.ToString() + fim.CreationTime.Month.ToString();
                if (d2 == false)
                {
                    foreach (var day in dayD2)
                    {
                        string daym = day + DateTime.Now.Month.ToString();
                        if (daym == creatDay)
                        {
                            d2 = true;
                            break;
                        }
                        else
                        {
                            d2 = false;
                        }
                    }
                }
                if (d2 == true)
                {
                    break;
                }
            }
            if (d1 == true && d2 == true)
            {
                //Console.WriteLine("2 месячных бекапа есть");
            }
            else if (d1 == false && d2 == false)
            {
                //Console.WriteLine("Месячных бекапов нет");
                foreach (var fi in new DirectoryInfo(backupDirDay).EnumerateFiles("*.backup", SearchOption.AllDirectories))
                {
                    string creatTime = fi.CreationTime.Day.ToString() + fi.CreationTime.Month.ToString();
                    if (creatTime == currDay)
                    {
                        File.Copy(fi.FullName, backupDirMonth + fi.Name);
                    }
                }
            }
            else if (d1 == false && d2 == true)
            {
                // Console.WriteLine("Есть бекап из 2 пула");
                foreach (var day in dayD1)
                {
                    string daym = day + DateTime.Now.Month.ToString();
                    foreach (var fi in new DirectoryInfo(backupDirDay).EnumerateFiles("*.backup", SearchOption.AllDirectories))
                    {
                        string creatTime = fi.CreationTime.Day.ToString() + fi.CreationTime.Month.ToString();
                        if (creatTime == daym)
                        {
                            File.Copy(fi.FullName, backupDirMonth + fi.Name);
                        }
                    }
                }
            }
            else if (d1 == true && d2 == false)
            {
                //Console.WriteLine("Есть бекапы из 1 пула");
                foreach (var day in dayD2)
                {
                    string daym = day + DateTime.Now.Month.ToString();
                    foreach (var fi in new DirectoryInfo(backupDirDay).EnumerateFiles("*.backup", SearchOption.AllDirectories))
                    {
                        string creatTime = fi.CreationTime.Day.ToString() + fi.CreationTime.Month.ToString();
                        if (creatTime == daym)
                        {
                            File.Copy(fi.FullName, backupDirMonth + fi.Name);
                        }
                    }
                }
            }
        }
        public static void DelLog()
        {
            foreach (var fi in new DirectoryInfo(FullLog).EnumerateFiles("*.log", SearchOption.AllDirectories))
            {
                string creatDay = fi.CreationTime.Day.ToString();
                DateTime creatTime = fi.CreationTime;
                if (DateTime.Now - creatTime > TimeSpan.FromDays(storageDayLog))
                {
                    fi.Delete();
                }
            }
        }
        public static void Reglament()
        {
            string LogRegVACUMDB = LogReglament + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + "VACUMDB.log";
            string LogRegREINDEXDB = LogReglament + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + "REINDEXDB.log";
            string parameterV = "vacuumdb " + DBname + " -f -z -v";
            string parameterR = "reindexdb " + DBname + " -v";
            string outputV;
            string outputR;
            Process rein = new Process();
            Process vac = new Process();
            vac.StartInfo.FileName = "/bin/bash";
            vac.StartInfo.Arguments = "-c \" " + parameterV + " \"";
            vac.StartInfo.RedirectStandardError = true;
            vac.Start();
            StreamReader readerV = vac.StandardError;
            outputV = readerV.ReadToEnd();
            vac.WaitForExit();
            vac.Close();
            rein.StartInfo.FileName = "/bin/bash";
            rein.StartInfo.Arguments = "-c \" " + parameterR + " \"";
            rein.StartInfo.RedirectStandardError = true;
            rein.Start();
            StreamReader readerR = rein.StandardError;
            outputR = readerR.ReadToEnd();
            rein.WaitForExit();
            rein.Close();
            File.AppendAllText(LogRegVACUMDB, outputV);
            File.AppendAllText(LogRegREINDEXDB, outputR);
        }
    }
}
