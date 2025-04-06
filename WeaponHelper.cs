using Mutagen.Bethesda.Skyrim;
using System;
using System.Linq;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Order;
using System.IO;
using System.Text;
using Mutagen.Bethesda.Synthesis;

namespace AnimatedArmouryRebalancer
{
    public static class WeaponHelper
    {
        public static string GetWeaponMaterial(IWeaponGetter weapon, ILinkCache linkCache, bool includeWACCF)
        {
            // Check if this is a bound weapon first
            if (weapon.EditorID != null)
            {
                string editorIdLower = weapon.EditorID.ToLower();
                
                // Special case for Dragon Priest Claws
                if (editorIdLower == "dragonpriestclaws" || editorIdLower == "dragonpriestclawsleft")
                {
                    return "orcish";
                }
                
                if (editorIdLower.Contains("bound"))
                {
                    // If it contains "mystic", it's a daedric bound weapon
                    if (editorIdLower.Contains("mystic"))
                    {
                        return "daedric";
                    }
                    // Otherwise it's a dwarven bound weapon
                    return "dwarven";
                }
            }

            // If not a bound weapon, proceed with normal material detection
            if (weapon.Keywords == null) return "steel";

            foreach (var keyword in weapon.Keywords)
            {
                // Skip if we can't resolve the keyword
                if (!linkCache.TryResolve(keyword, out var resolvedKeyword)) continue;
                
                var keywordName = resolvedKeyword.EditorID?.ToLower() ?? "";
                
                // Check for WACCF material keywords first if enabled
                if (includeWACCF && keywordName.Contains("waccf_weaponmaterial"))
                {
                    return keywordName.Replace("waccf_weaponmaterial", "").Trim();
                }
                
                // Check for any material keyword
                if (keywordName.Contains("material"))
                {
                    // Extract the material name by taking everything after "material"
                    int materialIndex = keywordName.IndexOf("material");
                    if (materialIndex >= 0 && materialIndex + "material".Length < keywordName.Length)
                    {
                        return keywordName.Substring(materialIndex + "material".Length).Trim();
                    }
                }
            }

            return "steel"; // Default material
        }
        

        /// <summary>
        /// Gets the damage offset for a weapon based on its material
        /// </summary>
        /// <param name="material">The weapon material</param>
        /// <param name="includeWACCF">Whether to include WACCF material offsets</param>
        /// <returns>The damage offset for the weapon</returns>
        public static int GetDamageOffset(string material, bool includeWACCF = false)
        {
            if (string.IsNullOrEmpty(material)) return 0;

            if (includeWACCF)
            {
                switch (material.ToLower())
                {
                    case "iron": return 0;
                    case "riekling": return -1;
                    case "steel": return 1;
                    case "silver": return 1;
                    case "draugr": return 1;
                    case "imperial": return 1;
                    case "orcish": return 4;
                    case "dragonpriest": return 2;
                    case "dwarven": return 2;
                    case "falmer": return 3;
                    case "forsworn": return 2;
                    case "dawnguard": return 3;
                    case "skyforge": return 4;
                    case "elven": return 3;
                    case "nordic": return 4;
                    case "blades": return 4;
                    case "draugrhoned": return 4;
                    case "redguard": return 4;
                    case "glass": return 5;
                    case "falmerhoned": return 5;
                    case "ebony": return 6;
                    case "stalhrim": return 6;
                    case "daedric": return 8;
                    case "dragonbone": return 7;
                    default: return 0;
                }
            }
            else
            {
                switch (material.ToLower())
                {
                    case "iron": return 0;
                    case "riekling": return -1;
                    case "steel": return 1;
                    case "silver": return 1;
                    case "draugr": return 1;
                    case "imperial": return 2;
                    case "orcish": return 2;
                    case "dragonpriest": return 2;
                    case "dwarven": return 3;
                    case "falmer": return 3;
                    case "forsworn": return 3;
                    case "dawnguard": return 3;
                    case "nordhero": return 4;
                    case "skyforge": return 4;
                    case "elven": return 4;
                    case "nordic": return 4;
                    case "blades": return 4;
                    case "draugrhoned": return 4;
                    case "redguard": return 4;
                    case "glass": return 5;
                    case "falmerhoned": return 5;
                    case "ebony": return 6;
                    case "stalhrim": return 6;
                    case "tempest": return 6;
                    case "daedric": return 7;
                    case "dragonbone": return 8;
                    default: return 0;
                }
            }
        }

        public static WeaponStats? GetWeaponStats(string weaponType, string material, bool includeWaccf)
        {
            WeaponStats? stats = GetBaseWeaponStats(weaponType);
            if (stats == null) return null;

            // Apply material-specific modifications
            switch (material.ToLower())
            {
                case "stalhrim":
                    // Only apply Stalhrim stagger bonus if WACCF is not enabled
                    if (!includeWaccf)
                    {
                        stats.Stagger += 0.1f;
                    }
                    break;
                // Add other material-specific modifications here
            }

            // Apply damage offset based on material
            int damageOffset = GetDamageOffset(material, includeWaccf);
            stats.BaseDamage += damageOffset;

            return stats;
        }

        private static WeaponStats? GetBaseWeaponStats(string weaponType)
        {
            // Parameters for each weapon type: Speed, Reach, Stagger, BaseDamage, CritDamage
            switch (weaponType.ToLower())
            {
                case "claw":
                    return new WeaponStats(1.2f, 0.7f, 0.0f, 5, 1);
                case "rapier":
                    return new WeaponStats(1.15f, 1.1f, 0.6f, 5, 5);
                case "katana":
                    return new WeaponStats(1.125f, 1.0f, 0.75f, 7, 3);
                case "whip":
                    return new WeaponStats(0.9f, 2.0f, 0.4f, 7, 1);
                case "pike":
                    return new WeaponStats(0.7f, 1.7f, 1.0f, 12, 7);
                case "quarterstaff":
                    return new WeaponStats(1.1f, 1.2f, 1.0f, 10, 4);
                case "halberd":
                    return new WeaponStats(0.65f, 1.55f, 1.1f, 15, 8);
                default:
                    return null;
            }
        }

        public static string? GetWeaponTypeFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            name = name.ToLower();

            // Check for weapon type in the name
            if (name.Contains("dagger")) return "dagger";
            if (name.Contains("sword")) return "sword";
            if (name.Contains("war axe") || name.Contains("waraxe")) return "waraxe";
            if (name.Contains("mace")) return "mace";
            if (name.Contains("greatsword")) return "greatsword";
            if (name.Contains("battleaxe") || name.Contains("battle axe")) return "battleaxe";
            if (name.Contains("warhammer") || name.Contains("war hammer")) return "warhammer";
            if (name.Contains("spear")) return "spear";
            if (name.Contains("halberd")) return "halberd";
            if (name.Contains("quarterstaff") || name.Contains("quarter staff")) return "quarterstaff";
            if (name.Contains("claw")) return "claw";

            return null;
        }

        /// <summary>
        /// Gets the weapon type based on the weapon's keywords
        /// </summary>
        /// <param name="weapon">The weapon to check</param>
        /// <param name="linkCache">The link cache to resolve keywords</param>
        /// <returns>The weapon type, or null if no matching type is found</returns>
        public static string? GetWeaponTypeFromKeywords(IWeaponGetter weapon, ILinkCache linkCache)
        {
            if (weapon.Keywords == null) return null;

            foreach (var keyword in weapon.Keywords)
            {
                if (!linkCache.TryResolve(keyword, out var resolvedKeyword)) continue;
                
                string keywordName = resolvedKeyword.EditorID?.ToLower() ?? "";
                
                // Check for specific weapon type keywords
                if (keywordName == "weaptypeclaw") return "claw";
                if (keywordName == "weaptypehalberd") return "halberd";
                if (keywordName == "weaptypekatana") return "katana";
                if (keywordName == "weaptypepike") return "pike";
                if (keywordName == "weaptypeqtrstaff") return "quarterstaff";
                if (keywordName == "weaptyperapier") return "rapier";
                if (keywordName == "weaptypewhip") return "whip";
            }

            return null;
        }
    }
} 