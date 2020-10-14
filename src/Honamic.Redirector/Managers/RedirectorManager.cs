using Honamic.Redirector.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Honamic.Redirector
{
    public class RedirectorManager : IRedirectorManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RedirectorManager> _logger;
        private readonly object _lock = new object();
        private ConcurrentDictionary<string, CachedRedirectObject> RedirectObjects;

        public int _httpCodeResult = 307;

        public RedirectorManager(IServiceProvider serviceProvider, ILogger<RedirectorManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public RedirectResult Evaluate(HttpRequest request)
        {
            try
            {

                if (RedirectObjects == null)
                {
                    TryInitialize();
                }

                if (RedirectObjects.Count == 0)
                {
                    return null;
                }

                var path = request.Path;

                var normalizedPath = NormalizePath(path);

                foreach (var item in RedirectObjects)
                {
                    switch (item.Value.Type)
                    {
                        case RedirectType.Path:
                            if (item.Value.Path.Equals(normalizedPath, StringComparison.InvariantCultureIgnoreCase))
                            {
                                return new RedirectResult
                                {
                                    Destination = item.Value.Destination,
                                    HttpCode = item.Value.HttpCode ?? _httpCodeResult
                                };
                            }
                            break;
                        case RedirectType.Regex:

                            var matchResult = item.Value.Regex.Match(path);

                            if (matchResult.Success)
                            {
                                return new RedirectResult
                                {
                                    Destination = matchResult.Result(item.Value.Destination),
                                    HttpCode = item.Value.HttpCode ?? _httpCodeResult
                                };
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new RedirectorException(null, ex), $"An unhandled exception has been occurred. path: {request.Path}");
            }

            return null;
        }

        public void Reload()
        {
            RedirectObjects = null;
        }

        public void AddOrUpdate(List<RedirectObject> redirects)
        {
            if (RedirectObjects == null)
                return;

            foreach (var redirectObject in redirects)
            {
                var AddOrUpdateValue = new CachedRedirectObject(redirectObject, NormalizePath(redirectObject.Path));

                RedirectObjects?.AddOrUpdate(redirectObject.Id, AddOrUpdateValue, (key, item) => AddOrUpdateValue);
            }
        }

        public void Remove(string[] ids)
        {
            if (RedirectObjects == null)
                return;

            foreach (var id in ids)
            {
                RedirectObjects.TryRemove(id, out _);
            }
        }

        private void TryInitialize()
        {
            Monitor.Enter(_lock);

            try
            {
                if (RedirectObjects == null)
                {
                    InitializeData();
                }
            }
            catch (Exception ex)
            {
                RedirectObjects = new ConcurrentDictionary<string, CachedRedirectObject>();
                _logger.LogError(ex, "Redirector initialize failed.Redirector is now disabled.");
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        private void InitializeData()
        {
            using (var scop = _serviceProvider.CreateScope())
            {
                var redirectorStorage = scop.ServiceProvider.GetRequiredService<IRedirectorStorage>();

                var redirects = redirectorStorage.GetAll();

                var redirectObjects = new Dictionary<string, CachedRedirectObject>();

                foreach (var item in redirects.OrderBy(c => c.Order).ToList())
                {
                    redirectObjects.Add(item.Id.ToString(), new CachedRedirectObject(item, NormalizePath(item.Path)));
                }

                RedirectObjects = new ConcurrentDictionary<string, CachedRedirectObject>(redirectObjects);
            }
        }

        private string NormalizePath(string value)
        {
            return HttpUtility.UrlDecode(value.ToUpperInvariant().TrimEnd().TrimEnd('/'));
        }

    }
}