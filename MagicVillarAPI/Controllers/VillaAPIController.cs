using MagicVillarAPI.Data;
using MagicVillarAPI.Models;
using MagicVillarAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MagicVillarAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(_db.Villas);
        }
        //[HttpGet("id")]
        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villaalready exist.");
                return BadRequest(ModelState);
            }
            if(villaDto == null)
            {
                return BadRequest();
            }
            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla",new {id = villaDto.Id}, villaDto);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        [HttpDelete("{id:int}", Name ="DeleteVilla")]
        public  IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            return NoContent(); 

        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
            };
            _db.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("id:int",Name = "UpdatePartailVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartailVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault( u => u.Id==id);
            VillaDto villaDto = new() 
            { 
                Amenity = villa.Amenity,
                Details = villa.Details,
                Name = villa.Name,
                ImageUrl= villa.ImageUrl,
                Id = villa.Id,
                Occupancy= villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,

            };

            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDto,ModelState);
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Name = villaDto.Name,
                ImageUrl= villaDto.ImageUrl,
                Id = villaDto.Id,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,

            };
            _db.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
