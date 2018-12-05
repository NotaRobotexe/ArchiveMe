using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace ArchiveMe
{
    class Program
    {
        static void Main(string[] args)
        {
            string board;
            string thread_id; 
            IOandErrorHandling.Menu();
            string inp = Console.ReadLine();
            if (inp == "1")
            {
                Console.Write("Set board (g,fit,diy,...): ");
                board = Console.ReadLine();
                Console.Write("Enter thread ID: ");
                thread_id = Console.ReadLine();

                if(IOandErrorHandling.CheckUrl(board, thread_id))
                {
                    Console.WriteLine("Scrapping old posts...");
                    API api = new API();
                    api.Thread_scrapper(board, thread_id);

                    IOandErrorHandling.Quit();
                    api.Quit();
                }
                else
                {
                    Console.WriteLine("Wrong board or thread ID.");
                    Console.ResetColor();
                }
            }
            else if(inp == "2")
            {
                Database.GetAllDatabasesAndTables();
                Console.Write("\nSelect board: ");
                board = Console.ReadLine();
                Console.Write("Select Thread ID: ");
                thread_id = Console.ReadLine();

                
                if (IOandErrorHandling.CheckIfDatabaseExist(board, thread_id)  && IOandErrorHandling.CheckIfTableExist(board, thread_id))
                {
                    Visualise visualise = new Visualise(board, thread_id);
                }
                else
                {
                    Console.WriteLine("Wrong board or thread ID.");
                }

            }
            else
            {
                Console.WriteLine("Wrong input.");
                Console.ResetColor();
            }
        }
    }
}
