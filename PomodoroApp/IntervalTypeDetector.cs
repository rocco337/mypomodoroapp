using System;
using System.Collections.Generic;
using System.Linq;

namespace PomodoroApp
{
    public class IntervalTypeDetector
    {
        public IntervalTypeDetector(PomodoroRepository pomodoroRepository)
        {
            PomodoroRepository = pomodoroRepository;
        }

        public PomodoroRepository PomodoroRepository { get; }

        public IntervalType GetNext(IntervalType current, int numberOfWorkIntervalsBeforeLongBreak)
        {
            var skip = numberOfWorkIntervalsBeforeLongBreak + (numberOfWorkIntervalsBeforeLongBreak - 1);
            var next = IntervalType.Work;
            
            var sessions = PomodoroRepository.GetLastNSessions(skip).ToList();
            
            //if there was no long-break and was exactly N work sessions, than is time to do long break
            if (!sessions.Any(m => m.Type == (int)IntervalType.LongBreak) && sessions.Count(m => m.Type == (int)IntervalType.Work) >= numberOfWorkIntervalsBeforeLongBreak)
            {
                next = IntervalType.LongBreak;
            }
            else
            {
                if(current== IntervalType.Work)
                {
                    next = IntervalType.ShortBreak;
                }                
            }

            return next;
        }
    }
}
