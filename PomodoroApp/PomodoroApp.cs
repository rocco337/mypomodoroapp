using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PomodoroApp
{
    //todo - list - from today?
    //todo add notification
    public class PomodoroApp
    {
        /// <summary>
        /// Holds infinite timer 
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Count downs timer(work, short break, long break)
        /// </summary>
        //private DateTime? _timerCountDown;
        private PomodoroInterval _interval;

        private PomodoroRepository _repository = new PomodoroRepository();

        private IntervalTypeDetector _intervalTypeDetector = new IntervalTypeDetector(new PomodoroRepository());
        private NextSessionIndexDetector _nextSessionIndexDetector = new NextSessionIndexDetector(new PomodoroRepository());

        private int _updateIntervalMs = 100;
        private int _workIntervalDuration, _pauseIntervalDuration, _longPauseIntervalDuration;


        private Action<PomodoroInterval> _onTick;
        private Action<PomodoroInterval, IntervalType> _onIntervalFinished;

        private DateTime? _lastTickTime;
        
        public int NumberOfWorkIntervalsBeforeLongBreak => 5;

        public PomodoroApp(int workInterval, int shortPauseInterval, Action<PomodoroInterval> onTick, Action<PomodoroInterval, IntervalType> onIntervalFinished)
        {
            this._workIntervalDuration = workInterval;
            this._pauseIntervalDuration = shortPauseInterval;
            this._longPauseIntervalDuration = shortPauseInterval * 3;

            this._onTick = onTick;
            this._onIntervalFinished = onIntervalFinished;
        }

        public void Start()
        {
            var callback = new TimerCallback(TimerCallback);
            this._interval = SetNewInterval(null, IntervalType.Work);
            this._timer = new Timer(callback, null, 0, Timeout.Infinite);
        }

        public void Stop(PomodoroInterval interval)
        {
            CloseInterval(interval);
            ClearTimer();
        }

        private void TimerCallback(object state)
        {
            _lastTickTime = _lastTickTime ?? DateTime.Now;
                        
            if (_interval.CountDown.IsIntervalFinished())
            {
                //close old interval
                CloseInterval(_interval);

                //has to be called after previous has finished
                var nextInterval = _intervalTypeDetector.GetNext(_interval.Type, NumberOfWorkIntervalsBeforeLongBreak);
                
                //notify client that interval finished
                _onIntervalFinished(_interval, nextInterval);

                //if new work interval should start, break everything and wait until user manually starts it
                if (nextInterval == IntervalType.Work)
                {
                    ClearTimer();
                    return;
                }
                else
                {
                    _interval = SetNewInterval(_interval, nextInterval);                   
                }
            }
            else {
                _interval.CountDown = _interval.CountDown.AddMilliseconds(-(DateTime.Now - _lastTickTime.Value).TotalMilliseconds);            
            }

            //notify subscribers
            _onTick(_interval);
            
            _timer.Change(_updateIntervalMs, Timeout.Infinite);
            _lastTickTime = DateTime.Now;
        }

        private PomodoroInterval SetNewInterval(PomodoroInterval currentInterval, IntervalType nextInterval)
        {
            int nextSessionIndex;

            if (currentInterval != null && nextInterval != IntervalType.Work)
            {
                nextSessionIndex = currentInterval.SessionIndex;
            }
            else
            {
                nextSessionIndex = _nextSessionIndexDetector.GetNextSessionIndex(this.NumberOfWorkIntervalsBeforeLongBreak);                
            }

            var interval = new PomodoroInterval(DateTime.Now, nextInterval, nextSessionIndex);
            interval.CountDown = DateTimeExtensions.GetTimeInterval(GetIntervalDuration(nextInterval));

            _repository.SaveOrUpdate(interval);
            return interval;
        }

        private void CloseInterval(PomodoroInterval interval)
        {
            interval.EndTime = DateTime.Now;
            _repository.SaveOrUpdate(interval);
        }

        private void ClearTimer()
        {
            this._timer.Dispose();
            this._lastTickTime = null;
        }

        private int GetIntervalDuration(IntervalType intervalType)
        {
            switch (intervalType)
            {
                case IntervalType.Work:
                    return _workIntervalDuration;
                case IntervalType.ShortBreak:
                    return _pauseIntervalDuration;
                case IntervalType.LongBreak:
                    return _longPauseIntervalDuration;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime GetTimeInterval(int intervalMinutes)
        {
            //return new DateTime(1, 1, 1, 0, 0, 3, 0);

            return new DateTime(1, 1, 1, 0, intervalMinutes, 0, 0);
        }

        public static bool IsIntervalFinished(this DateTime interval)
        {
            return interval.Minute <= 0 && interval.Second <= 0;
        }
    }

    public enum IntervalType
    {
        None,
        Work,
        ShortBreak,
        LongBreak,
    }
}
