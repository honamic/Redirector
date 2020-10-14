using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Honamic.Redirector
{
    public class OptionsRedirectorStorage : IRedirectorStorage
    {
        private readonly IOptionsMonitor<RedirectorResurceOptions> _options;
        private readonly ILogger<OptionsRedirectorStorage> _logger;
        private readonly OptionsChangedHandler _changedHandler;

        public OptionsRedirectorStorage(IOptionsMonitor<RedirectorResurceOptions> options,
            ILogger<OptionsRedirectorStorage> logger,
            OptionsChangedHandler changedHandler
            )
        {
            _options = options;
            _logger = logger;

            //warm it up after first use
            _changedHandler = changedHandler;
        }

        public List<RedirectObject> GetAll()
        {
            var list = _options.CurrentValue.Items;

            list.ForEach(i => i.HttpCode = !i.HttpCode.HasValue ? _options.CurrentValue.StatusCode : i.HttpCode);

            return list;
        }
    }
}