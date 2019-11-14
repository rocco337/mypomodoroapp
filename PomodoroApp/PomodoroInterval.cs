using System;

namespace PomodoroApp
{
    public class PomodoroInterval
    {
        public int SessionIndex { get; set; }
        public Guid Id = Guid.NewGuid();
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; set; }
        public IntervalType Type { get; }
        public DateTime CountDown { get; set; }
        public string Comment { get; set; }
        public PomodoroInterval(DateTime startTime, IntervalType type, int sessionIndex)
        {
            StartTime = startTime;
            Type = type;
            SessionIndex = sessionIndex;
        }
    }
}
