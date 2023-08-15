using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExchangeRateAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpdateLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DbUpdateDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiCallSuccess = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(20,10)", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, "Euro", "EUR" },
                    { 2, "United Arab Emirates Dirham", "AED" },
                    { 3, "Afghan Afghani", "AFN" },
                    { 4, "Albanian Lek", "ALL" },
                    { 5, "Armenian Dram", "AMD" },
                    { 6, "Netherlands Antillean Guilder", "ANG" },
                    { 7, "Angolan Kwanza", "AOA" },
                    { 8, "Argentine Peso", "ARS" },
                    { 9, "Australian Dollar", "AUD" },
                    { 10, "Aruban Florin", "AWG" },
                    { 11, "Azerbaijani Manat", "AZN" },
                    { 12, "Bosnia-Herzegovina Convertible Mark", "BAM" },
                    { 13, "Barbadian Dollar", "BBD" },
                    { 14, "Bangladeshi Taka", "BDT" },
                    { 15, "Bulgarian Lev", "BGN" },
                    { 16, "Bahraini Dinar", "BHD" },
                    { 17, "Burundian Franc", "BIF" },
                    { 18, "Bermudian Dollar", "BMD" },
                    { 19, "Brunei Dollar", "BND" },
                    { 20, "Bolivian Bolíviano", "BOB" },
                    { 21, "Brazilian Real", "BRL" },
                    { 22, "Bahamian Dollar", "BSD" },
                    { 23, "Bitcoin", "BTC" },
                    { 24, "Bhutanese Ngultrum", "BTN" },
                    { 25, "Botswana Pula", "BWP" },
                    { 26, "Belarusian Ruble", "BYN" },
                    { 27, "Belarusian Ruble (till 2016)", "BYR" },
                    { 28, "Belize Dollar", "BZD" },
                    { 29, "Canadian Dollar", "CAD" },
                    { 30, "Congolese Franc", "CDF" },
                    { 31, "Swiss Franc", "CHF" },
                    { 32, "Chilean Unit of Account (UF)", "CLF" },
                    { 33, "Chilean Peso", "CLP" },
                    { 34, "Chinese Yuan", "CNY" },
                    { 35, "Colombian Peso", "COP" },
                    { 36, "Costa Rican Colón", "CRC" },
                    { 37, "Cuban Convertible Peso", "CUC" },
                    { 38, "Cuban Peso", "CUP" },
                    { 39, "Cape Verdean Escudo", "CVE" },
                    { 40, "Czech Republic Koruna", "CZK" },
                    { 41, "Djiboutian Franc", "DJF" },
                    { 42, "Danish Krone", "DKK" },
                    { 43, "Dominican Peso", "DOP" },
                    { 44, "Algerian Dinar", "DZD" },
                    { 45, "Egyptian Pound", "EGP" },
                    { 46, "Eritrean Nakfa", "ERN" },
                    { 47, "Ethiopian Birr", "ETB" },
                    { 48, "Fijian Dollar", "FJD" },
                    { 49, "Falkland Islands Pound", "FKP" },
                    { 50, "British Pound Sterling", "GBP" },
                    { 51, "Georgian Lari", "GEL" },
                    { 52, "Guernsey Pound", "GGP" },
                    { 53, "Ghanaian Cedi", "GHS" },
                    { 54, "Gibraltar Pound", "GIP" },
                    { 55, "Gambian Dalasi", "GMD" },
                    { 56, "Guinean Franc", "GNF" },
                    { 57, "Guatemalan Quetzal", "GTQ" },
                    { 58, "Guyanese Dollar", "GYD" },
                    { 59, "Hong Kong Dollar", "HKD" },
                    { 60, "Honduran Lempira", "HNL" },
                    { 61, "Croatian Kuna", "HRK" },
                    { 62, "Haitian Gourde", "HTG" },
                    { 63, "Hungarian Forint", "HUF" },
                    { 64, "Indonesian Rupiah", "IDR" },
                    { 65, "Israeli New Sheqel", "ILS" },
                    { 66, "Manx pound", "IMP" },
                    { 67, "Indian Rupee", "INR" },
                    { 68, "Iraqi Dinar", "IQD" },
                    { 69, "Iranian Rial", "IRR" },
                    { 70, "Icelandic Króna", "ISK" },
                    { 71, "Jersey Pound", "JEP" },
                    { 72, "Jamaican Dollar", "JMD" },
                    { 73, "Jordanian Dinar", "JOD" },
                    { 74, "Japanese Yen", "JPY" },
                    { 75, "Kenyan Shilling", "KES" },
                    { 76, "Kyrgyzstani Som", "KGS" },
                    { 77, "Cambodian Riel", "KHR" },
                    { 78, "Comorian Franc", "KMF" },
                    { 79, "North Korean Won", "KPW" },
                    { 80, "South Korean Won", "KRW" },
                    { 81, "Kuwaiti Dinar", "KWD" },
                    { 82, "Cayman Islands Dollar", "KYD" },
                    { 83, "Kazakhstani Tenge", "KZT" },
                    { 84, "Laotian Kip", "LAK" },
                    { 85, "Lebanese Pound", "LBP" },
                    { 86, "Sri Lankan Rupee", "LKR" },
                    { 87, "Liberian Dollar", "LRD" },
                    { 88, "Lesotho Loti", "LSL" },
                    { 89, "Lithuanian Litas", "LTL" },
                    { 90, "Latvian Lats", "LVL" },
                    { 91, "Libyan Dinar", "LYD" },
                    { 92, "Moroccan Dirham", "MAD" },
                    { 93, "Moldovan Leu", "MDL" },
                    { 94, "Malagasy Ariary", "MGA" },
                    { 95, "Macedonian Denar", "MKD" },
                    { 96, "Myanma Kyat", "MMK" },
                    { 97, "Mongolian Tugrik", "MNT" },
                    { 98, "Macanese Pataca", "MOP" },
                    { 99, "Mauritanian Ouguiya (till 2017)", "MRO" },
                    { 100, "Mauritian Rupee", "MUR" },
                    { 101, "Maldivian Rufiyaa", "MVR" },
                    { 102, "Malawian Kwacha", "MWK" },
                    { 103, "Mexican Peso", "MXN" },
                    { 104, "Malaysian Ringgit", "MYR" },
                    { 105, "Mozambican Metical", "MZN" },
                    { 106, "Namibian Dollar", "NAD" },
                    { 107, "Nigerian Naira", "NGN" },
                    { 108, "Nicaraguan Córdoba", "NIO" },
                    { 109, "Norwegian Krone", "NOK" },
                    { 110, "Nepalese Rupee", "NPR" },
                    { 111, "New Zealand Dollar", "NZD" },
                    { 112, "Omani Rial", "OMR" },
                    { 113, "Panamanian Balboa", "PAB" },
                    { 114, "Peruvian Nuevo Sol", "PEN" },
                    { 115, "Papua New Guinean Kina", "PGK" },
                    { 116, "Philippine Peso", "PHP" },
                    { 117, "Pakistani Rupee", "PKR" },
                    { 118, "Polish Zloty", "PLN" },
                    { 119, "Paraguayan Guarani", "PYG" },
                    { 120, "Qatari Rial", "QAR" },
                    { 121, "Romanian Leu", "RON" },
                    { 122, "Serbian Dinar", "RSD" },
                    { 123, "Russian Ruble", "RUB" },
                    { 124, "Rwandan Franc", "RWF" },
                    { 125, "Saudi Riyal", "SAR" },
                    { 126, "Solomon Islands Dollar", "SBD" },
                    { 127, "Seychellois Rupee", "SCR" },
                    { 128, "Sudanese Pound", "SDG" },
                    { 129, "Swedish Krona", "SEK" },
                    { 130, "Singapore Dollar", "SGD" },
                    { 131, "Saint Helena Pound", "SHP" },
                    { 132, "Sierra Leonean Leone", "SLL" },
                    { 133, "Somali Shilling", "SOS" },
                    { 134, "Surinamese Dollar", "SRD" },
                    { 135, "South Sudanese Pound", "SSP" },
                    { 136, "São Tomé and Príncipe Dobra (pre-2018)", "STD" },
                    { 137, "Salvadoran Colón", "SVC" },
                    { 138, "Syrian Pound", "SYP" },
                    { 139, "Swazi Lilangeni", "SZL" },
                    { 140, "Thai Baht", "THB" },
                    { 141, "Tajikistani Somoni", "TJS" },
                    { 142, "Turkmenistani Manat", "TMT" },
                    { 143, "Tunisian Dinar", "TND" },
                    { 144, "Tongan Pa'anga", "TOP" },
                    { 145, "Turkish Lira", "TRY" },
                    { 146, "Trinidad and Tobago Dollar", "TTD" },
                    { 147, "New Taiwan Dollar", "TWD" },
                    { 148, "Tanzanian Shilling", "TZS" },
                    { 149, "Ukrainian Hryvnia", "UAH" },
                    { 150, "Ugandan Shilling", "UGX" },
                    { 151, "United States Dollar", "USD" },
                    { 152, "Uruguayan Peso", "UYU" },
                    { 153, "Uzbekistan Som", "UZS" },
                    { 154, "Venezuelan Bolívar Fuerte (Old)", "VEF" },
                    { 155, "Vietnamese Dong", "VND" },
                    { 156, "Vanuatu Vatu", "VUV" },
                    { 157, "Samoan Tala", "WST" }
                });

            migrationBuilder.InsertData(
                table: "UpdateLogs",
                columns: new[] { "Id", "ApiCallSuccess", "DbUpdateDate", "Message" },
                values: new object[] { 1, "NA", "14.08.2023 12:15:49", "DB Created" });

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "CurrencyId", "Date", "Value" },
                values: new object[,]
                {
                    { 1, 6, "1999-03-03", 1.946416m },
                    { 2, 9, "1999-03-03", 1.746727m },
                    { 3, 10, "1999-03-03", 1.946416m },
                    { 4, 13, "1999-03-03", 2.178089m },
                    { 5, 18, "1999-03-03", 1.089045m },
                    { 6, 21, "1999-03-03", 2.374996m },
                    { 7, 22, "1999-03-03", 2.131304m },
                    { 8, 24, "1999-03-03", 1.624130m },
                    { 9, 26, "1999-03-03", 1.179559m },
                    { 10, 29, "1999-03-03", 1.442242m },
                    { 11, 30, "1999-03-03", 2.283165m },
                    { 12, 31, "1999-03-03", 1.624130m },
                    { 13, 32, "1999-03-03", 2.131304m },
                    { 14, 33, "1999-03-03", 1.089045m },
                    { 15, 35, "1999-03-03", 1.089045m },
                    { 16, 38, "1999-03-03", 1.089045m },
                    { 17, 41, "1999-03-03", 1.740280m },
                    { 18, 42, "1999-03-03", 1.774681m },
                    { 19, 44, "1999-03-03", 2.283165m },
                    { 20, 46, "1999-03-03", 2.178089m },
                    { 21, 47, "1999-03-03", 1.774681m },
                    { 22, 48, "1999-03-03", 2.131304m },
                    { 23, 50, "1999-03-03", 2.178089m },
                    { 24, 51, "1999-03-03", 2.283165m },
                    { 25, 53, "1999-03-03", 1.774681m },
                    { 26, 54, "1999-03-03", 1.089045m },
                    { 27, 55, "1999-03-03", 2.374996m },
                    { 28, 56, "1999-03-03", 2.178089m },
                    { 29, 58, "1999-03-03", 1.946416m },
                    { 30, 59, "1999-03-03", 1.946416m },
                    { 31, 61, "1999-03-03", 2.178089m },
                    { 32, 63, "1999-03-03", 2.178089m },
                    { 33, 64, "1999-03-03", 2.131304m },
                    { 34, 65, "1999-03-03", 2.374996m },
                    { 35, 66, "1999-03-03", 1.946416m },
                    { 36, 68, "1999-03-03", 2.283165m },
                    { 37, 70, "1999-03-03", 1.624130m },
                    { 38, 71, "1999-03-03", 2.374996m },
                    { 39, 72, "1999-03-03", 2.178089m },
                    { 40, 73, "1999-03-03", 2.178089m },
                    { 41, 74, "1999-03-03", 1.624130m },
                    { 42, 75, "1999-03-03", 1.946416m },
                    { 43, 76, "1999-03-03", 2.374996m },
                    { 44, 78, "1999-03-03", 2.178089m },
                    { 45, 79, "1999-03-03", 2.178089m },
                    { 46, 80, "1999-03-03", 1.946416m },
                    { 47, 81, "1999-03-03", 2.178089m },
                    { 48, 82, "1999-03-03", 2.178089m },
                    { 49, 84, "1999-03-03", 2.283165m },
                    { 50, 86, "1999-03-03", 2.178089m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rates_CurrencyId",
                table: "Rates",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "UpdateLogs");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
