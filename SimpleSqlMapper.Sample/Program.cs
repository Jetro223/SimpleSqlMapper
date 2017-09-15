using System;
using System.Data;

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

    class Program
    {
        static void Main(string[] args)
        {
            var r = new SqlMapper("Server=localhost;Database=DynamicDatabaseTest;User Id=simplesqlmapper;Password=simplesqlmapper");
            r.IsCaseSensitive = false;
            var dataList = r.GetListFromStoredProcedure<DataClass>("[sproc_Test_Dynamic]"
                , new
                {
                    para_string = "Mein String",
                    para_int = 999,
                    para_date = DateTime.Now.AddMinutes(-10),
                    para_bit = true,
                    para_decimal = 12.555
                });

            var dataList2 = r.GetList<DataClass>("SELECT * FROM TestTable", CommandType.Text);

            foreach (var data in dataList)
            {
                Console.WriteLine(data.ToString());
            }

            foreach (var data in dataList2)
            {
                Console.WriteLine(data.ToString());
            }

            Console.ReadLine();
        }
    }
}
