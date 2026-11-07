namespace BentleyPlayz.BentleyFish
{
    /// <summary>The mod settings (config.json).</summary>
    public class ModConfig
    {
        /// <summary>Master switch. If false, the whole mod is off: no new fish, no BentleyPlayz Rod in the shop, no recipe, no extra damage. Restart the game after changing it.</summary>
        public bool ModEnabled { get; set; } = true;

        /// <summary>If true, the BentleyPlayz Rod only boosts catches while it is the tool in your hands. If false, having it in your backpack is enough.</summary>
        public bool RequireRodInHands { get; set; } = true;

        /// <summary>Extra health lost when eating a raw Windex Fish or Tide Pod Trout, on top of the normal food damage. Set 0 to turn off.</summary>
        public int BonusRawFishDamage { get; set; } = 10;
    }
}
