using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}",Name ="GetById")]
        public IActionResult GetById(int id)
        {
            if (!_context.CelestialObjects.Where(x => x.Id == id).Any())
            {
                return NotFound();
            }
            else
            {
                var singleObject = _context.CelestialObjects.SingleOrDefault(x => x.Id == id);
                var celestialObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == singleObject.Id).ToList();
                singleObject.Satellites = celestialObjects;
                return Ok(singleObject);
            }
            //return Ok(CelestialObject celestialobject);

        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            if (!_context.CelestialObjects.Where(x => x.Name == name).Any())
            {
                return NotFound();
            }
            else
            {
                var nameMatch = _context.CelestialObjects.Where(x => x.Name == name).ToList();
               foreach (var item in nameMatch)
                {
                    //go through list of all matched names and if their 
                    item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id)?.ToList();
                }
                return Ok(nameMatch);
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var allCelestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestial in allCelestialObjects)
            {
                celestial.Satellites = allCelestialObjects.Where(x => x.Id == celestial.Id).ToList();
            }
            return Ok(allCelestialObjects);
        }
    }
}
