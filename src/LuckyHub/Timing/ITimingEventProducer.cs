﻿namespace Lucky.Timing {
    // 它的生命周期和App相同所以不需要终结
    public interface ITimingEventProducer {
        void Start();
    }
}
