using System.Linq;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Harmony;

namespace DaeAracKontrol
{
    internal class Yamalar
    {
        private static void AraçPatlamadanÖnce(InteractableVehicle __instance) => typeof(VehicleAsset).GetProperty("dropsTableId").DeclaringType.GetProperty("dropsTableId").GetSetMethod(true).Invoke(__instance.asset, new object[]{ (ushort)0 });

        private static void AraçPatladıktanSonra(InteractableVehicle __instance) => VehicleManager.instance.channel.send("tellVehicleDestroy", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, __instance.instanceID);

        private static bool EşyaKullanılmadanÖnce(UseableFuel __instance, bool mode)
        {
            if (mode && AraçKontrol.Örnek.Configuration.Instance.HerkesBenzinDoldurabilir
                || !mode && AraçKontrol.Örnek.Configuration.Instance.HerkesBenzinAlabilir)
            {
                return true;
            }
            
            var oyuncu = UnturnedPlayer.FromPlayer(__instance.player);
            if (oyuncu == null || oyuncu.IsAdmin)
            {
                return true;
            }

            var ışınBilgisi = DamageTool.raycast(new Ray(oyuncu.Player.look.aim.position, oyuncu.Player.look.aim.forward), 3f, RayMasks.VEHICLE);
            if (ışınBilgisi.vehicle == null)
            {
                return true;
            }

            if (mode)
            {
                if (oyuncu.HasPermission($"dae.arackontrol.{AraçKontrol.Örnek.Configuration.Instance.BenzinDoldurmaYetkisi}"))
                {
                    return true;
                }
                
                var araç = AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == ışınBilgisi.vehicle.id);
                if (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlarAktif &&
                    (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.bd"))
                {
                    return true;
                }
            }
            else
            {
                if (oyuncu.HasPermission($"dae.arackontrol.{AraçKontrol.Örnek.Configuration.Instance.BenzinAlmaYetkisi}"))
                {
                    return true;
                }
                
                var araç = AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == ışınBilgisi.vehicle.id);
                if (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlarAktif &&
                    (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.ba"))
                {
                    return true;
                }
            }
            
            oyuncu.Player.equipment.dequip();

            return false;
        }
    }

    [HarmonyPatch(typeof(DamageTool))]
    [HarmonyPatch("damage")]
    [HarmonyPatch(new[]{ typeof(InteractableVehicle), typeof(bool), typeof(Vector3), typeof(bool), typeof(float), typeof(float), typeof(bool), typeof(EPlayerKill), typeof(CSteamID), typeof(EDamageOrigin) },
                  new[]{ ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
    internal class YamaHasar
    {
        [HarmonyPrefix]
        private static bool AracaHasarVerilmedenÖnce(InteractableVehicle vehicle, bool damageTires, bool isRepairing, CSteamID instigatorSteamID)
        {
            if (vehicle == null || isRepairing && vehicle.isRepaired || vehicle.isExploded || vehicle.isDead)
            {
                return false;
            }

            if (damageTires && !isRepairing)
            {
                return true;
            }

            var oyuncu = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (oyuncu == null || isRepairing && oyuncu.HasPermission($"dae.arackontrol.{AraçKontrol.Örnek.Configuration.Instance.AraçlarıTamirYetkisi}"))
            {
                return true;
            }

            var araç = AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == vehicle.id);
            if (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlarAktif &&
                (AraçKontrol.Örnek.Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.t"))
            {
                return true;
            }

            return false;
        }
    }
}