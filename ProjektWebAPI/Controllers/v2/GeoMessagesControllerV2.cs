using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektWebAPI.Data;
using ProjektWebAPI.Models;
using ProjektWebAPI.Models.v2;
using ProjektWebAPI.Models.V2;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjektWebAPI.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    
    public class GeoMessagesControllerV2 : ControllerBase
    {
        private readonly GeoMessageDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public GeoMessagesControllerV2(GeoMessageDbContext context,
            SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
           _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: api/GeoMessages
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLon">
        /// <para>Minimum Longitude för geomessage</para>
        /// </param>
        /// <param name="maxLon">
        /// <para>Max Longitude för geomessage</para>
        /// </param>
        /// <param name="minLat">
        /// <para>Minimum Latitude för geomessage</para>
        /// </param>
        /// <param name="maxLat">
        /// <para>Max Latitude för geomessage</para>
        /// </param>
        /// <returns>Returnerar geomessages som JSON objekt/></returns>

        [HttpGet]
        [SwaggerOperation(
            Summary ="Hämta alla geomessages",
            Description ="Hämtar ut alla geomessages beroende på inskrivet latitude och longitude. Om något inte är ifyllt hämtas alla messages ut.")]
        [SwaggerResponse(200, Description = "Visar Geomessages")]
        public async Task<ActionResult<IEnumerable<GeoMessageV2DTO>>> GetGeoMessages(
            double? minLon, double? maxLon, double? minLat, double? maxLat)
        {
            if(minLat == null || minLon == null || maxLat == null || maxLon == null)
            {

                var v1 = await _context.GeoMessages.ToListAsync();
                var v2 = await _context.GeoMessagesV2.ToListAsync();

                var list = formatV1(v1).Concat(formatV2(v2));
                return Ok(list);
            }
            else
            {
                var v1 = await _context.GeoMessages.Where(x => x.Latitude >= minLat && x.Latitude <= maxLat && x.Longitude >= minLon && x.Longitude <= maxLon).ToListAsync();
                var v2 = await _context.GeoMessagesV2.Where(x => x.Latitude >= minLat && x.Latitude <= maxLat && x.Longitude >= minLon && x.Longitude <= maxLon).ToListAsync();

                var list = formatV1(v1).Concat(formatV2(v2));
                return Ok(list);
            }
        }
        private IEnumerable<GeoMessageV2DTO> formatV1(IEnumerable<GeoMessage> list)
        {
            foreach (var message in list)
            {
                var messageDTO = new GeoMessageV2DTO
                {
                   
                    Message = new Message { Title = "test"},
                    Longitude = message.Longitude,
                    Latitude = message.Latitude
                };
                yield return messageDTO;
            }
        }
        private IEnumerable<GeoMessageV2DTO> formatV2(IEnumerable<GeoMessageV2> list)
        {
            foreach (var message in list)
            {
                var messageDTO = new GeoMessageV2DTO
                {
                    Message = new Message { Title = message.Title, Body = message.Body, Author = message.Author },
                    Longitude = message.Longitude,
                    Latitude = message.Latitude
                };
                yield return messageDTO;
            }
        }

        // GET: api/GeoMessages/5
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary ="Visar ett Geomessage",
            Description ="Hämtar ett Geomessage i v2 på id")]
        [SwaggerResponse(200, Description ="Ett Geomessage")]
        public async Task<ActionResult<GeoMessageV2DTO>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessagesV2.FindAsync(id);

            var messageDTO = new GeoMessageV2DTO
            {
                Message = new Message
                {
                    Title = geoMessage.Title,
                    Body = geoMessage.Body,
                    Author = geoMessage.Author
                },
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude
            };

            if (geoMessage == null)
            {
                return NotFound();
            }

            return messageDTO;
        }

        

        // POST: api/GeoMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Skapar ett Geomessage",
            Description = "Skapar ett Geomessage som sparas i både v1 & v2")]
        [SwaggerResponse(201, Description ="Nytt Geomessage har skapats")]
        public async Task<ActionResult<GeoMessageV2DTO>> PostGeoMessage(V2InputDTO geoMessage)
        {
            
            var user = await _context.User.FindAsync(_userManager.GetUserId(User));
            var msg = new GeoMessageV2()
            {
                Title = geoMessage.Title,
                Body = geoMessage.Body,
                Author = $"{user.FirstName} {user.LastName}",
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude
            };
            await _context.AddAsync(msg);
            await _context.SaveChangesAsync();
            var messageDTO = new GeoMessageV2DTO
            {
                Message = new Message
                {
                    Title = msg.Title,
                    Body = msg.Body,
                    Author = msg.Author
                },
                Longitude = msg.Longitude,
                Latitude = msg.Latitude
            };


            return CreatedAtAction("GetGeoMessage", new { id = msg.Id }, messageDTO);

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
