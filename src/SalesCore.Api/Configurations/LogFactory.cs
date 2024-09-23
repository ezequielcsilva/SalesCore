using Destructurama;
using Serilog;
using Serilog.Exceptions;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace SalesCore.Api.Configurations;

public static class LogFactory
{
    public static Serilog.Core.Logger CreateSerilogLogger(IConfiguration configuration,
        bool readFromConfiguration = true)
    {
        var config = new LoggerConfiguration()
            .Destructure.UsingAttributes()
            .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Async(wt => wt.Console(new ExpressionTemplate(
                "{ {Timestamp: @t, Level: @l, Message: @m, Properties: @p, Stacktrace: @x } }\n",
                theme: TemplateTheme.Literate)));

        if (readFromConfiguration)
        {
            config.ReadFrom.Configuration(configuration);
        }

        return config.CreateLogger();
    }
}