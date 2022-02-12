using System;
using System.Threading.Tasks;
using NetPad.Events;
using NetPad.Runtimes;
using NetPad.Scripts;
using NetPad.UiInterop;
using O2Html;
using O2Html.Dom;

namespace NetPad.BackgroundServices
{
    public class IpcScriptOutputWriter : IScriptRuntimeOutputWriter
    {
        private readonly IIpcService _ipcService;
        private readonly HtmlSerializerSettings _htmlSerializerSettings;

        public IpcScriptOutputWriter(ScriptEnvironment environment, IIpcService ipcService)
        {
            Environment = environment;
            _ipcService = ipcService;
            _htmlSerializerSettings = new HtmlSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.IgnoreAndSerializeCyclicReference
            };
        }

        public ScriptEnvironment Environment { get; }

        public async Task WriteAsync(object? output, string? title = null)
        {
            var group = new Element("div").WithAddClass("group");

            if (title != null)
            {
                group.WithAddClass("titled")
                    .AddAndGetElement("h6")
                    .WithAddClass("title")
                    .AddText(title);
            }

            Element element;

            try
            {
                element = HtmlConvert.Serialize(output, _htmlSerializerSettings);
            }
            catch (Exception ex)
            {
                element = HtmlConvert.Serialize(ex, _htmlSerializerSettings);
            }

            group.AddChild(element);

            await _ipcService.SendAsync(new ScriptOutputEmitted(Environment.Script.Id, group.ToHtml()));
        }
    }
}