using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLoco.Core.Services;
using GeoLoco.Infrastructure;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.Services
{
    public class SimpleCsvParserShould
    {
        [Fact]
        public async Task CreateATemplateCsvFileAsync()
        {
            const string fileName = "./template.csv";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var parser = new SimpleCsvParser(new AppConfig());

            await parser.CreateTemplateFileAsync(fileName);

            File.Exists(fileName).ShouldBeTrue();
        }
    }
}
