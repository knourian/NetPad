using System.Reflection;

namespace NetPad.Apps.App.Common.Plugins;

public record PluginRegistration(Assembly Assembly, IPlugin Plugin);
