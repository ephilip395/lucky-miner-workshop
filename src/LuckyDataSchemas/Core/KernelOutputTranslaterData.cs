﻿using System;

namespace Lucky.Core {
    public class KernelOutputTranslaterData : IKernelOutputTranslater, IDbEntity<Guid>, ISortable {
        public KernelOutputTranslaterData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid KernelOutputId { get; set; }

        public string RegexPattern { get; set; }

        public string Replacement { get; set; }

        public int SortNumber { get; set; }

        public bool IsPre { get; set; }
    }
}
