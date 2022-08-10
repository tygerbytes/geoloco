using System.IO;
using System.Threading.Tasks;
using GeoLoco.CLI;
using GeoLoco.Tests.TestData;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.CLI
{
    public class ProgramShould
    {
        [Fact]
        public async Task GeocodeAnAddressListAndProduceNewCsvFile()
        {
            const string outputFile = "./out.csv";

            var x = await Program.Main(new[] {
                    "--address-list-csv", TestPaths.AddressesCsv,
                    "--output-file", outputFile,
                    //"--no-money",
                });
            x.ShouldBe(0);

            File.Exists(outputFile).ShouldBeTrue();

            var contents = File.ReadAllText(outputFile);
            contents.ShouldContain("45.534");
        }

        [Fact]
        public async Task GeocodeAnAddressListAndProduceKmlFile()
        {
            const string outputFile = "./out.kml";

            var x = await Program.Main(new[] {
                    "--address-list-csv", TestPaths.AddressesCsv,
                    "--output-file", outputFile,
                    "--boundary-kml", TestPaths.BoundaryKml,
                    //"--no-money"
                });
            x.ShouldBe(0);

            File.Exists(outputFile).ShouldBeTrue();

            var contents = File.ReadAllText(outputFile);
            contents.ShouldContain("45.534");
            contents.ShouldContain("<Placemark>");
        }

        [Fact]
        public async Task CheckCsvFile()
        {
            var x = await Program.Main(new[] {
                    "--check",
                    "--address-list-csv", TestPaths.AddressesCsv,
                });
            x.ShouldBe(0);
        }
    }
}
