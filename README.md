# AdoNetProfiler

Profiler for ADO.NET with hooking ```DbConnection```, ```DbCommand```, ```DbDataReader```, ```DbTransaction```.
Although threre is already <a href="">MiniProfiler</a> as similar, I make this because I think it lacks hook points.
AdoNetProfiler can profile the state of ```DbConnection```, ```DbCommand``` and ```DbTransaction```.

[![Build status](https://ci.appveyor.com/api/projects/status/9qtd3fxwft5ucxlj?svg=true)](https://ci.appveyor.com/project/ttakahari/adonetprofiler)

## Install

from NuGet - <a href="https://www.nuget.org/packages/AdoNetProfiler/">AdoNetProfiler</a>

```ps1
PM > Install-Package AdoNetProfiler
```

## How to use

In stead of creating the connection instance directly from ```SqlConnection``` or others, create from ```AdoNetProfilerDbConnection``` with giving the original connection instance and your profiler implementing ```IAdoNetProfiler``` as arguments.

```csharp
using (var connection = new AdoNetProfilerConnection(new SqlConnection("Your connection string"), new YourProfiler()))
{
    // write data-access logics here.
}
```

Or, if initalizing ```AdoNetProfilerFactory``` when the application starts, it is available from ```DbProviderFctories```.

```csharp
// when the application starts.
AdoNetProfilerFactory.Inialize(typeof(YourProfiler)).

// data-access logic
var factory = DbProviderFactories.GetFactory("System.Data.SqlClient").
using (var connection = factory.CreateConnection())
{
    connection.ConnectionString = "[Your connection string]";

    // write data-access logics here.
}
```

## Lisence

under <a href="http://opensource.org/licenses/MIT">MIT License</a>
