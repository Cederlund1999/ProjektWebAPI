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
using ProjektWebAPI.Models.V2;



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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoMessageV2>>> GetGeoMessages(
            double? minLon, double? maxLon, double? minLat, double? maxLat)
        {
            if(minLat == null || minLon == null || maxLat == null || maxLon == null)
            {
                
                var GeoMessageList2 = await _context.GeoMessagesV2.ToListAsync();

                if (GeoMessageList2 == null)
                    return NotFound();

                return Ok(GeoMessageList2);
            }
            else
            {
                var GeoMessageList2 = await _context.GeoMessagesV2.Where(
                    z => z.Latitude >= minLat && z.Latitude <= maxLat && z.Longitude >= minLon && z.Longitude <= maxLon).ToListAsync();
                return Ok(GeoMessageList2);
            }
        }

        // GET: api/GeoMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoMessageV2>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessagesV2.FindAsync(id);

            if (geoMessage == null)
            {
                return NotFound();
            }

            return Ok(geoMessage);
        }



        // POST: api/GeoMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GeoMessageV2>> PostGeoMessage(GeoMessageV2 geoMessage)
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


            return CreatedAtAction("GetGeoMessage", new { id = msg.Id }, msg);

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
