using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoLoco.Core.Model.Geolocation;

namespace GeoLoco.Core.Services
{
    public class KmlBuilder
    {
        private List<PointLocation> _pointLocations = new();

        private GeoBoundary _geoBoundary;

        public string Build()
        {
            var locationNodesSb = new StringBuilder();

            foreach (var point in _pointLocations)
            {
                var nodeString = ConvertPointLocationKmlNodeString(point);
                locationNodesSb.Append(nodeString);
            }

            var boundaryCoordinatesKml = string.Empty;
            if (_geoBoundary != null)
            {
                var coordinatesSb = new StringBuilder();

                foreach (var coords in _geoBoundary.Coordinates)
                {
                    coordinatesSb.AppendLine(coords.ToKmlString());
                }

                boundaryCoordinatesKml = string.Format(
                    GetPolygonPlacemarkTemplate(),
                    coordinatesSb.ToString());
            }

            var kml = string.Format(
                GetKmlTemplateBase(),
                locationNodesSb.ToString(),
                boundaryCoordinatesKml);

            return kml;
        }

        public KmlBuilder AddPointLocations(IEnumerable<PointLocation> points)
        {
            _pointLocations = _pointLocations.Union(points).ToList();
            return this;
        }

        public KmlBuilder AddPointLocation(PointLocation point)
        {
            _pointLocations.Add(point);
            return this;
        }

        public KmlBuilder AddBoundary(GeoBoundary boundary)
        {
            _geoBoundary = boundary;
            return this;
        }

        private string ConvertPointLocationKmlNodeString(PointLocation point)
        {
            if (point == null
                || point.Coordinates == null)
            {
                return string.Empty;
            }

            var locationString = @$"
    <Placemark>
      <name><![CDATA[{point.Label ?? "?"}]]></name>
      <styleUrl>#icon-1603-000000-nodesc</styleUrl>
      <Point>
        <coordinates>
          {point.Coordinates.ToKmlString()}
        </coordinates>
      </Point>
    </Placemark>";
            return locationString;
        }

        private string GetPolygonPlacemarkTemplate()
        {
            var template = @"
    <Placemark>
        <name>Boundary</name>
        <styleUrl>#poly-9C27B0-4000-74-nodesc</styleUrl>
        <Polygon>
          <outerBoundaryIs>
            <LinearRing>
              <tessellate>1</tessellate>
              <coordinates>
              {0}
              </coordinates>
            </LinearRing>
          </outerBoundaryIs>
        </Polygon>
      </Placemark> ";
            return template;
        }

        private string GetKmlTemplateBase()
        {
            var template = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"">
  <Document>
    <name>GeoLoco project</name>
    <Style id=""pushpin"">
     <IconStyle id=""icon-pushpin"">
      <Icon>
        <href>http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png</href>
        <scale>1.0</scale>
      </Icon>
     </IconStyle>
     <LabelStyle>
        <scale>0</scale>
     </LabelStyle>
     <BalloonStyle>
       <text><![CDATA[<h3>$[name]</h3>]]></text>
     </BalloonStyle>
    </Style>
    <Style id=""icon-1603-000000-nodesc-normal"">
      <IconStyle>
        <scale>1</scale>
        <Icon>
          <href>http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png</href>
        </Icon>
      </IconStyle>
      <LabelStyle>
        <scale>0</scale>
      </LabelStyle>
      <BalloonStyle>
        <text><![CDATA[<h3>$[name]</h3>]]></text>
      </BalloonStyle>
    </Style>
    <Style id=""icon-1603-000000-nodesc-highlight"">
      <IconStyle>
        <scale>1</scale>
        <Icon>
          <href>http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png</href>
        </Icon>
      </IconStyle>
      <LabelStyle>
        <scale>1</scale>
      </LabelStyle>
      <BalloonStyle>
        <text><![CDATA[<h3>$[name]</h3>]]></text>
      </BalloonStyle>
    </Style>
    <Style id=""poly-9C27B0-4000-74-nodesc-normal"">
      <LineStyle>
        <color>ffb0279c</color>
        <width>4</width>
      </LineStyle>
      <PolyStyle>
        <color>4ab0279c</color>
        <fill>1</fill>
        <outline>1</outline>
      </PolyStyle>
      <BalloonStyle>
        <text><![CDATA[<h3>$[name]</h3>]]></text>
      </BalloonStyle>
    </Style>
    <Style id=""poly-9C27B0-4000-74-nodesc-highlight"">
      <LineStyle>
        <color>ffb0279c</color>
        <width>6</width>
      </LineStyle>
      <PolyStyle>
        <color>4ab0279c</color>
        <fill>1</fill>
        <outline>1</outline>
      </PolyStyle>
      <BalloonStyle>
        <text><![CDATA[<h3>$[name]</h3>]]></text>
      </BalloonStyle>
    </Style>
    <StyleMap id=""icon-1603-000000-nodesc"">
      <Pair>
        <key>normal</key>
        <styleUrl>#icon-1603-000000-nodesc-normal</styleUrl>
      </Pair>
      <Pair>
        <key>highlight</key>
        <styleUrl>#icon-1603-000000-nodesc-highlight</styleUrl>
      </Pair>
    </StyleMap>
    <StyleMap id=""poly-9C27B0-4000-74-nodesc"">
      <Pair>
        <key>normal</key>
        <styleUrl>#poly-9C27B0-4000-74-nodesc-normal</styleUrl>
      </Pair>
      <Pair>
        <key>highlight</key>
        <styleUrl>#poly-9C27B0-4000-74-nodesc-highlight</styleUrl>
      </Pair>
    </StyleMap>

    <!-- LOCATIONS -->
    <Folder>
      <name>Addresses</name>
     {0}
    </Folder>

    <!-- BOUNDARY -->
    <Folder>
      <name>Boundaries</name>
      {1}

    </Folder>

  </Document>
</kml>
";
            return template;
        }
    }
}
