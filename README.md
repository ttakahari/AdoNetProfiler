# AdoNetProfiler

Profiler for ADO.NET with hooking ```DbConnection```, ```DbCommand```, ```DbDataReader```, ```DbTransaction```.
Although threre is already <a href="">MiniProfiler</a> as similar, I make this because I think it lacks hook points.
AdoNetProfiler can profile the state of ```DbConnection```, ```DbCommand``` and ```DbTransaction```.

[![AppVeyor](https://img.shields.io/appveyor/ci/gruntjs/grunt.svg?style=plastic)](https://ci.appveyor.com/project/ttakahari/adonetprofiler)
[![NuGet](https://img.shields.io/nuget/v/AdoNetProfiler.svg?style=plastic)](https://www.nuget.org/packages/AdoNetProfiler/)

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

## Profiler

```IAdoNetProfiler``` has the following contents.

```csharp
/// <summary>
/// The interface that defines methods profiling of database accesses.
/// </summary>
public interface IAdoNetProfiler
{
    /// <summary>
    /// Get the profiler is enabled or not.
    /// </summary>
    bool IsEnabled { get; }
        
    /// <summary>
    /// Execute before the connection open.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnOpening(DbConnection connection);

    /// <summary>
    /// Execute after the connection open.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnOpened(DbConnection connection);

    /// <summary>
    /// Execute before the connection close.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnClosing(DbConnection connection);

    /// <summary>
    /// Execute after the connection close.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnClosed(DbConnection connection);

    /// <summary>
    /// Execute before the connection creates the transaction.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnStartingTransaction(DbConnection connection);

    /// <summary>
    /// Execute after the connection creates the transaction.
    /// </summary>
    /// <param name="transaction">The created transaction.</param>
    void OnStartedTransaction(DbTransaction transaction);

    /// <summary>
    /// Execute before the transaction commits.
    /// </summary>
    /// <param name="transaction">The current transaction.</param>
    void OnCommitting(DbTransaction transaction);

    /// <summary>
    /// Execute after the transaction commits.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnCommitted(DbConnection connection);

    /// <summary>
    /// Execute before the transaction rollbacks.
    /// </summary>
    /// <param name="transaction">The current transaction.</param>
    void OnRollbacking(DbTransaction transaction);

    /// <summary>
    /// Execute after the transaction rollbacks.
    /// </summary>
    /// <param name="connection">The current connection.</param>
    void OnRollbacked(DbConnection connection);

    /// <summary>
    /// Execute before <see cref="DbCommand.ExecuteReader()"/> executes.
    /// </summary>
    /// <param name="command">The executing command.</param>
    void OnExecuteReaderStart(DbCommand command);

    /// <summary>
    /// Execute after <see cref="DbDataReader"/> reads.
    /// </summary>
    /// <param name="reader">The stream from data source.</param>
    /// <param name="record">The number of read data.</param>
    void OnReaderFinish(DbDataReader reader, int record);

    /// <summary>
    /// Execute before <see cref="DbCommand.ExecuteNonQuery"/> executes.
    /// </summary>
    /// <param name="command">The executing command.</param>
    void OnExecuteNonQueryStart(DbCommand command);

    /// <summary>
    /// Execute after <see cref="DbCommand.ExecuteNonQuery"/> executes.
    /// </summary>
    /// <param name="command">The executing command.</param>
    /// <param name="executionRestlt">The result of executing <see cref="DbCommand.ExecuteNonQuery"/>.</param>
    void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt);

    /// <summary>
    /// Execute before <see cref="DbCommand.ExecuteScalar"/> executes.
    /// </summary>
    /// <param name="command">The executing command.</param>
    void OnExecuteScalarStart(DbCommand command);

    /// <summary>
    /// Execute after <see cref="DbCommand.ExecuteScalar"/> executes.
    /// </summary>
    /// <param name="command">The executing command.</param>
    /// <param name="executionRestlt">The result of executing <see cref="DbCommand.ExecuteScalar"/>.</param>
    void OnExecuteScalarFinish(DbCommand command, object executionRestlt);

    /// <summary>
    /// Execute when the error occurs while the command executing.
    /// </summary>
    /// <param name="command">The executing command.</param>
    /// <param name="exception">The occurred error.</param>
    void OnCommandError(DbCommand command, Exception exception);
}
```

Implementing this interface, you can get your profiler.

An instance of `IAdoNetProfiler` is not thread-safe. So you should not use an instance of `IAdoNetProfiler` among some instances of `DbConnection`.

## Entity Framework

AdoNerProfiler does not support Entity Framework. When you use AdoNetProfiler with Entity Framework in a same project, some errors may occur becouse of conflicting.

## Glimpse

<a href="http://getglimpse.com/">Glimpse</a> is profiling library for ASP.NET.

AdoNetProfiler has the library for Glimpse.

from NuGet - <a href="https://www.nuget.org/packages/Glimpse.AdoNetProfiler/">Glimpse.AdoNetProfiler</a>

```ps1
PM > Install-Package Glimpse.AdoNetProfiler
```

Glimpse.Ado, that has already been provided as a part of Glimpse libaries, displays SQL profilings.

But Glimpse.AdoNetProfiler displays connection and transaction profilings, to say nothing of SQL.

<img src="https://github.com/ttakahari/AdoNetProfiler/blob/master/docs/timeline.png" />

<img src="https://github.com/ttakahari/AdoNetProfiler/blob/master/docs/command.png" />

<img src="https://github.com/ttakahari/AdoNetProfiler/blob/master/docs/connection.png" />

<img src="https://github.com/ttakahari/AdoNetProfiler/blob/master/docs/transaction.png" />

## Lisence

under <a href="http://opensource.org/licenses/MIT">MIT License</a>
