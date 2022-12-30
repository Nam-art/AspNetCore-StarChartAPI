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
        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            var routeValue = new { id = celestialObject.Id };
            return CreatedAtRoute("GetById",routeValue,celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var selectedObject = _context.CelestialObjects.SingleOrDefault(x => x.Id == id);
            if (selectedObject == null)
            {
                return NotFound();
            }
            else
            {
                selectedObject.Name = celestialObject.Name;
                selectedObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                selectedObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
                _context.CelestialObjects.Update(selectedObject);
                _context.SaveChanges();
                return NoContent();
            }
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var selectedObject = _context.CelestialObjects.SingleOrDefault(x => x.Id == id);
            if (selectedObject == null)
            {
                return NotFound();
            }
            selectedObject.Name = name;
            _context.Update(selectedObject);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var selectedObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (!selectedObjects.Any()) { return NotFound(); }
            _context.CelestialObjects.RemoveRange(selectedObjects);
            _context.SaveChanges();
            return NoContent();
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
