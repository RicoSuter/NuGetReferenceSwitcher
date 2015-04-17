// Guids.cs
// MUST match guids.h

using System;

namespace RicoSuter.NuGetReferenceSwitcher
{
    static class GuidList
    {
        public const string guidNuGetReferenceSwitcherPkgString = "87a7b0e9-e41c-47c8-953c-b81650401edb";
        public const string guidNuGetReferenceSwitcherCmdSetString = "ae6a9753-b188-45b5-8563-70bc990bf707";

        public static readonly Guid guidNuGetReferenceSwitcherCmdSet = new Guid(guidNuGetReferenceSwitcherCmdSetString);
    };
}