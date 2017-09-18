# SimpleSqlMapper

A simple SQL to POCO mapper.

# Usage
```cs
var db = new SqlMapper();
var data = db.GetListFromStoredProcedure<DataClass>("dbo.my_stored_procedure", new { para_filter = filter });

foreach (var row in data)
{
  do_something_with(row);
}
```

Please refer to the sample project SimpleSqlMapper.Sample for further samples.

## Installation (NuGet)
    Install-Package SimpleSqlMapper 
