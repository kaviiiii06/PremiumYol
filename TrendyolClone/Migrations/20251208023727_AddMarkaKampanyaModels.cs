using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendyolClone.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkaKampanyaModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marka",
                table: "Urunler");

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                table: "UrunVaryasyonlari",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CanonicalUrl",
                table: "Urunler",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FavoriSayisi",
                table: "Urunler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoruntulenmeSayisi",
                table: "Urunler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MarkaAdi",
                table: "Urunler",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarkaId",
                table: "Urunler",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Urunler",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Urunler",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PopuleriteSkor",
                table: "Urunler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SatinAlmaSayisi",
                table: "Urunler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Urunler",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Kampanyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Turu = table.Column<int>(type: "INTEGER", nullable: false),
                    IndirimMiktari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaksimumIndirim = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumSepetTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumUrunAdedi = table.Column<int>(type: "INTEGER", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    BannerUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Oncelik = table.Column<int>(type: "INTEGER", nullable: false),
                    KullanimSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    ToplamIndirimTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kampanyalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KargoFirmalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Logo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ApiSecret = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    TemelUcret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KgBasinaUcret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DesiBasinaUcret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UcretsizKargoLimiti = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoFirmalari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KayitliSepetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KayitliSepetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KayitliSepetler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kuponlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IndirimTuru = table.Column<int>(type: "INTEGER", nullable: false),
                    IndirimMiktari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaksimumIndirim = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumSepetTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaksimumKullanimSayisi = table.Column<int>(type: "INTEGER", nullable: true),
                    KullanimSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciBasinaKullanimSayisi = table.Column<int>(type: "INTEGER", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuponlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sira = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrunKampanyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    KampanyaId = table.Column<int>(type: "INTEGER", nullable: false),
                    OzelIndirimMiktari = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StokLimiti = table.Column<int>(type: "INTEGER", nullable: true),
                    KullanilanStok = table.Column<int>(type: "INTEGER", nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunKampanyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunKampanyalari_Kampanyalar_KampanyaId",
                        column: x => x.KampanyaId,
                        principalTable: "Kampanyalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrunKampanyalari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KargoUcretleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KargoFirmaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CikisIli = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VarisIli = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ucret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TahminiGun = table.Column<int>(type: "INTEGER", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoUcretleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KargoUcretleri_KargoFirmalari_KargoFirmaId",
                        column: x => x.KargoFirmaId,
                        principalTable: "KargoFirmalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KayitliSepetUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KayitliSepetId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    VaryasyonId = table.Column<int>(type: "INTEGER", nullable: true),
                    Adet = table.Column<int>(type: "INTEGER", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KayitliSepetUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KayitliSepetUrunleri_KayitliSepetler_KayitliSepetId",
                        column: x => x.KayitliSepetId,
                        principalTable: "KayitliSepetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KayitliSepetUrunleri_UrunVaryasyonlari_VaryasyonId",
                        column: x => x.VaryasyonId,
                        principalTable: "UrunVaryasyonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KayitliSepetUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KuponKategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KuponId = table.Column<int>(type: "INTEGER", nullable: false),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KuponKategoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KuponKategoriler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KuponKategoriler_Kuponlar_KuponId",
                        column: x => x.KuponId,
                        principalTable: "Kuponlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KuponKullanimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KuponId = table.Column<int>(type: "INTEGER", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiparisId = table.Column<int>(type: "INTEGER", nullable: true),
                    IndirimTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KullanimTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SepetTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IpAdresi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KuponKullanimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KuponKullanimlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KuponKullanimlari_Kuponlar_KuponId",
                        column: x => x.KuponId,
                        principalTable: "Kuponlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KuponKullanimlari_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_MarkaId",
                table: "Urunler",
                column: "MarkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Slug",
                table: "Urunler",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kampanyalar_Slug",
                table: "Kampanyalar",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KargoUcretleri_KargoFirmaId_CikisIli_VarisIli",
                table: "KargoUcretleri",
                columns: new[] { "KargoFirmaId", "CikisIli", "VarisIli" });

            migrationBuilder.CreateIndex(
                name: "IX_KayitliSepetler_KullaniciId",
                table: "KayitliSepetler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KayitliSepetUrunleri_KayitliSepetId",
                table: "KayitliSepetUrunleri",
                column: "KayitliSepetId");

            migrationBuilder.CreateIndex(
                name: "IX_KayitliSepetUrunleri_UrunId",
                table: "KayitliSepetUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_KayitliSepetUrunleri_VaryasyonId",
                table: "KayitliSepetUrunleri",
                column: "VaryasyonId");

            migrationBuilder.CreateIndex(
                name: "IX_KuponKategoriler_KategoriId",
                table: "KuponKategoriler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_KuponKategoriler_KuponId",
                table: "KuponKategoriler",
                column: "KuponId");

            migrationBuilder.CreateIndex(
                name: "IX_KuponKullanimlari_KullaniciId",
                table: "KuponKullanimlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KuponKullanimlari_KuponId",
                table: "KuponKullanimlari",
                column: "KuponId");

            migrationBuilder.CreateIndex(
                name: "IX_KuponKullanimlari_SiparisId",
                table: "KuponKullanimlari",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_Kuponlar_Kod",
                table: "Kuponlar",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markalar_Slug",
                table: "Markalar",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunKampanyalari_KampanyaId",
                table: "UrunKampanyalari",
                column: "KampanyaId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunKampanyalari_UrunId_KampanyaId",
                table: "UrunKampanyalari",
                columns: new[] { "UrunId", "KampanyaId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Markalar_MarkaId",
                table: "Urunler",
                column: "MarkaId",
                principalTable: "Markalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Markalar_MarkaId",
                table: "Urunler");

            migrationBuilder.DropTable(
                name: "KargoUcretleri");

            migrationBuilder.DropTable(
                name: "KayitliSepetUrunleri");

            migrationBuilder.DropTable(
                name: "KuponKategoriler");

            migrationBuilder.DropTable(
                name: "KuponKullanimlari");

            migrationBuilder.DropTable(
                name: "Markalar");

            migrationBuilder.DropTable(
                name: "UrunKampanyalari");

            migrationBuilder.DropTable(
                name: "KargoFirmalari");

            migrationBuilder.DropTable(
                name: "KayitliSepetler");

            migrationBuilder.DropTable(
                name: "Kuponlar");

            migrationBuilder.DropTable(
                name: "Kampanyalar");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_MarkaId",
                table: "Urunler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_Slug",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Sira",
                table: "UrunVaryasyonlari");

            migrationBuilder.DropColumn(
                name: "CanonicalUrl",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "FavoriSayisi",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "GoruntulenmeSayisi",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "MarkaAdi",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "MarkaId",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "PopuleriteSkor",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "SatinAlmaSayisi",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Urunler");

            migrationBuilder.AddColumn<string>(
                name: "Marka",
                table: "Urunler",
                type: "TEXT",
                nullable: true);
        }
    }
}
