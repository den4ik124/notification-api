using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApplication.Test.Integration;

public class CalculatorFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddTransient<ICalculator, Calculator>()
            .Configure<Options>(config => configuration?.GetSection("Options").Bind(config));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }
}