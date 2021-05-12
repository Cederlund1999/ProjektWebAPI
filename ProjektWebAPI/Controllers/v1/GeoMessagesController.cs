using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektWebAPI.Data;
using ProjektWebAPI.Models;

namespace ProjektWebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    
    public class GeoMessagesController : ControllerBase
    {
        private readonly GeoMessageDbContext _context;

        public GeoMessagesController(GeoMessageDbContext context)
        {
            _context = context;
        }

        // GET: api/GeoMessages

        [HttpGet("/v1/Geo-Messages")]
        public async Task<ActionResult<IEnumerable<GeoMessage>>> GetGeoMessages()
        {
            return await _context.GeoMessages.ToListAsync();
        }

        // GET: api/GeoMessages/5
        [HttpGet("/v1/Geo-Messages/{id}")]
        public async Task<ActionResult<GeoMessage>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.FindAsync(id);

            if (geoMessage == null)
            {
                return NotFound();
            }

            return Ok(geoMessage);
        }

       

        // POST: api/GeoMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("/v1/Geo-MessagesPost")]
        public async Task<ActionResult<GeoMessage>> PostGeoMessage(GeoMessage geoMessage)
        {       
            var newMessage = new GeoMessage
            {
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude,
                Message = geoMessage.Message
            };
            await _context.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoMessage", new { id = newMessage.Id }, newMessage);

            /*_context.GeoMessages.Add(geoMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoMessage", new { id = geoMessage.Id }, geoMessage);*/
        }

        

        private bool GeoMessageExists(int id)
        {
            return _context.GeoMessages.Any(e => e.Id == id);
        }
    }
}
