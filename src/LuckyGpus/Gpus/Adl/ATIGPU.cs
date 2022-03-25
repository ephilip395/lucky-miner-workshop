﻿namespace Lucky.Gpus.Adl {
    public class ATIGPU {
        public static readonly ATIGPU Empty = new ATIGPU {
            AdapterIndex = -1,
            BusNumber = -1,
            AdapterName = string.Empty,
            DeviceNumber = -1,
            OverdriveVersion = -1,
            MemoryTimingLevels = new int[0],
            CurrentMemoryTimingLevel = -1,
            DefaultMemoryTimingLevel = -1
        };

        public ATIGPU() {
            this.MemoryTimingLevels = new int[0];
        }

        public int AdapterIndex { get; set; }
        public int BusNumber { get; set; }
        public int DeviceNumber { get; set; }
        public string AdapterName { get; set; }
        public int OverdriveVersion { get; set; }
        public int GpuLevels { get; set; }
        public int MemoryLevels { get; set; }
        /// <summary>
        /// iMaximumNumberOfPerformanceLevels
        /// </summary>
        public int MaxLevels { get; set; }

        public int CoreClockMin { get; set; }
        public int CoreClockMax { get; set; }
        public int CoreClockDefault { get; set; }
        public int MemoryClockMin { get; set; }
        public int MemoryClockMax { get; set; }
        public int MemoryClockDefault { get; set; }
        public int PowerMin { get; set; }
        public int PowerMax { get; set; }
        public int PowerDefault { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitMax { get; set; }
        public int TempLimitDefault { get; set; }
        public int VoltMin { get; set; }
        public int VoltMax { get; set; }
        public int VoltDefault { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public int FanSpeedMin { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public int FanSpeedMax { get; set; }

        internal ADLOD8InitSetting ADLOD8InitSetting { get; set; }
        public int[] MemoryTimingLevels { get; set; }
        public int CurrentMemoryTimingLevel { get; set; }
        internal int DefaultMemoryTimingLevel { get; set; }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }
}
