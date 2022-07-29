# C# Read Grib

## Meta
This program is written by Gary Wee.

Date: 2022 Jul 28

## Description
This program uses ```Grib.Api``` Nuget package to extract messages in Grib files, it is based on .NET Framework because the package cannot be used in .NET 6.

## Basic Codes

1. Import Grib file. (ERA5 Grib file is used as example)

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

3. If if ```msg.GeoSpatialValues``` cannot be used, we can read the keys and values in the message, then figure out the grid's position and value using another method. 

4. Get keys and values in a Grib message.
```C#
DataTable dt = new DataTable();
dt.Columns.Add("no");
dt.Columns.Add("key");
dt.Columns.Add("value");
using (GribFile file = new GribFile(file_name))
{
    GribMessage msg = file.First();
    double[] rawValues;
    msg.Values(out rawValues);
    int i = 0;
    foreach (var m in msg)
    {
        DataRow row = dt.NewRow();
        row["no"] = i;
        row["key"] = m.Key;
        row["value"] = m.ToString();
        t.Rows.Add(row);
        i++;
    }
}
```
![ReadKeysAndValues.JPG](/ReadKeysAndValues.JPG)

5. Some keys can be selected to calculate the position of grid.
- longitudeOfFirstGridPoint 第一個點經度
- latitudeOfFirstGridPoint 第一個點緯度
- Ni  經度方向資料個數
- Nj  緯度方向資料個數
- iDirectionIncrement 經度方向間距
- jDirectionIncrement 緯度方向間距
- iScansPositively 經度間距是否為正值
- jScansPositively 緯度間距是否為正值

Remarks: Some of the keys of grid information can be found at ECMWF's website.
- [Grid1:Mercator](https://apps.ecmwf.int/codes/grib/format/grib1/grids/1/)
- [Grid3:Lambert conformal](https://apps.ecmwf.int/codes/grib/format/grib1/grids/3/)

Therefore, we can calculate the location of each grid. And ```msg.Values(out rawValues)``` can be used to extract the readings of certain paramter.
```C#
DataTable dt = new DataTable();
dt.Columns.Add("no");
dt.Columns.Add("lon");
dt.Columns.Add("lat");
dt.Columns.Add("value");
using (GribFile file = new GribFile(file_name))
{
    GribMessage msg = file.First();
    double[] rawValues;
    msg.Values(out rawValues);
    string msg_str = msg.ToString();
    var lon_first_grid = msg["longitudeOfFirstGridPoint"].AsDouble();
    var lat_first_grid = msg["latitudeOfFirstGridPoint"].AsDouble();
    var Ni = msg["Ni"].AsInt();
    var Nj = msg["Nj"].AsInt();
    var interval_i = msg["iDirectionIncrement"].AsDouble();
    var interval_j = msg["jDirectionIncrement"].AsDouble();
    if (msg["iScansPositively"].AsString() == "0")
        interval_i = -interval_i;
    if (msg["jScansPositively"].AsString() == "0")
        interval_j = -interval_j;

    double lon, lat;
    int index_raw_data = 0;
    for (int j = 0; j < Nj; j += 1)
    {
        lat = lat_first_grid + j * interval_j;
        for (int i = 0; i < Ni; i += 1)
        {
            lon = lon_first_grid + i * interval_i;
            DataRow row = dt.NewRow();
            row["no"] = index_raw_data;
            row["lon"] = lon;
            row["lat"] = lat;
            row["value"] = rawValues[index_raw_data];
            dt.Rows.Add(row);
            index_raw_data++;
        }
    }
```
