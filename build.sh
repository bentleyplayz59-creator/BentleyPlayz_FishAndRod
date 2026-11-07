#!/usr/bin/env bash
# Builds BentleyPlayz.BentleyFishCore.dll and assembles the installable mod zip.
set -e
cd "$(dirname "$0")"

command -v dotnet >/dev/null 2>&1 || { echo "ERROR: dotnet SDK (6.0 or newer) not found. See README.txt"; exit 1; }

missing=0
for f in "StardewModdingAPI.dll" "MonoGame.Framework.dll" "xTile.dll" "StardewValley.GameData.dll"; do
  [ -f "refs/$f" ] || { echo "Missing refs/$f"; missing=1; }
done
{ [ -f "refs/Stardew Valley.dll" ] || [ -f "refs/StardewValley.dll" ]; } || { echo "Missing refs/Stardew Valley.dll (or StardewValley.dll)"; missing=1; }
[ "$missing" = 0 ] || { echo "Copy the game DLLs into refs/ (see README.txt)."; exit 1; }

rm -rf build stage dist
dotnet build src/BentleyFishCore.csproj -c Release -o build/out

mkdir -p stage dist
cp -r mod/BentleyPlayz_FishAndRod stage/
cp build/out/BentleyPlayz.BentleyFishCore.dll stage/BentleyPlayz_FishAndRod/BentleyFish/

python3 - <<'PY'
import shutil
shutil.make_archive("dist/BentleyPlayz_FishAndRod_1_1_0", "zip", "stage")
print("Built dist/BentleyPlayz_FishAndRod_1_1_0.zip")
PY
