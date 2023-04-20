using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static ItemDrop;

namespace EmberTreeFeller
{
    [BepInPlugin("EmberTreeFeller", "Ember Tree Feller", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ValheimMod : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("EmberTreeFeller");
        private static ManualLogSource Logs;

        private void Awake()
        {
            harmony.PatchAll();
            Logs = Logger;
            Logs.LogInfo("Ready to blow up some trees!");
        }
		
		// Theoretical maximum of 180 chop at level 100 o.o

        [HarmonyPatch(typeof(ItemData))]
        class Ember_Staff_Patch
        {
            [HarmonyPatch(nameof(ItemData.GetDamage), typeof(int))]
            [HarmonyPostfix]
            static void PatchStaffChopDamage(ref HitData.DamageTypes __result, ref ItemData.SharedData ___m_shared)
            {
                var name = ___m_shared.m_name;
                if (name.ToLower().Equals("$item_stafffireball"))
                {
                    __result.m_chop = __result.m_fire * 1.5f;
                }
            }
        }

		// Since the Projectile OnHit() doesn't add toolTier to Hit Data, this workaround just patches in if the projectile did chop and fire damage
		// which will logically only be the staff. All previous tiers of axes (Stone, Flint, Bronze, Iron) only do Slash + Chop so you're not in
		// danger of illegitimately cutting Yddrassil wood with a Stone axe!
		
        [HarmonyPatch(typeof(TreeBase))]
        class Ember_Staff_TreeBase
        {
            [HarmonyPatch(nameof(TreeBase.Damage))]
            [HarmonyPrefix]
            static void PatchToolTier(ref HitData hit)
            {
                if(hit.m_damage.m_chop > 0 && hit.m_damage.m_fire > 0)
                {
                    hit.m_toolTier = 4;
                }
            }
        }

        [HarmonyPatch(typeof(TreeLog))]
        class Ember_Staff_TreeLog
        {
            [HarmonyPatch(nameof(TreeLog.Damage))]
            [HarmonyPrefix]
            static void PatchToolTier(ref HitData hit)
            {
                if (hit.m_damage.m_chop > 0 && hit.m_damage.m_fire > 0)
                {
                    hit.m_toolTier = 4;
                }
            }
        }
    }
}
