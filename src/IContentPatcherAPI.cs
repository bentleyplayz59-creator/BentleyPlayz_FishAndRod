using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace BentleyPlayz.BentleyFishCore
{
    public interface IContentPatcherAPI
    {
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
    }
}
