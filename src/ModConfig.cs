namespace BentleyPlayz.BentleyFishCore
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool RequireRodInHands { get; set; } = true;
        public int BonusRawFishDamage { get; set; } = 10;
        public bool DrawRodInHands { get; set; } = true;
        public int RodOffsetX { get; set; } = 0;
        public int RodOffsetY { get; set; } = 0;

        public void Normalize()
        {
            BonusRawFishDamage = Clamp(BonusRawFishDamage, 0, 100);
            RodOffsetX = Clamp(RodOffsetX, -64, 64);
            RodOffsetY = Clamp(RodOffsetY, -64, 64);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}
