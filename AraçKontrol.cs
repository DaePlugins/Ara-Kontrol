using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using HarmonyLib;

namespace DaeAracKontrol
{
	public class AraçKontrol : RocketPlugin<AraçKontrolYapılandırma>
	{
	    public static AraçKontrol Örnek { get; private set; }
	    private Harmony _harmony;

        protected override void Load()
		{
		    Örnek = this;
            
            if (_harmony == null)
            {
                _harmony = new Harmony("dae.arackontrol");
            }

            if (Configuration.Instance.PatlayanAraçlarSilinsin)
            {

                _harmony.Patch(AccessTools.Method(typeof(InteractableVehicle), "explode"),
                    new HarmonyMethod(AccessTools.Method(typeof(Yamalar), "AraçPatlamadanÖnce")),
                    new HarmonyMethod(AccessTools.Method(typeof(Yamalar), "AraçPatladıktanSonra")));
            }

            if (!Configuration.Instance.HerkesAraçlaraHasarVerebilir)
            {
                VehicleManager.onDamageVehicleRequested += AracaHasarVerilmekİstendiğinde;
            }

            if (!Configuration.Instance.HerkesTekerlereHasarVerebilir)
            {
                VehicleManager.onDamageTireRequested += TekereHasarVerilmekİstendiğinde;
            }

            if (!Configuration.Instance.HerkesCarjackKullanabilir)
            {
                VehicleManager.onVehicleCarjacked += CarjackKullanılmakİstendiğinde;
            }

			if (!Configuration.Instance.HerkesAraçlarıTamirEdebilir)
			{
                _harmony.PatchAll();
			}

            if (!Configuration.Instance.HerkesBenzinDoldurabilir || !Configuration.Instance.HerkesBenzinAlabilir)
            {
                _harmony.Patch(AccessTools.Method(typeof(UseableFuel), "fire"),
                    new HarmonyMethod(AccessTools.Method(typeof(Yamalar), "EşyaKullanılmadanÖnce")));
            }
		}

        protected override void Unload()
	    {
	        Örnek = null;
            
			if (Configuration.Instance.PatlayanAraçlarSilinsin || !Configuration.Instance.HerkesAraçlarıTamirEdebilir
                                                               || !Configuration.Instance.HerkesBenzinDoldurabilir || !Configuration.Instance.HerkesBenzinAlabilir)
			{
			    _harmony.UnpatchAll("dae.arackontrol");
			    _harmony = null;
			}

            if (!Configuration.Instance.HerkesAraçlaraHasarVerebilir)
            {
                VehicleManager.onDamageVehicleRequested -= AracaHasarVerilmekİstendiğinde;
            }

            if (!Configuration.Instance.HerkesTekerlereHasarVerebilir)
            {
                VehicleManager.onDamageTireRequested -= TekereHasarVerilmekİstendiğinde;
            }

            if (!Configuration.Instance.HerkesCarjackKullanabilir)
            {
                VehicleManager.onVehicleCarjacked -= CarjackKullanılmakİstendiğinde;
            }
	    }

		private void AracaHasarVerilmekİstendiğinde(CSteamID hasarıVeren, InteractableVehicle hasarıAlanAraç, ref ushort hasar, ref bool tamirEdilebilir, ref bool hasarVerebilir, EDamageOrigin kaynak)
        {
            var oyuncu = UnturnedPlayer.FromCSteamID(hasarıVeren);
		    if (oyuncu == null || oyuncu.HasPermission($"dae.arackontrol.{Configuration.Instance.AraçlaraHasarYetkisi}"))
		    {
		        return;
		    }

            var araç = Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == hasarıAlanAraç.id);
            if (Configuration.Instance.ÖzelAraçlarAktif && (Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.ah"))
            {
                return;
            }
            
            hasarVerebilir = false;
		}
		
		private void TekereHasarVerilmekİstendiğinde(CSteamID hasarıVeren, InteractableVehicle hasarıAlanAraç, int tekerSırası, ref bool hasarVerebilir, EDamageOrigin kaynak)
		{
            var oyuncu = UnturnedPlayer.FromCSteamID(hasarıVeren);
		    if (oyuncu == null || oyuncu.HasPermission($"dae.arackontrol.{Configuration.Instance.TekerlereHasarYetkisi}"))
		    {
		        return;
		    }

            var araç = Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == hasarıAlanAraç.id);
            if (Configuration.Instance.ÖzelAraçlarAktif && (Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.th"))
            {
                return;
            }
            
            hasarVerebilir = false;
		}

        private void CarjackKullanılmakİstendiğinde(InteractableVehicle carjacklananAraç, Player carjackıKullanan, ref bool carjackKullanılabilir, ref Vector3 kuvvet, ref Vector3 tork)
        {
            var oyuncu = UnturnedPlayer.FromPlayer(carjackıKullanan);
            if (oyuncu.HasPermission($"dae.arackontrol.{Configuration.Instance.CarjackYetkisi}"))
            {
                return;
            }

            var araç = Configuration.Instance.ÖzelAraçlar.FirstOrDefault(a => a == carjacklananAraç.id);
            if (Configuration.Instance.ÖzelAraçlarAktif && (Configuration.Instance.ÖzelAraçlardaVarsaİzinVer ? araç != 0 : araç == 0) || oyuncu.HasPermission($"dae.arackontrol.{araç}.c"))
            {
                return;
            }
            
            oyuncu.Player.equipment.dequip();

            carjackKullanılabilir = false;
        }
	}
}