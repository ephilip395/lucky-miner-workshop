﻿using LiteDB;

namespace Lucky.Core {
    public class OperationResultData : OperationResultDto, IOperationResult {
        public static OperationResultData Create(OperationResultDto data) {
            return new OperationResultData {
                StateCode = data.StateCode,
                ReasonPhrase = data.ReasonPhrase,
                Timestamp = data.Timestamp,
                Description = data.Description
            };
        }

        public OperationResultData() {
            this.Id = LiteDB.ObjectId.NewObjectId();
        }

        [BsonId]
        public ObjectId Id { get; set; }
    }
}
