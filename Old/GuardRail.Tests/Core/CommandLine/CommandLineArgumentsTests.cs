using System.Collections.Generic;
using GuardRail.Core.CommandLine;
using Xunit;

namespace GuardRail.Tests.Core.CommandLine;

public static class CommandLineArgumentsTests
{
    [Fact]
    public static void CommandLineArgumentsToStringSingleValue()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1
            });
        var stringValue = commandLineArguments.ToString();
        Assert.Equal(argument1, stringValue);
    }

    [Fact]
    public static void CommandLineArgumentsToStringMultiValue()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var argument2 = $"/{CommandLineArgumentType.ShouldShowSetup} false";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1,
                argument2
            });
        var stringValue = commandLineArguments.ToString();
        Assert.Equal($"{argument1} {argument2}", stringValue);
    }

    [Fact]
    public static void CommandLineArgumentsContainsKeyTrue()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var argument2 = $"/{CommandLineArgumentType.ShouldShowSetup} false";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1,
                argument2
            });
        Assert.True(commandLineArguments.ContainsKey(CommandLineArgumentType.FreshInstall));
    }

    [Fact]
    public static void CommandLineArgumentsContainsKeyFalse()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var argument2 = $"/{CommandLineArgumentType.ShouldShowSetup} false";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1,
                argument2
            });
        Assert.False(commandLineArguments.ContainsKey(CommandLineArgumentType.Update));
    }

    [Fact]
    public static void CommandLineArgumentsTryGetByKeyReturnsItem()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var argument2 = $"/{CommandLineArgumentType.ShouldShowSetup} false";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1,
                argument2
            });
        var foundArgument = commandLineArguments.TryGetByKey(CommandLineArgumentType.FreshInstall);
        Assert.Equal(CommandLineArgumentType.FreshInstall, foundArgument.Type);
    }

    [Fact]
    public static void CommandLineArgumentsTryGetByKeyReturnsNull()
    {
        var argument1 = $"/{CommandLineArgumentType.FreshInstall} true";
        var argument2 = $"/{CommandLineArgumentType.ShouldShowSetup} false";
        var commandLineArguments = CommandLineArguments.Create(
            new List<string>
            {
                argument1,
                argument2
            });
        var foundArgument = commandLineArguments.TryGetByKey(CommandLineArgumentType.Update);
        Assert.Null(foundArgument);
    }
}