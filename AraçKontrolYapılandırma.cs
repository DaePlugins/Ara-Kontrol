using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace DaeAracKontrol
{
    public class AraçKontrolYapılandırma : IRocketPluginConfiguration
    {
        public bool PatlayanAraçlarSilinsin { get; set; }

        public bool HerkesAraçlaraHasarVerebilir { get; set; }
        public string AraçlaraHasarYetkisi { get; set; }

        public bool HerkesTekerlereHasarVerebilir { get; set; }
        public string TekerlereHasarYetkisi { get; set; }

        public bool HerkesAraçlarıTamirEdebilir { get; set; }
        public string AraçlarıTamirYetkisi { get; set; }

        public bool HerkesCarjackKullanabilir { get; set; }
        public string CarjackYetkisi { get; set; }

        public bool HerkesBenzinDoldurabilir { get; set; }
        public string BenzinDoldurmaYetkisi { get; set; }

        public bool HerkesBenzinAlabilir { get; set; }
        public string BenzinAlmaYetkisi { get; set; }
	    
        public bool ÖzelAraçlarAktif { get; set; }
        public bool ÖzelAraçlardaVarsaİzinVer { get; set; }
        [XmlArrayItem("Araç")]
        public List<ushort> ÖzelAraçlar { get; set; } = new List<ushort>();

        public void LoadDefaults()
        {
            PatlayanAraçlarSilinsin = true;

            HerkesAraçlaraHasarVerebilir = false;
            AraçlaraHasarYetkisi = "AraçlaraHasarVerebilir";

            HerkesTekerlereHasarVerebilir = false;
            TekerlereHasarYetkisi = "TekerlereHasarVerebilir";

            HerkesAraçlarıTamirEdebilir = true;
            AraçlarıTamirYetkisi = "AraçTamirEdebilir";

            HerkesCarjackKullanabilir = true;
            CarjackYetkisi = "CarjackKullanabilir";
            
            HerkesBenzinDoldurabilir = false;
            BenzinDoldurmaYetkisi = "BenzinDoldurabilir";

            HerkesBenzinAlabilir = false;
            BenzinAlmaYetkisi = "BenzinAlabilir";
            
            ÖzelAraçlarAktif = true;
            ÖzelAraçlardaVarsaİzinVer = true;
            ÖzelAraçlar = new List<ushort>
            {
                1,
                25
            };
        }
    }
}