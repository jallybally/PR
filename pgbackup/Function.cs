﻿using System;
using System.Diagnostics;
using System.IO;
using Npgsql;
using System.Collections.Generic;

namespace pgbackup
{
    class Function
    {
        public static string param = Directory.GetCurrentDirectory() + "/param.xml"; //ИЗМЕНИТЬ СЛЕШИ ПЕРЕД ПЕРЕНОСОМ В ЛИНЬ!!!
        //public static string param = Directory.GetCurrentDirectory() + "/param.xml";
        public static string сurrentPath = Directory.GetCurrentDirectory();
        public static string[] dayD1 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };//число первого месячного бекапа
        public static string[] dayD2 = { "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };//число второго месячного бекапа
        public static double monthD;//количество дней хранения месячных бекапов
        public static double storageDay;//количество дней хранения бекапа
        public static double storageMega;//количество хранимых копий в меге
        public static double storageDayLog;//время хранения логов
        public static int minSizeB;//минимальный размер бекапа в байтах
        public static string backupDirDay;//путь к папке в которой лежат бекапы
        public static string backupDirMonth;//путь к папке с месячными бекапами
        public static string backupMega;//путь к папке для синхрона в мегу
        public static string FullLog;//путь к папке с логами
        public static string ShortLog; //путь к кратким логам
        public static string LogReglament;//путь для лога регламентных
        public static string[] DBnameMas;//cписок баз данных
        public static string tempDB; //переменная для еденичных копий
        public static string DBanmeT = null;
        public static string DBname = null;
        public static string output;
        //почта
        public static string SendEmail = null;
        public static string SendEmailPass = null;
        public static string GetEmail = null;
        public static string Key = null;
        public static string Alertbool = null;
        public static string Message = null;
        //
        public static string time = DateTime.Now.ToString("yyyy:MM:dd");
        /*
         * Подключение к СУБД, выполнение запросов
         */
        public static void GetDBName()//получаем список баз данных
        {
            List<String> dbname = new List<String>();
            string connString = String.Format(

            "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Disable",
            "",
            "",
            "",
            "",
            ""
            );
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT datname FROM pg_database;", conn);
                NpgsqlDataReader reader;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    dbname.Add(reader.GetString(0));
                }
                reader.Close();
                string i = String.Join(",", dbname);
                i = i.Replace("postgres,", "");
                i = i.Replace("template1,", "");
                i = i.Replace("template0,", "");
                output = i;
            }
        }
        /*
         * Запуск создания бекапов.
         */
        public static string Backup()
        {
            string LogDB = FullLog + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + ".log";
            string backupFile = backupDirDay + DBname + "_" + DateTime.Now.ToString("yyyy:MM:dd") + "_" + DateTime.Now.ToString("HH:mm:ss") + ".backup";
            string BackupString = "pg_dump -f " + backupFile + " -F c -d " + DBname + " -v";
            string output;
            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + BackupString + " \"";
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            StreamReader reader = proc.StandardError;
            output = reader.ReadToEnd();
            proc.WaitForExit();
            if (backupFile.Length < minSizeB)
            {
                File.AppendAllText(ShortLog + DBname + ".log", DateTime.Now.ToString("yyyy:MM:dd") + " Бекап выполнен успешно.\r\n");
            }
            else
            {
                File.AppendAllText(ShortLog + DBname + ".log", DateTime.Now.ToString("yyyy:MM:dd") + " Бекап не выполнен. \r\n");
            }
            proc.Close();
            File.AppendAllText(LogDB, output);
            if (output.Contains($"pg_dump: dumping contents of table \"public.widget_field\""))
            {
                Message += $"{DateTime.Now.ToString("yyyy:MM:dd")}: Копия базы {DBname} выполнена успешно.\n";
            }
            else
            {
                Message += $"{DateTime.Now.ToString("yyyy:MM:dd:HH:mm")}: Копия базы {DBname} не выполнена.\n";
            }
            Reglament();
            return backupFile;
        }
        /*
         * Запуск регламентных заданий
         */
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
        /*
         * Сортировка бекапов
         */
        public static void Sortbackup()
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
                if (DateTime.Now == creatTime)
                {
                    File.Copy(fi.FullName, backupMega + fi.Name);
                }
            }
            foreach (var fi in new DirectoryInfo(backupMega).EnumerateFiles("*.backup", SearchOption.AllDirectories))
            {
                string creatDay = fi.CreationTime.Day.ToString();//день создания файла дневного
                DateTime creatTime = fi.CreationTime;//дата создания файла
                if (DateTime.Now - creatTime > TimeSpan.FromDays(storageMega) || fi.Length < minSizeB) //если прошло больше 20 дней с момента создания
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
        /*
         * Синхронизация с мегой
         */
        public static void CopyToMega()
        {
            if (Convert.ToString(storageMega) == "0")
            {

            }
            else
            {               
                Process mega = new Process();
                mega.StartInfo.FileName = "/bin/bash";
                mega.StartInfo.Arguments = "mega-sync " + backupMega + " /MEGAsync/1CBackup"; //тянуть из параметров
                mega.StartInfo.RedirectStandardError = false;
                mega.StartInfo.RedirectStandardOutput = false;
                mega.Start();
                mega.WaitForExit();
                mega.Close();
            }
        }
        /*
         * Удаление старых логов
         */
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
        /*
         * Бекап руками
         */
        public static void BackupDataBase()
        {
            
            string backuppath;
            GetDBName();
            Console.WriteLine("Список доступных баз: " + output + "\nВведите название баз для резервного копирования:\n");
            tempDB = Console.ReadLine();
            backuppath = сurrentPath + "/" + tempDB + "/" + time + ".backup";
            string i = сurrentPath + "/" + tempDB + "/";
            string[] ii = tempDB.Split(',');
            if (!Directory.Exists(i))
            {
                Directory.CreateDirectory(i);
                foreach (string db in ii)
                {
                    DBanmeT = db;
                    string backuppatht = сurrentPath + "/" + DBanmeT + "/" + time + ".backup";
                    string BackupString = "pg_dump -f " + backuppatht + "  -F c -d " + DBanmeT + " -v";
                    Process proc = new Process();
                    proc.StartInfo.FileName = "/bin/bash";
                    proc.StartInfo.Arguments = "-c \" " + BackupString + " \"";
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();
                    StreamReader reader = proc.StandardError;
                    proc.WaitForExit();
                    proc.Close();
                    Console.WriteLine("Копия сделана");
                }
                Console.WriteLine("Путь к копии: " + backuppath);
            }
            else
            {
                foreach (string db in ii)
                {
                    DBanmeT = db;
                    string backuppatht = сurrentPath + "/" + DBanmeT + "/" + time + ".backup";
                    string BackupString = "pg_dump -f " + backuppatht + "  -F c -d " + DBanmeT + " -v";
                    Process proc = new Process();
                    proc.StartInfo.FileName = "/bin/bash";
                    proc.StartInfo.Arguments = "-c \" " + BackupString + " \"";
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();
                    StreamReader reader = proc.StandardError;
                    proc.WaitForExit();
                    proc.Close();
                    Console.WriteLine("Копия сделана");
                }
                Console.WriteLine("Путь к копии: " + backuppath);
            }
        }
        /*
         * Регламентные руками
         */
        public static void ReglamentOne()
        {
            GetDBName();
            foreach (string db in Function.DBnameMas)
            {
                DBanmeT = db;
                Console.WriteLine("Список доступных баз: " + output + "\nВведите название баз для резервного копирования:\n");
                tempDB = Console.ReadLine();
                string parameterV = "vacuumdb " + DBanmeT + " -f -z -v";
                string parameterR = "reindexdb " + DBanmeT + " -v";
                Process rein = new Process();
                Process vac = new Process();
                vac.StartInfo.FileName = "/bin/bash";
                vac.StartInfo.Arguments = "-c \" " + parameterV + " \"";
                vac.StartInfo.RedirectStandardError = true;
                vac.Start();
                vac.WaitForExit();
                vac.Close();
                rein.StartInfo.FileName = "/bin/bash";
                rein.StartInfo.Arguments = "-c \" " + parameterR + " \"";
                rein.StartInfo.RedirectStandardError = true;
                rein.Start();
                rein.WaitForExit();
                rein.Close();
            }
        }
    }
}