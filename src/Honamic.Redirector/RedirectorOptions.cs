﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Honamic.Redirector
{
    public class RedirectorOptions
    {
        public RedirectorOptions()
        {
            RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            ForceLowercaseUrls = false;
            ForceHttps = false;
            TrailingSlash = TrailingSlashAction.NoAction;
            WwwMode = WwwModeAction.NoAction;
        }

        public int RedirectStatusCode { get; set; }

        public bool ForceLowercaseUrls { get; set; }

        public bool ForceHttps { get; set; }

        public TrailingSlashAction TrailingSlash { get; set; }

        public WwwModeAction WwwMode { get; set; }
    }

    public enum TrailingSlashAction
    {
        NoAction = 0,
        ForceToStrip = 1,
        ForceToAppend = 2
    }

    public enum WwwModeAction
    {
        NoAction = 0,
        ForceToWww = 1,
        ForceToNonWww = 2
    }
}
