using UnityEngine;

namespace EndlessBeloved.Core
{
    /// <summary>
    /// Runtime variant detection for the dual-app system.
    ///
    /// HEAL_VERSION = Therapeutic version (CBT/DBT skills, softer tone)
    /// DARK_VERSION = Dark fantasy version (explicit content, no therapeutic framing)
    ///
    /// Compile-time: set via scripting defines HEAL_VERSION / DARK_VERSION
    /// Runtime: use AppVariant.IsHeal / AppVariant.IsDark for conditional logic
    /// Editor: Endless Beloved > Set Variant > Heal / Dark to switch
    /// </summary>
    public enum AppEdition
    {
        Heal,
        Dark
    }

    public static class AppVariant
    {
        /// <summary>
        /// The active edition. Determined at compile time by scripting defines,
        /// but can be overridden at runtime for testing.
        /// </summary>
        public static AppEdition Current
        {
            get
            {
                if (_override.HasValue) return _override.Value;
#if DARK_VERSION
                return AppEdition.Dark;
#else
                return AppEdition.Heal;
#endif
            }
        }

        private static AppEdition? _override;

        /// <summary>Override variant at runtime (for testing in editor).</summary>
        public static void SetOverride(AppEdition edition) => _override = edition;
        public static void ClearOverride() => _override = null;

        // ── Quick checks ────────────────────────────────────────────

        public static bool IsHeal => Current == AppEdition.Heal;
        public static bool IsDark => Current == AppEdition.Dark;

        // ── Variant-specific constants ──────────────────────────────

        public static string ProductName => IsHeal
            ? "Endless, Beloved: Heal"
            : "Endless, Beloved";

        public static string BundleId => IsHeal
            ? "com.endlessbeloved.heal"
            : "com.endlessbeloved.dark";

        /// <summary>Whether therapeutic skill tracking is active.</summary>
        public static bool TherapeuticFeaturesEnabled => IsHeal;

        /// <summary>Whether explicit/mature content beyond the base 18+ is shown.</summary>
        public static bool ExplicitContentEnabled => IsDark;

        /// <summary>Art asset prefix for variant-specific sprites.</summary>
        public static string ArtPrefix => IsHeal ? "heal_" : "dark_";

        /// <summary>Card back sprite name for this variant.</summary>
        public static string CardBackName => IsHeal ? "card_back_heal" : "card_back_dark";

        /// <summary>Scripting define used for this variant.</summary>
        public static string DefineSymbol => IsHeal ? "HEAL_VERSION" : "DARK_VERSION";
    }
}
