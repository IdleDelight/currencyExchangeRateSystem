using ExchangeRateSharedLib;
using Moq.Protected;
using Moq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace UnitTest_ExchangeRateSharedLib
{
    namespace UnitTest_ExchangeRateSharedLib
    {
        public class Tests
        {
            private HttpClient mockHttpClient; private string mockApiKey = "testApiKey";
            private string mockBaseUrl = "http://testurl.com";
            private string mockBaseCurrency = "EUR";
            private string[] mockValidSymbols = new string[] { "USD", "EUR" };

            [SetUp]
            public void Setup()
            {
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"success\":true, \"rates\": {\"USD\": 1.2, \"EUR\": 1}}")
                    });
                mockHttpClient = new HttpClient(mockMessageHandler.Object);
            }

            [Test]
            public void IsValidCurrency_ValidSymbol_ReturnsTrue()
            {
                var service = new CurrencyService(mockHttpClient, mockApiKey, mockBaseUrl, mockBaseCurrency, mockValidSymbols);
                var result = service.IsValidCurrency("USD");
                Assert.IsTrue(result);
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

                var service = new CurrencyService(mockHttpClient, mockApiKey, mockBaseUrl, mockBaseCurrency, mockValidSymbols);

                var result = service.FetchExchangeRates("2023-08-12").Result;

                Assert.IsNotNull(result);
                Assert.IsTrue(result.ContainsKey("USD"));
            }

            [Test]
            public async Task FetchExchangeRates_ApiReturnsError_ThrowsException()
            {
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpClient = new HttpClient(mockMessageHandler.Object);
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"success\":false}")
                    });

                var service = new CurrencyService(mockHttpClient, mockApiKey, mockBaseUrl, mockBaseCurrency, mockValidSymbols);

                Assert.ThrowsAsync<Exception>(async () => await service.FetchExchangeRates("2023-08-12"));
            }

            [Test]
            public void ConvertCurrency_ValidConversion_ReturnsConvertedAmount()
            {
                var rates = JObject.Parse("{\"USD\": 1.2, \"EUR\": 1}");

                var service = new CurrencyService(mockHttpClient, mockApiKey, mockBaseUrl, mockBaseCurrency, mockValidSymbols);
                var result = service.ConvertCurrency("USD", "EUR", 12m, rates);

                Assert.That(result, Is.EqualTo(10m)); // 12 USD is 10 EUR at set rates.
            }

            [Test]
            public void ConvertCurrency_InvalidFromCurrency_ThrowsException()
            {
                var rates = JObject.Parse("{\"USD\": 1.2, \"EUR\": 1}");

                var service = new CurrencyService(mockHttpClient, mockApiKey, mockBaseUrl, mockBaseCurrency, mockValidSymbols);

                Assert.Throws<Exception>(() => service.ConvertCurrency("INVALID", "EUR", 12m, rates));
            }
        }
    }
}