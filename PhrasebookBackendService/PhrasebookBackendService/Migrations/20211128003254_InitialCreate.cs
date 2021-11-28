using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhrasebookBackendService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdentityProvider = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignedUpOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phrasebooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstLanguageId = table.Column<string>(type: "nvarchar(2)", nullable: true),
                    ForeignLanguageId = table.Column<string>(type: "nvarchar(2)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrasebooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Languages_FirstLanguageId",
                        column: x => x.FirstLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Languages_ForeignLanguageId",
                        column: x => x.ForeignLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhrasebookId = table.Column<int>(type: "int", nullable: false),
                    FirstLanguagePhrase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ForeignLanguagePhrase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LexicalItemType = table.Column<int>(type: "int", nullable: false),
                    ForeignLanguageSynonyms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectnessCount = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phrases_Phrasebooks_PhrasebookId",
                        column: x => x.PhrasebookId,
                        principalTable: "Phrasebooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { "ab", "Abkhaz" },
                    { "nr", "South Ndebele" },
                    { "oc", "Occitan" },
                    { "oj", "Ojibwe" },
                    { "cu", "Old Church" },
                    { "om", "Oromo" },
                    { "or", "Oriya" },
                    { "os", "Ossetian" },
                    { "pa", "Panjabi" },
                    { "ii", "Nuosu" },
                    { "pi", "Pāli" },
                    { "pl", "Polish" },
                    { "ps", "Pashto" },
                    { "pt", "Portuguese" },
                    { "qu", "Quechua" },
                    { "rm", "Romansh" },
                    { "rn", "Kirundi" },
                    { "ro", "Romanian" },
                    { "ru", "Russian" },
                    { "fa", "Persian" },
                    { "no", "Norwegian" },
                    { "nn", "Norwegian Nynorsk" },
                    { "ng", "Ndonga" },
                    { "lo", "Lao" },
                    { "lt", "Lithuanian" },
                    { "lu", "Luba-Katanga" },
                    { "lv", "Latvian" },
                    { "gv", "Manx" },
                    { "mk", "Macedonian" },
                    { "mg", "Malagasy" },
                    { "ms", "Malay" },
                    { "ml", "Malayalam" },
                    { "mt", "Maltese" },
                    { "mi", "Māori" },
                    { "mr", "Marathi (Marāṭhī)" },
                    { "mh", "Marshallese" },
                    { "mn", "Mongolian" },
                    { "na", "Nauru" },
                    { "nv", "Navajo" },
                    { "nb", "Norwegian Bokmål" },
                    { "nd", "North Ndebele" },
                    { "ne", "Nepali" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { "sa", "Sanskrit (Saṁskṛta)" },
                    { "sd", "Sindhi" },
                    { "se", "Northern Sami" },
                    { "sm", "Samoan" },
                    { "to", "Tonga (Tonga)" },
                    { "tr", "Turkish" },
                    { "ts", "Tsonga" },
                    { "tt", "Tatar" },
                    { "tw", "Twi" },
                    { "ty", "Tahitian" },
                    { "ug", "Uighur" },
                    { "uk", "Ukrainian" },
                    { "ur", "Urdu" },
                    { "uz", "Uzbek" },
                    { "ve", "Venda" },
                    { "vi", "Vietnamese" },
                    { "vo", "Volapük" },
                    { "wa", "Walloon" },
                    { "cy", "Welsh" },
                    { "wo", "Wolof" },
                    { "fy", "Western Frisian" },
                    { "xh", "Xhosa" },
                    { "yi", "Yiddish" },
                    { "tn", "Tswana" },
                    { "ln", "Lingala" },
                    { "tl", "Tagalog" },
                    { "bo", "Tibetan Standard" },
                    { "sg", "Sango" },
                    { "sr", "Serbian" },
                    { "gd", "Scottish Gaelic" },
                    { "sn", "Shona" },
                    { "si", "Sinhala" },
                    { "sk", "Slovak" },
                    { "sl", "Slovene" },
                    { "so", "Somali" },
                    { "st", "Southern Sotho" },
                    { "es", "Spanish" },
                    { "su", "Sundanese" },
                    { "sw", "Swahili" },
                    { "ss", "Swati" },
                    { "sv", "Swedish" },
                    { "ta", "Tamil" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { "te", "Telugu" },
                    { "tg", "Tajik" },
                    { "th", "Thai" },
                    { "ti", "Tigrinya" },
                    { "tk", "Turkmen" },
                    { "yo", "Yoruba" },
                    { "li", "Limburgish" },
                    { "lb", "Luxembourgish" },
                    { "my", "Burmese" },
                    { "ca", "Catalan" },
                    { "ch", "Chamorro" },
                    { "ce", "Chechen" },
                    { "ny", "Chichewa" },
                    { "zh", "Chinese" },
                    { "cv", "Chuvash" },
                    { "kw", "Cornish" },
                    { "bg", "Bulgarian" },
                    { "co", "Corsican" },
                    { "hr", "Croatian" },
                    { "cs", "Czech" },
                    { "da", "Danish" },
                    { "dv", "Divehi" },
                    { "nl", "Dutch" },
                    { "en", "English" },
                    { "eo", "Esperanto" },
                    { "et", "Estonian" },
                    { "cr", "Cree" },
                    { "br", "Breton" },
                    { "bs", "Bosnian" },
                    { "bi", "Bislama" },
                    { "aa", "Afar" },
                    { "af", "Afrikaans" },
                    { "ak", "Akan" },
                    { "sq", "Albanian" },
                    { "am", "Amharic" },
                    { "ar", "Arabic" },
                    { "an", "Aragonese" },
                    { "hy", "Armenian" },
                    { "as", "Assamese" },
                    { "av", "Avaric" },
                    { "ae", "Avestan" },
                    { "ay", "Aymara" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { "az", "Azerbaijani" },
                    { "bm", "Bambara" },
                    { "ba", "Bashkir" },
                    { "eu", "Basque" },
                    { "be", "Belarusian" },
                    { "bn", "Bengali" },
                    { "bh", "Bihari" },
                    { "ee", "Ewe" },
                    { "fo", "Faroese" },
                    { "fj", "Fijian" },
                    { "fi", "Finnish" },
                    { "it", "Italian" },
                    { "iu", "Inuktitut" },
                    { "ja", "Japanese" },
                    { "jv", "Javanese" },
                    { "kl", "Kalaallisut" },
                    { "kn", "Kannada" },
                    { "kr", "Kanuri" },
                    { "ks", "Kashmiri" },
                    { "kk", "Kazakh" },
                    { "km", "Khmer" },
                    { "ki", "Kikuyu" },
                    { "rw", "Kinyarwanda" },
                    { "ky", "Kirghiz" },
                    { "kv", "Komi" },
                    { "kg", "Kongo" },
                    { "ko", "Korean" },
                    { "ku", "Kurdish" },
                    { "kj", "Kwanyama" },
                    { "la", "Latin" },
                    { "is", "Icelandic" },
                    { "lg", "Luganda" },
                    { "io", "Ido" },
                    { "ig", "Igbo" },
                    { "fr", "French" },
                    { "ff", "Fula" },
                    { "gl", "Galician" },
                    { "ka", "Georgian" },
                    { "de", "German" },
                    { "el", "Greek" },
                    { "gn", "Guaraní" },
                    { "gu", "Gujarati" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { "ht", "Haitian" },
                    { "ha", "Hausa" },
                    { "he", "Hebrew (modern)" },
                    { "hz", "Herero" },
                    { "hi", "Hindi" },
                    { "ho", "Hiri Motu" },
                    { "hu", "Hungarian" },
                    { "ia", "Interlingua" },
                    { "id", "Indonesian" },
                    { "ie", "Interlingue" },
                    { "ga", "Irish" },
                    { "ik", "Inupiaq" },
                    { "za", "Zhuang" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_FirstLanguageId",
                table: "Phrasebooks",
                column: "FirstLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_ForeignLanguageId",
                table: "Phrasebooks",
                column: "ForeignLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_UserId",
                table: "Phrasebooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_PhrasebookId",
                table: "Phrases",
                column: "PhrasebookId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrincipalId",
                table: "Users",
                column: "PrincipalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phrases");

            migrationBuilder.DropTable(
                name: "Phrasebooks");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
