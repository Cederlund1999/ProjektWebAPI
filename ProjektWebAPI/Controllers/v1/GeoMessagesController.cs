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
using ProjektWebAPI.Models.v1;
using ProjektWebAPI.Models.V2;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet]
        [SwaggerOperation(
            Summary ="Hämta Geomessages",
            Description = "Hämtar alla Geomessages i v1")]
        [SwaggerResponse(200, Description = "Alla Geomessages")]
        public async Task<ActionResult<IEnumerable<GeoMessageV1DTO>>> GetGeoMessages()
        {
            var v1 = await _context.GeoMessages.ToListAsync();
            var v2 = await _context.GeoMessagesV2.ToListAsync();

            var list = formatV1(v1).Concat(formatV2(v2));
            return Ok(list);
        }

        private IEnumerable<GeoMessageV1DTO> formatV1(IEnumerable<GeoMessage> list)
        {
            foreach(var message in list)
            {
                var messageDTO = new GeoMessageV1DTO
                {
                    Message = message.Message,
                    Longitude = message.Longitude,
                    Latitude = message.Latitude
                };
                yield return messageDTO;
            }
        }
        private IEnumerable<GeoMessageV1DTO> formatV2(IEnumerable<GeoMessageV2> list)
        {
            foreach (var message in list)
            {
                var messageDTO = new GeoMessageV1DTO
                {
                    Message = message.Body,
                    Longitude = message.Longitude,
                    Latitude = message.Latitude
                };
                yield return messageDTO;
            }
        }

        // GET: api/GeoMessages/5
        [HttpGet("{Id}")]
        [SwaggerOperation(
            Summary = "Hämta Geomessage på Id",
            Description = "Hämtar ett Geomessage på Id")]
            [SwaggerResponse(200, Description = "Hämtar Geomessage")]
        public async Task<ActionResult<GeoMessageV1DTO>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.FindAsync(id);

            if (geoMessage == null)
            {
                return NotFound();
            }

            var messageDTO = new GeoMessageV1DTO
            {
                Message = geoMessage.Message,
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude
            };
            return Ok(messageDTO);
                
        }

       

        // POST: api/GeoMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary ="Skapa GeoMessage",
            Description = "Skapar ett Geomessage")]
        [SwaggerResponse(201, Description = "Ett nytt Geomessage har skapats")]
        public async Task<ActionResult<GeoMessageV1DTO>> PostGeoMessage(GeoMessageV1DTO geoMessage)
        {       
            var newMessage = new GeoMessage
            {
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude,
                Message = geoMessage.Message
            };
            await _context.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoMessage", new { id = newMessage.Id }, geoMessage);

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
