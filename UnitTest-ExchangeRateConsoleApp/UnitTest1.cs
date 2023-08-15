using ExchangeRateConsoleApp;
using Moq.Protected;
using Moq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace UnitTest_ExchangeRateSharedLib
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsValidCurrency_ValidSymbol_ReturnsTrue()
        {
            var service = new ExchangeRateSharedLib.CurrencyService();
            var result = service.IsValidCurrency("USD");
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidCurrency_InvalidSymbol_ReturnsFalse()
        {
            var service = new ExchangeRateSharedLib.CurrencyService();
            var result = service.IsValidCurrency("INVALID");
            Assert.IsFalse(result);
        }

        [Test]
        public void FetchExchangeRates_ValidDate_ReturnsRates()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"success\":true, \"rates\": {\"USD\": 1.2, \"EUR\": 1}}")
                });

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);

            var service = new ExchangeRateSharedLib.CurrencyService();
            var result = service.FetchExchangeRates("2023-08-12", mockHttpClient);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("USD"));
        }

        [Test]
        public void FetchExchangeRates_ApiReturnsError_ThrowsException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"success\":false}")
                });

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);

            var service = new ExchangeRateSharedLib.CurrencyService();

            Assert.Throws<Exception>(() => service.FetchExchangeRates("2023-08-12", mockHttpClient));
        }

        [Test]
        public void ConvertCurrency_ValidConversion_ReturnsConvertedAmount()
        {
            var rates = JObject.Parse("{\"USD\": 1.2, \"EUR\": 1}");

            var service = new ExchangeRateSharedLib.CurrencyService();
            var result = service.ConvertCurrency("USD", "EUR", 12m, rates);

            Assert.That(result, Is.EqualTo(10m)); // As 12 USD is 10 EUR based on provided rates.
        }

        [Test]
        public void ConvertCurrency_InvalidFromCurrency_ThrowsException()
        {
            var rates = JObject.Parse("{\"USD\": 1.2, \"EUR\": 1}");

            var service = new ExchangeRateSharedLib.CurrencyService();

            Assert.Throws<Exception>(() => service.ConvertCurrency("INVALID", "EUR", 12m, rates));
        }
    }
}