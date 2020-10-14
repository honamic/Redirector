using Microsoft.Extensions.Options;

namespace Honamic.Redirector
{
    public class OptionsChangedHandler
    {
        private readonly IRedirectorManager _redirectorManager;
        private readonly IOptionsMonitor<RedirectorResurceOptions> _optionMonitor;

        public OptionsChangedHandler(RedirectorManager redirectorManager, IOptionsMonitor<RedirectorResurceOptions> optionMonitor)
        {
            _redirectorManager = redirectorManager;
            _optionMonitor = optionMonitor;

            optionMonitor.OnChange(onResourceChanged);

        }

        private void onResourceChanged(RedirectorResurceOptions optionMonitor)
        {
            _redirectorManager.Reload();
        }
    }
}
