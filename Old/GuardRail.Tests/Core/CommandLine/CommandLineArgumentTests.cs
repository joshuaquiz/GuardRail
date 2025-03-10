using GuardRail.Core.CommandLine;
using Xunit;

namespace GuardRail.Tests.Core.CommandLine;

public static class CommandLineArgumentTests
{
    [Fact]
    public static void CommandLineArgumentToStringWithNoValue()
    {
        var argumentString = $"/{CommandLineArgumentType.FreshInstall}";
        var commandLineArgument = CommandLineArgument.Parse(argumentString);
        var strValue = commandLineArgument.ToString();
        Assert.Equal(argumentString, strValue);
    }

    [Fact]
    public static void CommandLineArgumentToStringWithNoSpaceValue()
    {
        var argumentString = $"/{CommandLineArgumentType.FreshInstall} true";
        var commandLineArgument = CommandLineArgument.Parse(argumentString);
        var strValue = commandLineArgument.ToString();
        Assert.Equal(argumentString, strValue);
    }

    [Fact]
    public static void CommandLineArgumentToStringWithSpaceValue()
    {
        var argumentString = $"/{CommandLineArgumentType.FreshInstall} \"true true\"";
        var commandLineArgument = CommandLineArgument.Parse(argumentString);
        var strValue = commandLineArgument.ToString();
        Assert.Equal(argumentString, strValue);
    }

    [Fact]
    public static void CommandLineArgumentParseValidArgumentWithNoValue()
    {
        var commandLineArgument = CommandLineArgument.Parse($"/{CommandLineArgumentType.FreshInstall}");
        Assert.Equal(CommandLineArgumentType.FreshInstall, commandLineArgument.Type);
        Assert.Null(commandLineArgument.Value);
    }

    [Fact]
    public static void CommandLineArgumentParseValidArgumentWithUnQuotedValue()
    {
        var commandLineArgument = CommandLineArgument.Parse($"/{CommandLineArgumentType.FreshInstall} true");
        Assert.Equal(CommandLineArgumentType.FreshInstall, commandLineArgument.Type);
        Assert.Equal("true", commandLineArgument.Value);
    }

    [Fact]
    public static void CommandLineArgumentParseValidArgumentWithQuotedValue()
    {
        var commandLineArgument = CommandLineArgument.Parse($"/{CommandLineArgumentType.FreshInstall} \"true\"");
        Assert.Equal(CommandLineArgumentType.FreshInstall, commandLineArgument.Type);
        Assert.Equal("true", commandLineArgument.Value);
    }

    [Fact]
    public static void CommandLineArgumentParseInvalidArgumentWithNoValue()
    {
        var e = Assert.Throws<InvalidCommandLineArgumentException>(() => CommandLineArgument.Parse("/invalid"));
        Assert.Equal("invalid", e.InvalidArgument);
        Assert.Equal("\"invalid\" is not a valid argument", e.Message);
    }

    [Fact]
    public static void CommandLineArgumentParseInvalidArgumentWithUnQuotedValue()
    {
        var e = Assert.Throws<InvalidCommandLineArgumentException>(() => CommandLineArgument.Parse("/invalid true"));
        Assert.Equal("invalid", e.InvalidArgument);
        Assert.Equal("\"invalid\" is not a valid argument", e.Message);
    }

    [Fact]
    public static void CommandLineArgumentParseInvalidArgumentWithQuotedValue()
    {
        var e = Assert.Throws<InvalidCommandLineArgumentException>(() => CommandLineArgument.Parse("/invalid \"true\""));
        Assert.Equal("invalid", e.InvalidArgument);
        Assert.Equal("\"invalid\" is not a valid argument", e.Message);
    }

    [Fact]
    public static void CommandLineArgumentParseInvalidArgumentFormatWithValue()
    {
        var e = Assert.Throws<InvalidCommandLineArgumentFormatException>(() => CommandLineArgument.Parse("--invalid \"true\""));
        Assert.Equal("--invalid \"true\"", e.InvalidArgument);
        Assert.Equal("\"--invalid \"true\"\" is not correctly formatted", e.Message);
    }

    [Fact]
    public static void CommandLineArgumentParseInvalidArgumentFormatWithNoValue()
    {
        var e = Assert.Throws<InvalidCommandLineArgumentFormatException>(() => CommandLineArgument.Parse("--invalid"));
        Assert.Equal("--invalid", e.InvalidArgument);
        Assert.Equal("\"--invalid\" is not correctly formatted", e.Message);
    }
}