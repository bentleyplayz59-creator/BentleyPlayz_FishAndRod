using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace BentleyPlayz.BentleyFish
{
    /// <summary>The subset of the Content Patcher API this mod uses.</summary>
    public interface IContentPatcherAPI
    {
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
    }
}
