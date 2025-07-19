using KafkaConsumer.DataModels;
using System;

namespace KafkaConsumer.Interfaces
{
    public interface IDummyService
    {
        public ReturnCode DummyProcessor(DummyModel dummyModel, out DummyOutput output);
    }
}
