using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PomodoroApp
{
    public class PomodoroRepository
    {
        public IList<PomodoroSessionEntity> GetSessions(DateTime? date = null)
        {
            using (var db = new PomodoroDbContext())
            {
                var sessions = db.Pomodoros.AsQueryable();

                if (date!= null)
                {
                    sessions = sessions.Where(m => m.StartTime.Date == date.Value.Date);
                }

                return sessions.ToList();
            }
        }
             
        public void SaveOrUpdate(PomodoroInterval interval)
        {
            using (var dbContext = new PomodoroDbContext())
            {
                var entity = dbContext.Pomodoros.FirstOrDefault(m => m.Id.Equals(interval.Id.ToString()));
                if (entity == null)
                {
                    dbContext.Pomodoros.Add(new PomodoroSessionEntity()
                    {
                        Id = interval.Id.ToString(),
                        StartTime = interval.StartTime,
                        EndTime = interval.EndTime,
                        Type = (int)interval.Type
                    });
                }
                else
                {
                    entity.EndTime = interval.EndTime;
                }

                dbContext.SaveChanges();

            }
        }
    }
}
