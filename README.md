# RhinoETL

Rhino Etl is a simple [Extract, transform and load](http://en.wikipedia.org/wiki/Extract,_transform,_load) library for .NET.

# Resources

  * [Hibernating Rhinos](http://hibernatingrhinos.com/oss/rhino-etl)
  
# Building

In order to build this project, you need to execute the psake build script from the PowerShell console.
You do this using the following command from the root project directory:

```
.\psake.ps1 default.ps1
```

You may need to allow script execution by running the following command as adminstrator:

```powershell
Set-ExecutionPolicy unrestricted
```

Also note that the build script assume that you have git.exe on your path.

Running the tests requires access to a database called 'test' on the default instance of SQL Server
  
# Releases

## 1.3.1

  * _Released_: July 15th 2015
  * *BREAKING CHANGE*
  * [Updated Common.Logging to version 3.0.0](https://github.com/hibernating-rhinos/rhino-etl/pull/21)

## 1.2.6

  * _Released_: July 15th 2015
  * [Bug Fix for issue: JoinOperation errors are not propagated to the main EtlProcess](https://github.com/hibernating-rhinos/rhino-etl/pull/19)
  * [Update OutputCommandOperation.cs](https://github.com/hibernating-rhinos/rhino-etl/pull/22)

## 1.2.5

  * _Released_: December 23rd 2014
  * [Support custom row key comparers](https://github.com/hibernating-rhinos/rhino-etl/pull/17)

## 1.2.4

  * _Released_: January 23rd 2014
  * [Test and fix for loss of final rows by SortMergeJoinOperation](https://github.com/hibernating-rhinos/rhino-etl/pull/14)
  * [Support ADO.NET compatible provider names in connection settings](https://github.com/hibernating-rhinos/rhino-etl/pull/15)

## 1.2.3
  
  * _Released_: September 24th 2013
  * [Sort merge join](https://github.com/hibernating-rhinos/rhino-etl/pull/11)
  * [Added an aggregation operation that assumes a sorted rowset](https://github.com/hibernating-rhinos/rhino-etl/pull/10)

