using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.ModelWeb
{
    public class TimerManager
    {
        private Timer timer;
        private AutoResetEvent autoResetEvent;
        private Action action;

        public DateTime timeStart { get; set; }

        public TimerManager(Action action)
        {
            this.action = action;
            autoResetEvent = new AutoResetEvent(false);
            timer = new Timer(Excute, autoResetEvent, 5  * 1000, 5 * 1000);
            timeStart = DateTime.Now;
        }
        public void Excute(Object stateInfo)
        {
            action();
            if ((DateTime.Now - timeStart).Seconds > 60)
            {
                timer.Dispose();
            }
        }
    }
}
