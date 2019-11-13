using System;
using System.Collections.Generic;
using System.Linq;

namespace PomodoroApp
{
    public class IntervalTypeDetector
    {
        static List<IntervalType> _types = new List<IntervalType>();

        public IntervalType GetNext(IntervalType current)
        {
            IntervalType next = IntervalType.None;

            var numberOfWorkIntervalsBeforeLongBreak = 5;
            var skip = numberOfWorkIntervalsBeforeLongBreak + (numberOfWorkIntervalsBeforeLongBreak - 1);
            if (_types.Count >= skip)
            {
                var lastWorkItems = _types.GetRange(_types.Count() - skip, skip).Where(m => m == IntervalType.Work);
                if (lastWorkItems.Count() == numberOfWorkIntervalsBeforeLongBreak)
                {
                    next = IntervalType.LongBreak;
                }
            }
            if(next == IntervalType.None)
            {
                switch (current)
                {
                    case IntervalType.Work:
                        next = IntervalType.ShortBreak;
                        break;
                    case IntervalType.LongBreak:
                    case IntervalType.ShortBreak:
                        next = IntervalType.Work;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            _types.Add(next);
            return next;
        }
    }
}
