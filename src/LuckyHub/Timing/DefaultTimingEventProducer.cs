using Lucky.Hub;
using System.Timers;

namespace Lucky.Timing
{
    public class DefaultTimingEventProducer : AbstractTimingEventProducer
    {
        public DefaultTimingEventProducer(IMessagePathHub hub) : base(hub) { }

        private Timer _timer;
        public override void Start()
        {
            if (_timer != null)
            {
                return;
            }
            _timer = new Timer(1000);
            _timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                Elapsed();
            };
            _timer.Start();
        }
    }
}
