# SimpleSqlMapper

A simple SQL to POCO mapper.

# Installation (NuGet)
    Install-Package SimpleSqlMapper 

# Usage
```cs
var db = new SqlMapper();
var data = db.GetListFromStoredProcedure<DataClass>("dbo.my_stored_procedure", new { para_filter = filter });

foreach (var row in data)
{
  do_something_with(row);
}
```

# Configuration / ConnectionString

## Constructor

You can set a connectionstring using the constructor

```cs
var db = new SqlMapper("your_connection_string");
```

## App.Config / Web.Config

You can set a default connectionstring and use the parameterless constructor

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="SqlMapperDefaultConnection"  connectionString="Server=localhost;Database=DynamicDatabaseTest;User Id=simplesqlmapper;Password=simplesqlmapper"/>
  </connectionStrings>
</configuration>
```

Please refer to the sample project SimpleSqlMapper.Sample for further samples.


