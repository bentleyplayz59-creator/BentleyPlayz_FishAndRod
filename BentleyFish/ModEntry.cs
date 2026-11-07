using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BentleyPlayz.BentleyFish
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private const string RodQualifiedId = "(T)BentleyPlayz.BentleyFish_Rod";

        private static readonly HashSet<string> RawDangerFish = new HashSet<string>
        {
            "(O)BentleyPlayz.BentleyFish_Windex",
            "(O)BentleyPlayz.BentleyFish_TidePodTrout"
        };

        private ModConfig Config;
        private bool WasEating;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            if (!this.Config.ModEnabled)
                this.Monitor.Log("Mod is turned off in config.json (ModEnabled = false). No fish or rod will be added.", LogLevel.Info);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // custom game state query used by the [CP] content pack for the rod's catch-chance boost
            GameStateQuery.Register("BentleyPlayz.BentleyFish_HOLDING_ROD", (query, context) => this.HasRod(context.Player ?? Game1.player));

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // token the [CP] content pack uses to turn all of its content on/off: {{BentleyPlayz.BentleyFishCore/Enabled}}
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (api == null)
            {
                this.Monitor.Log("Could not find the Content Patcher API; the fish content will not load.", LogLevel.Error);
                return;
            }
            api.RegisterToken(this.ModManifest, "Enabled", () => new[] { this.Config.ModEnabled ? "true" : "false" });
        }

        private bool HasRod(Farmer player)
        {
            if (player == null || !this.Config.ModEnabled)
                return false;

            if (this.Config.RequireRodInHands)
                return player.CurrentTool?.QualifiedItemId == RodQualifiedId;

            foreach (Item item in player.Items)
            {
                if (item?.QualifiedItemId == RodQualifiedId)
                    return true;
            }
            return false;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !this.Config.ModEnabled)
            {
                this.WasEating = false;
                return;
            }

            Farmer player = Game1.player;
            bool eating = player.isEating;

            if (eating && !this.WasEating && this.Config.BonusRawFishDamage > 0)
            {
                string id = player.itemToEat?.QualifiedItemId;
                if (id != null && RawDangerFish.Contains(id))
                    this.HurtPlayer(player);
            }

            this.WasEating = eating;
        }

        private void HurtPlayer(Farmer player)
        {
            // never kill the player
            int damage = Math.Min(this.Config.BonusRawFishDamage, player.health - 1);
            if (damage <= 0)
                return;

            player.health -= damage;
            Game1.playSound("ow");
            Game1.addHUDMessage(new HUDMessage("Yuck! That should have been cooked first.", HUDMessage.error_type));
        }
    }
}
