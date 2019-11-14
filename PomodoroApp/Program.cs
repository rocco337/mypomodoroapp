using System;
using System.IO;

namespace PomodoroApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            DbInit();
            if (args.Length > 0 && args[0] == "start")
            {
                var workInterval = Int32.Parse(args[1]);
                var pauseInterval = Int32.Parse(args[2]);
              
                Console.Clear();
                Console.CursorVisible = false;

                var pomodoro = new PomodoroApp(workInterval, pauseInterval, (interval) =>
                {
                    Console.WriteLine($"Time left({interval.Type}): { interval.CountDown.ToString("mm:ss")}");
                    Console.SetCursorPosition(0, 0);
                }, (interval, nextIntervalType) =>{ });

                pomodoro.Start();

                Console.ReadLine();
            }
            else if (args.Length > 0 && args[0] == "list")
            {
                foreach(var session in new PomodoroRepository().GetSessions())
                {
                    Console.WriteLine($"{session.Id}, {session.StartTime.ToShortDateString()} {session.StartTime.ToShortTimeString()} -  {session.EndTime?.ToShortTimeString()}, {((IntervalType)session.Type).ToString()}, {session.Comment}");
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Pomodoro App!");
                Console.WriteLine();
                Console.WriteLine("1. Usage: Pomodoro.exe start 25 5");
                Console.WriteLine("     25 - working time is 25 min, 5 break time is 5 min");
                Console.WriteLine("2. Usage: Pomodoro.exe list");
            }
        }

        public static void DbInit()
        {
            using (var dbContext = new PomodoroDbContext())
            {
                //Ensure database is created
                dbContext.Database.EnsureCreated();                
            }
        }       
    }
   
}
