using System;

namespace SimpleSqlMapper.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new SqlMapper("Server=localhost;Database=DynamicDatabaseTest;User Id=simplesqlmapper;Password=simplesqlmapper");
            var dataList = r.GetListFromStoredProcedure<DataClass>("[sproc_Test_Dynamic]"
                , new
                {
                    para_string = "Mein String",
                    para_int = 999,
                    para_date = DateTime.Now.AddMinutes(-10),
                    para_bit = true,
                    para_decimal = 12.555
                });

            var r2 = new SqlMapper();
            var dataList2 = r2.GetList<DataClass>("SELECT * FROM TestTable", SqlCommandType.QueryText);

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
