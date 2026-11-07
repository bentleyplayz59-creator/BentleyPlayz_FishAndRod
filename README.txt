BentleyPlayz Fish & Rod 1.1.0 - SOURCE PROJECT
================================================
DO NOT put this folder in Stardew's Mods folder. Build it, then install the zip it makes.

WHAT'S NEW
- Bentley's Golden Lure (Willy's shop, 500g): attach to any rod; every catch is one of our 11 fish.
- The BentleyPlayz Rod is drawn in the farmer's hands while fishing.
- Same as before: rod boosts new-fish odds, raw Windex Fish / Tide Pod Trout hurt.

FOLDERS
  src/   C# code (BentleyFishCore.csproj, ModEntry.cs, ...)
  mod/   the full mod folder: [CP] BentleyFish content pack + BentleyFish code folder
  refs/  you put the game DLLs here (not included)
  build.sh  builds the DLL and makes dist/BentleyPlayz_FishAndRod_1_1_0.zip

BUILD
1. Install the .NET SDK (6.0 or newer). Termux: try  pkg install dotnet-sdk-8.0  (may need: pkg install tur-repo)
2. Copy these DLLs from a Stardew Valley 1.6.15 PC install (and SMAPI) into refs/:
     StardewModdingAPI.dll, Stardew Valley.dll, StardewValley.GameData.dll,
     MonoGame.Framework.dll, xTile.dll
   If they exist in that install also copy: Netcode.dll, BmFont.dll, Lidgren.Network.dll
3. Run:  bash build.sh
4. Install dist/BentleyPlayz_FishAndRod_1_1_0.zip. Delete the old BentleyPlayz_FishAndRod folder first.

If the build prints errors, paste them to Claude and it will fix the code.

NOT TESTED (written without a compiler or the game):
- The code has never been compiled.
- Rod-in-hands drawing positions are guesses. Adjust Rod offset X / Y in the config or
  Generic Mod Config Menu until it looks right.
- Golden Lure wear-out follows the game's normal tackle rules.
