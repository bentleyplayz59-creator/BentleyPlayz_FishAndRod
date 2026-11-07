# BentleyPlayz Fish & Rod (source)

Hybrid Stardew Valley mod: a Content Patcher pack (`[CP] BentleyFish`) plus a small SMAPI DLL (`BentleyFish`).

## Build (GitHub Actions, no local setup)
1. Create a GitHub repo and upload everything in this folder (keep the `.github` folder).
2. Push to `main`. Actions > "Build Stardew Valley Mod" runs by itself (or press "Run workflow").
3. When it's green, open the run, scroll to **Artifacts**, download `BentleyPlayz_FishAndRod`.
4. Unzip it into `Stardew Valley/Mods`. It contains `BentleyFish` (DLL) and `[CP] BentleyFish` (content). Needs SMAPI 4.x + Content Patcher.

## What the DLL does
- Registers the game state query `BentleyPlayz.BentleyFish_HOLDING_ROD`, used by the content pack so the 90% boost only applies while you hold the BentleyPlayz Rod.
- Extra health loss for eating raw Windex Fish / Tide Pod Trout.
- Settings in `BentleyFish/config.json` (restart the game after changing):
  - `ModEnabled`: master switch. `false` turns the whole mod off (no fish, rod, shop item, recipe or extra damage). Items you already own show as error items while it's off and come back when you turn it on.
  - `RequireRodInHands` (true/false)
  - `BonusRawFishDamage` (0 turns it off)

## Notes
- The DLL uses only game and SMAPI APIs (no Harmony), so it should be fine on Android, but a Windows/Linux build passing doesn't prove Android works.
- Edit fish chance, prices and recipes in `[CP] BentleyFish/content.json`. Art is in `[CP] BentleyFish/assets`.
