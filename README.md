# GeoLoco

Simple command line tool that assists with geolocation on CSV-formatted address list. Can output to KML as well.

### Running GeoLoco

Copy the input `.csv` file and any `.kml` files to the same folder as the `geoloco` executable. Then run something like this:

```bash
# Example: Geocode addresses in contacts.csv and output to another CSV file
geoloco -f ./contacts.csv -o ./output.csv -l ./log.txt -v
```

```bash
# Example: Geocode addresses in contacts.csv and output to KML file
geoloco -f ./contacts.csv -o ./output.kml -l ./log.txt -v
```

The input CSV file should be formatted as:

```csv
Label,FullAddress,Latitude,Longitude
"Trader Joe's Portland - Hollywood (144)","4121 NE Halsey St, Portland, OR 97232 US",,
"Trader Joe's Portland Nw (146)","2122 NW Glisan St, Portland, OR 97210 US",,
```

### Set up the development environment

(This part really needs streamlined. For example, it should be easier to add your Azure Maps API key)

1. Clone this repo
1. Copy `appsetings.example.json` to `appsettings.json` and fill in the placeholders.
1. In particular, you will need an [Azure Maps](https://docs.microsoft.com/en-us/azure/azure-maps/) API key
1. Build the solution in VS 2019+ and run the tests
1. Publish the `GeoLoco.CLI` solution.
1. Wonder why this is so clunky, fork the project and make it better.

## More Docs

From `geoloco -h`:

```
Takes an address list (*.csv), geocodes them with Azure Maps, and produces a new .csv or .kml file

Usage: geoloco [options]

Options:
  -h                                        Show help information.
  -f|--address-list-csv <ADDRESS_LIST_CSV>  Path to .csv file to geocode
  -o|--output-file <OUTPUT_FILE>            Path to write output to. (.csv|.kml) extension. (Will be overwritten.)
  -b|--boundary-kml <BOUNDARY_KML>          Path to file (*.kml) containing boundaries to include in kml output.
  -nm|--no-money                            Don't use any services like Azure Maps that could cost $$$.
  -ck|--check                               Check the address list .csv file for errors.
  -gt|--generate-csv-template               Writes a CSV template to address_template.csv
  -l|--log-path <LOG_PATH>                  Path to log file. (Will be overwritten.)
  -v|--verbose                              Log verbose output to the console (--log-path is always verbose)
  -q|--quiet                                Don't log anything to the console.
  -vs|--version                             Print program version.
```

