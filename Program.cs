// Written By Gary Wee
// 2022-07-28
using System;
using System.Collections.Generic;
using Grib.Api;

namespace ReadGrib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string file_name = @"20150806.grib";
            List<double> Longitude;
            List<double> Latitude;
            List<double> Value;
            using (GribFile file = new GribFile(file_name))
            {
                foreach (GribMessage msg in file)
                {
                    Longitude = new List<double>();
                    Latitude = new List<double>();
                    Value = new List<double>();

                    var Time = msg.Time;
                    var Parameter_Name = msg.Name;
                    var Level = msg.Level;
                    var GeoSpatialValues = msg.GeoSpatialValues;

                    foreach (var point in GeoSpatialValues)
                    {
                        if (point.IsMissing)
                            continue;
                        Longitude.Add(point.Longitude);
                        Latitude.Add(point.Latitude);
                        Value.Add(point.Value);
                    }
                    break; // Read First Message Only.
                }
            }
            Console.WriteLine("Finish Parsing Grib.");
            Console.ReadLine();
        }
    }
}