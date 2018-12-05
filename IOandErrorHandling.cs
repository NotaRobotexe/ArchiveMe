using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Data.SQLite;

namespace ArchiveMe
{
    class IOandErrorHandling
    {
        public static void Menu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press 1 for thread scrapping:");
            Console.WriteLine("Press 2 to display archived threads:");
        }

        public static bool CheckIfDatabaseExist(string board, string thread)
        {
            DirectoryInfo dir = new DirectoryInfo("Database");
            FileInfo[] files = dir.GetFiles("*.db");

            foreach (var file in files)
            {
                if (board == file.Name.Substring(0, file.Name.IndexOf('.')))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckIfTableExist(string board, string thread)
        {
            SQLiteConnection sql_dat = new SQLiteConnection("Data Source=Database\\" + board.ToString() + ".db");
            sql_dat.Open();
            string sql = "SELECT * from op" + thread;
            SQLiteCommand sql_com = new SQLiteCommand(sql, sql_dat);

            try
            {
                using (SQLiteDataReader reader = sql_com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                    }
                }
            }
            catch
            {
                sql_dat.Close();
                return false;
            }
            sql_dat.Close();
            return true;
        }
    

        public static void Quit()
        {
            while (true)
            {
                string inp = Console.ReadLine();
                if (inp == "quit")
                {
                    break;
                }
            }
        }

        public static void ThreadScrappingLine()
        {
            Console.Write("Set board (g,fit,diy,...): ");
        }

        public static bool CheckUrl(string board, string thread)
        {
            try
            {
                var cliet = new WebClient();
                cliet.DownloadString("https://a.4cdn.org/" + board + "/thread/" + thread + ".json");
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
