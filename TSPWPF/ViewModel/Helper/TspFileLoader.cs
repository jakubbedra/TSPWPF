using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TSPWPF.Model;

namespace TSPWPF.ViewModel.Helper;

public static class TspFileLoader
{
    public static List<City> CreateCitiesListFromFile(string filePath)
    {
        List<City> cities = new List<City>();
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 7; !lines[i].Equals("EOF"); i++)
        {
            string[] split = lines[i].Split(" ");
            cities.Add(new City
            {
                Id = Int32.Parse(split[0]),
                X = Double.Parse(split[1], CultureInfo.InvariantCulture),
                Y = Double.Parse(split[2], CultureInfo.InvariantCulture)
            });
        }
        
        return cities;
    }
}