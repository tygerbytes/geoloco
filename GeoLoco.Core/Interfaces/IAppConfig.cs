using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using GeoLoco.Core.Model;
using Microsoft.Extensions.Configuration;

namespace GeoLoco.Core.Interfaces
{
    public interface IAppConfig
    {
        IAppLogger Log { get; }

        HttpClient HttpClient { get; }

        IConfiguration Configuration { get; }

        IDateService Date { get; }

        bool NoMoney { get; }

        string Version { get; }
    }
}
