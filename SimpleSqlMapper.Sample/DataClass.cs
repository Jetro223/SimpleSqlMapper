using System;

namespace SimpleSqlMapper.Sample
{
    public class DataClass
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public DateTime DateValue { get; set; }
        public bool BitValue { get; set; }
        public decimal DecimalValue { get; set; }

        public override string ToString()
        {
            return $"IntValue: {IntValue} StringValue: {StringValue} DateValue: {DateValue} BitValue: {BitValue} DecimalValue: {DecimalValue}";
        }
    }
}