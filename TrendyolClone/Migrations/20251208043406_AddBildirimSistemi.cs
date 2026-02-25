using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendyolClone.Migrations
{
    /// <inheritdoc />
    public partial class AddBildirimSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AramaGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AramaTerimi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SonucSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: true),
                    AramaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAdresi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AramaGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AramaGecmisleri_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AramaGecmisleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    Turu = table.Column<int>(type: "INTEGER", nullable: false),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TelefonNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GonderimTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HataMesaji = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SablonKodu = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Parametreler = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BildirimSablonlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Turu = table.Column<int>(type: "INTEGER", nullable: false),
                    Konu = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "TEXT", nullable: false),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BildirimSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BildirimTercihleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmsBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    SiparisBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    KampanyaBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    UrunBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    IadeBildirimleri = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BildirimTercihleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BildirimTercihleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    FaturaNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FaturaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FirmaUnvani = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    VergiDairesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    VergiNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Adres = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AraToplam = table.Column<decimal>(type: "TEXT", nullable: false),
                    KDV = table.Column<decimal>(type: "TEXT", nullable: false),
                    KargoUcreti = table.Column<decimal>(type: "TEXT", nullable: false),
                    IndirimTutari = table.Column<decimal>(type: "TEXT", nullable: false),
                    Toplam = table.Column<decimal>(type: "TEXT", nullable: false),
                    PdfUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EFaturaMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    EFaturaUuid = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturalar_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Iadeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Neden = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    IadeKargoTakipNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IadeKargoTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IadeTutari = table.Column<decimal>(type: "TEXT", nullable: false),
                    OdemeYapildiMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RedNedeni = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TalepTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iadeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iadeler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Iadeler_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KargoTakipler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    KargoFirmasi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TakipNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    KargoDurumu = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TahminiTeslimatTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoTakipler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KargoTakipler_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PopulerAramalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AramaTerimi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AramaSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    SonAramaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: true),
                    GunlukAramaSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    HaftalikAramaSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    AylikAramaSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopulerAramalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PopulerAramalar_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SiparisDurumGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: false),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    DegistirenKisi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisDurumGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisDurumGecmisleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SiparisDurumGecmisleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AramaTiklamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AramaGecmisiId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TiklamaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AramaTiklamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AramaTiklamalari_AramaGecmisleri_AramaGecmisiId",
                        column: x => x.AramaGecmisiId,
                        principalTable: "AramaGecmisleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AramaTiklamalari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IadeUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IadeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    Adet = table.Column<int>(type: "INTEGER", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IadeUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IadeUrunleri_Iadeler_IadeId",
                        column: x => x.IadeId,
                        principalTable: "Iadeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IadeUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KargoHareketler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KargoTakipId = table.Column<int>(type: "INTEGER", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Lokasyon = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoHareketler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KargoHareketler_KargoTakipler_KargoTakipId",
                        column: x => x.KargoTakipId,
                        principalTable: "KargoTakipler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AramaGecmisleri_KategoriId",
                table: "AramaGecmisleri",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_AramaGecmisleri_KullaniciId",
                table: "AramaGecmisleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_AramaTiklamalari_AramaGecmisiId",
                table: "AramaTiklamalari",
                column: "AramaGecmisiId");

            migrationBuilder.CreateIndex(
                name: "IX_AramaTiklamalari_UrunId",
                table: "AramaTiklamalari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_Durum",
                table: "Bildirimler",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId_OlusturmaTarihi",
                table: "Bildirimler",
                columns: new[] { "KullaniciId", "OlusturmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_OlusturmaTarihi",
                table: "Bildirimler",
                column: "OlusturmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_BildirimSablonlari_Kod",
                table: "BildirimSablonlari",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BildirimTercihleri_KullaniciId",
                table: "BildirimTercihleri",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_SiparisId",
                table: "Faturalar",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_KullaniciId",
                table: "Iadeler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_SiparisId",
                table: "Iadeler",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeUrunleri_IadeId",
                table: "IadeUrunleri",
                column: "IadeId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeUrunleri_UrunId",
                table: "IadeUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_KargoHareketler_KargoTakipId",
                table: "KargoHareketler",
                column: "KargoTakipId");

            migrationBuilder.CreateIndex(
                name: "IX_KargoTakipler_SiparisId",
                table: "KargoTakipler",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_PopulerAramalar_KategoriId",
                table: "PopulerAramalar",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDurumGecmisleri_KullaniciId",
                table: "SiparisDurumGecmisleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDurumGecmisleri_SiparisId",
                table: "SiparisDurumGecmisleri",
                column: "SiparisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AramaTiklamalari");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "BildirimSablonlari");

            migrationBuilder.DropTable(
                name: "BildirimTercihleri");

            migrationBuilder.DropTable(
                name: "Faturalar");

            migrationBuilder.DropTable(
                name: "IadeUrunleri");

            migrationBuilder.DropTable(
                name: "KargoHareketler");

            migrationBuilder.DropTable(
                name: "PopulerAramalar");

            migrationBuilder.DropTable(
                name: "SiparisDurumGecmisleri");

            migrationBuilder.DropTable(
                name: "AramaGecmisleri");

            migrationBuilder.DropTable(
                name: "Iadeler");

            migrationBuilder.DropTable(
                name: "KargoTakipler");
        }
    }
}
