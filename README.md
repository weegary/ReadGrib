# C# Read Grib

## Meta
This program is written by Gary Wee.

Date: 2022 Jul 28

## Description
This program uses ```Grib.Api``` Nuget package to extract messages in Grib files, it is based on .NET Framework because the package cannot be used in .NET 6.

## Basic Codes

1. Import Grib file.

```C#
GribFile file = new GribFile(file_name)
```

2. Get the messages in the GribFile. <br/>
Remark: Some of the grib files may cause error when running ```msg.GeoSpatialValues``` due to difference of meta-data in the grib file.

```C#
foreach (GribMessage msg in file)
{
    var Time = msg.Time;
    var Parameter_Name = msg.Name;
    var Level = msg.Level;
    var GeoSpatialValues = msg.GeoSpatialValues;

    foreach (var point in GeoSpatialValues)
    {
        if (point.IsMissing)
            continue;
        var Longitude = point.Longitude;
        var Latitude = point.Latitude;
        var Value = point.Value;
    }
}
```
