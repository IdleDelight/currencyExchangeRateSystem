﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeRateDB.Data;
using ExchangeRateDB.Model;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController : ControllerBase
{
    private readonly ExchangeRateDbContext _context;

    public CurrenciesController( ExchangeRateDbContext context )
    {
        _context = context;
    }

    // GET: api/Currencies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
    {
        return await _context.Currencies.ToListAsync();
    }

    // GET: api/Currencies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Currency>> GetCurrency( int id )
    {
        var currency = await _context.Currencies.FindAsync(id);

        if (currency == null) {
            return NotFound();
        }

        return Ok(currency);
    }

    // POST: api/Currencies
    [HttpPost]
    public async Task<ActionResult<Currency>> PostCurrency( Currency currency )
    {
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
    }

    // PUT: api/Currencies/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCurrency( int id, Currency currency )
    {
        if (id != currency.Id) {
            return BadRequest();
        }

        _context.Entry(currency).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!CurrencyExists(id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Currencies/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCurrency( int id )
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null) {
            return NotFound();
        }

        _context.Currencies.Remove(currency);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CurrencyExists( int id )
    {
        return _context.Currencies.Any(e => e.Id == id);
    }
}