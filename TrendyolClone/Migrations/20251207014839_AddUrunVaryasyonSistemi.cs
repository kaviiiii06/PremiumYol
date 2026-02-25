using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendyolClone.Migrations
{
    /// <inheritdoc />
    public partial class AddUrunVaryasyonSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IslemKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    YoneticiId = table.Column<int>(type: "INTEGER", nullable: true),
                    Islem = table.Column<string>(type: "TEXT", nullable: true),
                    VarlikTipi = table.Column<string>(type: "TEXT", nullable: true),
                    VarlikId = table.Column<int>(type: "INTEGER", nullable: true),
                    EskiDeger = table.Column<string>(type: "TEXT", nullable: true),
                    YeniDeger = table.Column<string>(type: "TEXT", nullable: true),
                    IpAdresi = table.Column<string>(type: "TEXT", nullable: true),
                    KullaniciAjanı = table.Column<string>(type: "TEXT", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IslemKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true),
                    ResimUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OdemeYontemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tip = table.Column<int>(type: "INTEGER", nullable: false),
                    BankaAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IbanNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    HesapSahibi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EkBilgi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdemeYontemleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SiteAciklamasi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LogoIcon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IletisimEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IletisimTelefon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    AltBilgiMetni = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAyarlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tedarikciler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Tip = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiAnahtari = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ApiGizliAnahtari = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IletisimEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IletisimTelefon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    KomisyonOrani = table.Column<decimal>(type: "TEXT", nullable: false),
                    TeslimatGunu = table.Column<int>(type: "INTEGER", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tedarikciler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yoneticiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yoneticiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Sifre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TelefonNumarasi = table.Column<string>(type: "TEXT", nullable: true),
                    ProfilFotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    RolId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Roller_RolId",
                        column: x => x.RolId,
                        principalTable: "Roller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: false),
                    Fiyat = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IndirimliFiyat = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    ResimUrl = table.Column<string>(type: "TEXT", nullable: false),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Stok = table.Column<int>(type: "INTEGER", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Marka = table.Column<string>(type: "TEXT", nullable: true),
                    Puan = table.Column<double>(type: "REAL", nullable: false),
                    YorumSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    TedarikciId = table.Column<int>(type: "INTEGER", nullable: true),
                    TedarikciUrunId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TedarikciUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Urunler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Urunler_Tedarikciler_TedarikciId",
                        column: x => x.TedarikciId,
                        principalTable: "Tedarikciler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Adresler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TamAd = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AdresSatiri = table.Column<string>(type: "TEXT", nullable: false),
                    Sehir = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ilce = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PostaKodu = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    TelefonNumarasi = table.Column<string>(type: "TEXT", nullable: false),
                    Varsayilan = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adresler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    TeslimatAdresi = table.Column<string>(type: "TEXT", nullable: true),
                    OdemeYontemi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparisler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoriler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoriler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KargoOlculeri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    En = table.Column<decimal>(type: "TEXT", nullable: false),
                    Boy = table.Column<decimal>(type: "TEXT", nullable: false),
                    Yukseklik = table.Column<decimal>(type: "TEXT", nullable: false),
                    Agirlik = table.Column<decimal>(type: "TEXT", nullable: false),
                    UcretsizKargo = table.Column<bool>(type: "INTEGER", nullable: false),
                    KargoUcreti = table.Column<decimal>(type: "TEXT", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoOlculeri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KargoOlculeri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SepetUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    Adet = table.Column<int>(type: "INTEGER", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepetUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunOzellikleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    OzellikAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OzellikDegeri = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunOzellikleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunOzellikleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunSorulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Soru = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Cevaplandi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunSorulari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunSorulari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrunSorulari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunVaryasyonlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Renk = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Beden = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DigerOzellik = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Fiyat = table.Column<decimal>(type: "TEXT", nullable: false),
                    IndirimliFiyat = table.Column<decimal>(type: "TEXT", nullable: true),
                    Stok = table.Column<int>(type: "INTEGER", nullable: false),
                    MinStok = table.Column<int>(type: "INTEGER", nullable: false),
                    AnaResim = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunVaryasyonlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunVaryasyonlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Yorumlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Puan = table.Column<int>(type: "INTEGER", nullable: false),
                    Icerik = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Onaylandi = table.Column<bool>(type: "INTEGER", nullable: false),
                    DogrulanmisSatin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yorumlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OdemeIslemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    OdemeId = table.Column<string>(type: "TEXT", nullable: true),
                    IslemId = table.Column<string>(type: "TEXT", nullable: true),
                    Tutar = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: true),
                    OdemeYontemi = table.Column<string>(type: "TEXT", nullable: true),
                    KartSonDortHane = table.Column<string>(type: "TEXT", nullable: true),
                    HataKodu = table.Column<string>(type: "TEXT", nullable: true),
                    HataMesaji = table.Column<string>(type: "TEXT", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdemeIslemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdemeIslemleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisKalemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    Miktar = table.Column<int>(type: "INTEGER", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisKalemleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunCevaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SoruId = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    YoneticiId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cevap = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Resmi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunCevaplari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunCevaplari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UrunCevaplari_UrunSorulari_SoruId",
                        column: x => x.SoruId,
                        principalTable: "UrunSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrunCevaplari_Yoneticiler_YoneticiId",
                        column: x => x.YoneticiId,
                        principalTable: "Yoneticiler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UrunResimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    VaryasyonId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResimUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false),
                    AnaResim = table.Column<bool>(type: "INTEGER", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunResimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunResimleri_UrunVaryasyonlari_VaryasyonId",
                        column: x => x.VaryasyonId,
                        principalTable: "UrunVaryasyonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UrunResimleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adresler_KullaniciId",
                table: "Adresler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_KullaniciId_UrunId",
                table: "Favoriler",
                columns: new[] { "KullaniciId", "UrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_UrunId",
                table: "Favoriler",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_KargoOlculeri_UrunId",
                table: "KargoOlculeri",
                column: "UrunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_RolId",
                table: "Kullanicilar",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_OdemeIslemleri_SiparisId",
                table: "OdemeIslemleri",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_KullaniciId_UrunId",
                table: "SepetUrunleri",
                columns: new[] { "KullaniciId", "UrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_UrunId",
                table: "SepetUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemleri_SiparisId",
                table: "SiparisKalemleri",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemleri_UrunId",
                table: "SiparisKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KullaniciId",
                table: "Siparisler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunCevaplari_KullaniciId",
                table: "UrunCevaplari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunCevaplari_SoruId",
                table: "UrunCevaplari",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunCevaplari_YoneticiId",
                table: "UrunCevaplari",
                column: "YoneticiId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriId",
                table: "Urunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_TedarikciId",
                table: "Urunler",
                column: "TedarikciId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunOzellikleri_UrunId",
                table: "UrunOzellikleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunResimleri_UrunId",
                table: "UrunResimleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunResimleri_VaryasyonId",
                table: "UrunResimleri",
                column: "VaryasyonId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunSorulari_KullaniciId",
                table: "UrunSorulari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunSorulari_UrunId",
                table: "UrunSorulari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunVaryasyonlari_SKU",
                table: "UrunVaryasyonlari",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunVaryasyonlari_UrunId",
                table: "UrunVaryasyonlari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_KullaniciId",
                table: "Yorumlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_UrunId",
                table: "Yorumlar",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresler");

            migrationBuilder.DropTable(
                name: "Favoriler");

            migrationBuilder.DropTable(
                name: "IslemKayitlari");

            migrationBuilder.DropTable(
                name: "KargoOlculeri");

            migrationBuilder.DropTable(
                name: "OdemeIslemleri");

            migrationBuilder.DropTable(
                name: "OdemeYontemleri");

            migrationBuilder.DropTable(
                name: "SepetUrunleri");

            migrationBuilder.DropTable(
                name: "SiparisKalemleri");

            migrationBuilder.DropTable(
                name: "SiteAyarlari");

            migrationBuilder.DropTable(
                name: "UrunCevaplari");

            migrationBuilder.DropTable(
                name: "UrunOzellikleri");

            migrationBuilder.DropTable(
                name: "UrunResimleri");

            migrationBuilder.DropTable(
                name: "Yorumlar");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "UrunSorulari");

            migrationBuilder.DropTable(
                name: "Yoneticiler");

            migrationBuilder.DropTable(
                name: "UrunVaryasyonlari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Roller");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Tedarikciler");
        }
    }
}
