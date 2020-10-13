using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Honamic.Redirector
{
    public class RedirectorManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RedirectorManager> _logger;
        private readonly object _lock = new object();
        private HashSet<RedirectObject> RedirectObjects;
        private Dictionary<string, Regex> RegexPaths;

        public int _httpCodeResult = 307;

        public RedirectorManager(IServiceProvider serviceProvider, ILogger<RedirectorManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            RegexPaths = new Dictionary<string, Regex>();
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

                var normalizePath = NormalizePath(path);

                var serachedPath = FindExistPath(normalizePath);

                var matchedPath = FindMatchedPath(path);

                if (serachedPath != null)
                {
                    if (serachedPath.Order < matchedPath?.Redirect.Order || matchedPath == null)
                    {
                        return new RedirectResult
                        {
                            Destination = serachedPath.Destination,
                            HttpCode = serachedPath.HttpCode ?? _httpCodeResult
                        };
                    }
                }

                if (matchedPath.HasValue)
                {
                    return new RedirectResult
                    {
                        Destination = matchedPath.Value.MathResul.Result(matchedPath.Value.Redirect.Destination),
                        HttpCode = matchedPath.Value.Redirect.HttpCode ?? _httpCodeResult
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new RedirectorException(null, ex), $"An unhandled exception has been occurred. path: {request.Path}");
            }

            return null;
        }

        private (RedirectObject Redirect, Match MathResul)? FindMatchedPath(string path)
        {
            foreach (var item in RegexPaths)
            {
                var matchResult = item.Value.Match(path);

                if (matchResult.Success)
                {
                    var redirectPath = RedirectObjects.FirstOrDefault(c => c.Id == item.Key);

                    return (redirectPath, matchResult);
                }
            }

            return null;
        }

        private RedirectObject FindExistPath(string path)
        {
            return RedirectObjects
                  .Where(c => c.Type == RedirectType.Path)
                  .FirstOrDefault(c => c.Path == path);
        }

        public string NormalizePath(string value)
        {
            return HttpUtility.UrlDecode(value.ToUpperInvariant().TrimEnd().TrimEnd('/'));
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
                RedirectObjects = new HashSet<RedirectObject>();
                _logger.LogError(ex, "Redirector initialize failed.Redirector is now disabled.");
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        private void InitializeData()
        {
            //todo reload

            using (var scop = _serviceProvider.CreateScope())
            {
                var redirectorStorage = scop.ServiceProvider.GetRequiredService<IRedirectorStorage>();

                var redirects = redirectorStorage.GetAll();

                foreach (var item in redirects.Where(c => c.Type == RedirectType.Regex).OrderBy(c => c.Order).ToList())
                {
                    var regex = new Regex(item.Path, RegexOptions.Compiled, TimeSpan.FromSeconds(1));

                    RegexPaths.Add(item.Id, regex);
                }

                foreach (var item in redirects)
                {
                    item.Path = NormalizePath(item.Path);
                }

                RedirectObjects = new HashSet<RedirectObject>(redirects.OrderBy(d => d.Order));
            }
        }
    }
}