using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PomodoroApp
{
    //todo - for each work in session add number, reset it after large break
    //todo - autocontinue or requires user interaction to start new session
    //todo - list - from today?
    //todo - add comment to work(like task)
    public class PomodoroApp
    {
        /// <summary>
        /// Holds infinite timer 
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Count downs timer(work, short break, long break)
        /// </summary>
        private DateTime? _timerCountDown;

        private IntervalChangesListener _changesQueue;

        private IntervalTypeDetector _intervalTypeDetector = new IntervalTypeDetector();
        private int _updateIntervalMs = 500;
        private int _workIntervalDuration, _pauseIntervalDuration, _longPauseIntervalDuration;


        private PomodoroInterval _interval;

        private Action<PomodoroInterval> _onTick;

        public PomodoroApp(int workInterval, int shortPauseInterval, Action<PomodoroInterval> onTick)
        {
            this._workIntervalDuration = workInterval;
            this._pauseIntervalDuration = shortPauseInterval;
            this._longPauseIntervalDuration = shortPauseInterval * 3;

            this._changesQueue = new IntervalChangesListener();
            this._onTick = onTick;
        }

        public void Start(IntervalType type = IntervalType.Work)
        {
            var callback = new TimerCallback(TimerCallback);
            this._timer = new Timer(callback, null, 0, Timeout.Infinite);
        }

        public void Stop()
        {
            this._timer.Dispose();
            this._interval.EndTime = DateTime.Now;
            _changesQueue.Enqueue(this._interval);

            this._interval = null;
        }

        private DateTime? LastTickTime;
        private void TimerCallback(object state)
        {
            if (LastTickTime == null)
            {
                LastTickTime = DateTime.Now;
            }

            if (this._interval == null)
            {
                this._interval = new PomodoroInterval(DateTime.Now, _intervalTypeDetector.GetNext(IntervalType.ShortBreak));
                _changesQueue.Enqueue(this._interval);
            }

            if (_timerCountDown == null)
            {
                var intervalDuration = GetIntervalDuration(this._interval.Type);
                this._timerCountDown = DateTimeExtensions.GetTimeInterval(intervalDuration);
                this._interval.CurrentTime = this._timerCountDown.Value;
            }

            if (_timerCountDown.Value.IsIntervalFinished())
            {
                //start new interval
                this._interval.EndTime = DateTime.Now;
                _changesQueue.Enqueue(this._interval);

                this._interval = new PomodoroInterval(DateTime.Now, _intervalTypeDetector.GetNext(this._interval.Type));
                _changesQueue.Enqueue(this._interval);

                var intervalDuration = GetIntervalDuration(this._interval.Type);
                this._timerCountDown = DateTimeExtensions.GetTimeInterval(intervalDuration);
            }
            else
            {
                _timerCountDown = _timerCountDown.Value.AddMilliseconds(-(DateTime.Now - LastTickTime.Value).TotalMilliseconds);
                this._interval.CurrentTime = _timerCountDown.Value;
            }

            //notify subscribers
            this._onTick(this._interval);
            this._timer.Change(_updateIntervalMs, Timeout.Infinite);
            LastTickTime = DateTime.Now;
        }
        
        private int GetIntervalDuration(IntervalType? intervalType = null)
        {
            if (intervalType == null)
            {
                return _workIntervalDuration;
            }

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
            return new DateTime(1, 1, 1, 0, intervalMinutes, 0, 0);
        }

        public static bool IsIntervalFinished(this DateTime interval)
        {
            return interval.Minute <= 0 && interval.Second <= 0;
        }
    }

    public class PomodoroInterval
    {
        public Guid Id = Guid.NewGuid();
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; set; }
        public IntervalType Type { get; }
        public DateTime CurrentTime { get; set; }

        public PomodoroInterval(DateTime startTime, IntervalType type)
        {
            StartTime = startTime;
            Type = type;
        }
    }

    public enum IntervalType
    {
        None,
        Work,
        ShortBreak,
        LongBreak,
        End
    }

    public class IntervalChangesListener
    {
        private PomodoroRepository _repository = new PomodoroRepository();

        private Queue<PomodoroInterval> _queue = new Queue<PomodoroInterval>();

        public IntervalChangesListener()
        {
        }

        public void Enqueue(PomodoroInterval item)
        {
            _queue.Enqueue(item);
            _repository.SaveOrUpdate(item);
        }
    }
}
