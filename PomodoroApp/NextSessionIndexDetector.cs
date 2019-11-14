using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace PomodoroApp
{
    public class NextSessionIndexDetector
    {
        public PomodoroRepository PomodoroRepository { get; }

        public NextSessionIndexDetector(PomodoroRepository pomodoroRepository)
        {
            PomodoroRepository = pomodoroRepository;
        }

        public int GetNextSessionIndex(int maxSessions)
        {
            var toSkip = maxSessions + (maxSessions - 1);
            var list = PomodoroRepository.GetLastNSessions(toSkip).ToList();
            
            //if there was long break, find index of long break, and count work items until long break
            var longBreak = list.FirstOrDefault(m => m.Type == (int)IntervalType.LongBreak);
            if (longBreak !=null)
            {
                var longBreakIndex = list.IndexOf(longBreak);
                list = list.GetRange(0, longBreakIndex);
            }
           
            var nextIndex = list.Where(m => m.Type == (int)IntervalType.Work).Count() + 1;
            return nextIndex;
        }
    }
}
