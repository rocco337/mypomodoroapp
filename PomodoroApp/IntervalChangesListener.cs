//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace PomodoroApp
//{
//    public class IntervalChangesListener
//    {
//        private PomodoroRepository _repository = new PomodoroRepository();

//        private Queue<PomodoroInterval> _queue = new Queue<PomodoroInterval>();

//        public IntervalChangesListener()
//        {
//        }

//        public void Enqueue(PomodoroInterval item)
//        {
//            _queue.Enqueue(item);
//            _repository.SaveOrUpdate(item);
//        }
        
//    }
//}
