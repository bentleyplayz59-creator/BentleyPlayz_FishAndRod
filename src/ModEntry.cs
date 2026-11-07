using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace BentleyPlayz.BentleyFishCore
{
    /// <summary>Code part of BentleyPlayz Fish &amp; Rod 1.1.0.</summary>
    public class ModEntry : Mod
    {
        public const string RodQualifiedId = "(T)BentleyPlayz.BentleyFish_Rod";
        public const string TackleQualifiedId = "(O)BentleyPlayz.BentleyFish_GoldenLure";

        private static readonly HashSet<string> RawDangerFish = new HashSet<string>
        {
            "(O)BentleyPlayz.BentleyFish_Windex",
            "(O)BentleyPlayz.BentleyFish_TidePodTrout"
        };

        private static ModConfig Config = new ModConfig();
        private bool eatingHandled;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            Config.Normalize();

            // Game state queries used by the content pack's fish spawn data.
            GameStateQuery.Register("BentleyPlayz.BentleyFish_HOLDING_ROD",
                (string[] query, GameStateQueryContext context) => HoldingRod());
            GameStateQuery.Register("BentleyPlayz.BentleyFish_HAS_TACKLE",
                (string[] query, GameStateQueryContext context) => HasTackle());

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        }

        private static bool HoldingRod()
        {
            if (!Config.ModEnabled)
                return false;
            Farmer player = Game1.player;
            if (player == null)
                return false;

            if (Config.RequireRodInHands)
                return player.CurrentTool != null && player.CurrentTool.QualifiedItemId == RodQualifiedId;

            foreach (Item item in player.Items)
            {
                if (item != null && item.QualifiedItemId == RodQualifiedId)
                    return true;
            }
            return false;
        }

        private static bool HasTackle()
        {
            if (!Config.ModEnabled)
                return false;
            FishingRod rod = Game1.player?.CurrentTool as FishingRod;
            if (rod == null)
                return false;

            foreach (SObject attached in rod.attachments)
            {
                if (attached != null && attached.QualifiedItemId == TackleQualifiedId)
                    return true;
            }
            return false;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var cp = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (cp == null)
                this.Monitor.Log("Could not find the Content Patcher API; the fish content will not load.", LogLevel.Error);
            else
                cp.RegisterToken(this.ModManifest, "Enabled", () => new[] { Config.ModEnabled ? "true" : "false" });

            var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            gmcm.Register(this.ModManifest, () => Config = new ModConfig(), () => this.Helper.WriteConfig(Config));
            gmcm.AddBoolOption(this.ModManifest, () => Config.ModEnabled, v => Config.ModEnabled = v,
                () => "Mod enabled");
            gmcm.AddBoolOption(this.ModManifest, () => Config.RequireRodInHands, v => Config.RequireRodInHands = v,
                () => "Rod must be in hands", () => "The BentleyPlayz Rod only boosts catches while you hold it.");
            gmcm.AddNumberOption(this.ModManifest, () => Config.BonusRawFishDamage, v => Config.BonusRawFishDamage = v,
                () => "Raw fish damage", () => "Extra damage for eating raw Windex Fish or Tide Pod Trout.", 0, 100, 1);
            gmcm.AddBoolOption(this.ModManifest, () => Config.DrawRodInHands, v => Config.DrawRodInHands = v,
                () => "Draw rod in hands", () => "Draws the BentleyPlayz Rod in the farmer's hands while fishing.");
            gmcm.AddNumberOption(this.ModManifest, () => Config.RodOffsetX, v => Config.RodOffsetX = v,
                () => "Rod offset X", () => "Nudge the in-hand rod left (negative) or right (positive).", -64, 64, 2);
            gmcm.AddNumberOption(this.ModManifest, () => Config.RodOffsetY, v => Config.RodOffsetY = v,
                () => "Rod offset Y", () => "Nudge the in-hand rod up (negative) or down (positive).", -64, 64, 2);
        }

        /// <summary>Extra damage when the player eats a raw Windex Fish or Tide Pod Trout.</summary>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            Farmer player = Game1.player;
            if (!player.isEating)
            {
                this.eatingHandled = false;
                return;
            }

            if (this.eatingHandled)
                return;

            Item eaten = player.itemToEat;
            if (eaten == null || !RawDangerFish.Contains(eaten.QualifiedItemId))
                return;

            this.eatingHandled = true;
            player.health = Math.Max(1, player.health - Math.Clamp(Config.BonusRawFishDamage, 0, 100));
            Game1.addHUDMessage(new HUDMessage("Yuck! That should have been cooked first.", 3));
        }

        /// <summary>Draws the rod in the farmer's hands while fishing (the game does not draw custom rods there).</summary>
        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled || !Config.DrawRodInHands)
                return;

            Farmer player = Game1.player;
            if (!player.UsingTool || player.CurrentTool == null || player.CurrentTool.QualifiedItemId != RodQualifiedId)
                return;

            ParsedItemData data = ItemRegistry.GetDataOrErrorItem(RodQualifiedId);
            Texture2D texture = data.GetTexture();
            Rectangle source = data.GetSourceRect();

            Vector2 local = player.getLocalPosition(Game1.viewport);
            Vector2 offset;
            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(0f, source.Height);

            switch (player.FacingDirection)
            {
                case 0: offset = new Vector2(28f, -60f); break;   // up
                case 1: offset = new Vector2(48f, -28f); break;   // right
                case 3:                                           // left
                    offset = new Vector2(16f, -28f);
                    effects = SpriteEffects.FlipHorizontally;
                    origin = new Vector2(source.Width, source.Height);
                    break;
                default: offset = new Vector2(36f, -20f); break;  // down
            }

            offset += new Vector2(Config.RodOffsetX, Config.RodOffsetY);
            e.SpriteBatch.Draw(texture, local + offset, source, Color.White, 0f, origin, 4f, effects, 1f);
        }
    }
}
