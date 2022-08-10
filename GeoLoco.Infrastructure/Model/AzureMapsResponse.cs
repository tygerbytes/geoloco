using System;
using System.Collections.Generic;
using System.Text;
using GeoLoco.Core.Model;
using Newtonsoft.Json;

namespace GeoLoco.Infrastructure.Model
{
    internal class AzureMapsResponse
    {
        // NOTE: Thanks to https://json2csharp.com/ for the quick POCO generation

        public Summary Summary { get; set; }

        public List<Result> Results { get; set; }
    }

    public class Summary
    {
        public string Query { get; set; }

        public string QueryType { get; set; }

        public int QueryTime { get; set; }

        public int NumResults { get; set; }

        public int Offset { get; set; }

        public int TotalResults { get; set; }

        public int FuzzyLevel { get; set; }
    }

    public class Address
    {
        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string Municipality { get; set; }

        public string CountrySecondarySubdivision { get; set; }

        public string CountrySubdivision { get; set; }

        public string CountrySubdivisionName { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public string Country { get; set; }

        public string CountryCodeIso3 { get; set; }

        public string FreeformAddress { get; set; }

        public string LocalName { get; set; }

        public string ExtendedPostalCode { get; set; }
    }

    public class Position
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class TopLeftPoint
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class BtmRightPoint
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class Viewport
    {
        public TopLeftPoint TopLeftPoint { get; set; }

        public BtmRightPoint BtmRightPoint { get; set; }
    }

    public class Position2
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class EntryPoint
    {
        public string Type { get; set; }

        public Position2 Position { get; set; }
    }

    public class From
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class To
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class AddressRanges
    {
        public string RangeLeft { get; set; }

        public From From { get; set; }

        public To To { get; set; }
    }

    public class Result
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public double Score { get; set; }

        public Address Address { get; set; }

        public Position Position { get; set; }

        public Viewport Viewport { get; set; }

        public List<EntryPoint> EntryPoints { get; set; }

        public AddressRanges AddressRanges { get; set; }
    }
}
