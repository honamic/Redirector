using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Honamic.Redirector
{
    public class OptionsRedirectorStorage : IRedirectorStorage
    {
        private readonly IOptionsMonitor<RedirectorResurceOptions> _options;
        private readonly ILogger<OptionsRedirectorStorage> _logger;

        public OptionsRedirectorStorage(IOptionsMonitor<RedirectorResurceOptions> options, ILogger<OptionsRedirectorStorage> logger)
        {
            _options = options;
            _logger = logger;
        }

        public List<RedirectObject> GetAll()
        {
            return _options.CurrentValue.Items;
        }
    }
}