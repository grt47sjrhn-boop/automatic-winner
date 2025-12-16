using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;

namespace substrate_shared.Mappers
{
    public static class RarityMapper
    {
        public static CrystalRarity ToCrystalRarity(IRarityTier tier)
        {
            return tier.Tier switch
            {
                "Common"    => CrystalRarity.Common,
                "Rare"      => CrystalRarity.Rare,
                "Epic"      => CrystalRarity.Epic,
                "Mythic"    => CrystalRarity.Mythic,
                "UltraRare" => CrystalRarity.UltraRare,
                _           => CrystalRarity.Common
            };
        }
    }
}