// Written By Gary Wee
// 2022-07-28
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using Grib.Api;

namespace ReadGrib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string file_name = @"20150806.grib";
            Get_Keys(file_name);
            Method_1(file_name);
            Method_2(file_name);
            Console.WriteLine("Finish Parsing Grib.");
            Console.ReadLine();
        }

        static void Method_1(string file_name)
        {
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

                    var Message_String = msg.ToString();
                    var Time = msg.Time;
                    var Parameter_Name = msg.Name;
                    var Units = msg.Units;
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
        }

        // For parsing ERA-5 Grib files.
        // This method can be used when the keys are known.
        static void Method_2(string file_name)
        {
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
                Label label = new Label();
                label.Dock = DockStyle.Top;
                label.Text = msg_str;
                label.Text += "\r\nUnit: " + msg.Units;
                label.Height = 30;
                DataGridView dgv = new DataGridView();
                dgv.DataSource = dt;
                dgv.Dock = DockStyle.Fill;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                Form f = new Form();
                f.Controls.Add(dgv);
                f.Controls.Add(label);
                f.ShowDialog();
            }
        }

        static void Get_Keys(string file_name)
        {
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
                    dt.Rows.Add(row);
                    i++;
                }
            }
            DataGridView dgv = new DataGridView();
            dgv.DataSource = dt;
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Form f = new Form();
            f.Controls.Add(dgv);
            f.ShowDialog();
        }
    }
}