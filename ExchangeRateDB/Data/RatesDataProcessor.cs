using ExchangeRateDB.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRateDB.Data
{
    public class RatesDataProcessor
    {
        private readonly ExchangeRateDbContext _context;

        public RatesDataProcessor( ExchangeRateDbContext context )
        {
            _context = context;
            Console.WriteLine("RatesDataProcessor initialized.");
        }

        public bool ProcessRatesData( JObject apiData, List<Currency> currenciesList )
        {
            var ratesJson = apiData["rates"] as JObject;
            if (ratesJson == null) {
                Console.WriteLine("Warning: Rates object not found or not in expected format.");
                return false;
            }
            Console.WriteLine($"Fetched Rates JSON: {ratesJson}");

            var rateDate = apiData["date"]?.Value<string>();
            if (rateDate == null) {
                Console.WriteLine("Warning: Date not found in the rates data.");
                return false;
            }
            if (!DateTime.TryParse(rateDate, out var parsedDate)) {
                Console.WriteLine("Warning: Unable to parse the date from the rates data.");
                return false;
            }

            // Check if rates for this date already exist
            if (_context.Rates.Any(r => r.Date == parsedDate)) {
                Console.WriteLine($"Rates for date {parsedDate} already exist. Updating rates.");

                var existingRatesForDate = _context.Rates.Where(r => r.Date == parsedDate).ToList();

                foreach (var rate in ratesJson) {
                    var currency = currenciesList.FirstOrDefault(c => c.Symbol == rate.Key);
                    if (currency != null) {
                        var existingRate = existingRatesForDate.FirstOrDefault(r => r.CurrencyId == currency.Id);
                        if (existingRate != null) {
                            // Update the rate value
                            existingRate.Value = rate.Value?.Type == JTokenType.Float ? rate.Value.Value<decimal>() : 0M;
                        }
                        else {
                            // This is for handling an unlikely scenario where a new rate for a currency appears for an old date.
                            _context.Rates.Add(new Rate
                            {
                                Value = rate.Value?.Type == JTokenType.Float ? rate.Value.Value<decimal>() : 0M,
                                Date = parsedDate.Date,
                                CurrencyId = currency.Id
                            });
                        }
                    }
                    else {
                        Console.WriteLine($"Warning: Currency symbol {rate.Key} not found in the provided currencies list.");
                    }
                }

                try {
                    _context.SaveChanges();
                    Console.WriteLine("Updated rates successfully committed to the database.");
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error while committing updates to the database: {ex.Message}");
                    Console.WriteLine($"Exception details: {ex.InnerException?.Message}");
                    return false;
                }
            }
            else {
                // Insert new rates
                var ratesList = new List<Rate>();

                foreach (var rate in ratesJson) {
                    var currency = currenciesList.FirstOrDefault(c => c.Symbol == rate.Key);
                    if (currency != null) {
                        ratesList.Add(new Rate
                        {
                            Value = rate.Value?.Type == JTokenType.Float ? rate.Value.Value<decimal>() : 0M,
                            Date = parsedDate.Date,
                            CurrencyId = currency.Id
                        });
                    }
                    else {
                        Console.WriteLine($"Warning: Currency symbol {rate.Key} not found in the provided currencies list.");
                    }
                }

                Console.WriteLine($"Processed Rates List: {JsonConvert.SerializeObject(ratesList)}");

                try {
                    _context.Rates.AddRange(ratesList);
                    _context.SaveChanges();
                    Console.WriteLine("Changes successfully committed to the database.");
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error while committing changes to the database: {ex.Message}");
                    Console.WriteLine($"Exception details: {ex.InnerException?.Message}");
                    return false;
                }
            }
        }
    }
}