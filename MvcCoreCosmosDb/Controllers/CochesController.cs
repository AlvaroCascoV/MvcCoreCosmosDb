using Microsoft.AspNetCore.Mvc;
using MvcCoreCosmosDb.Models;
using MvcCoreCosmosDb.Services;
using System.Numerics;

namespace MvcCoreCosmosDb.Controllers
{
    public class CochesController : Controller
    {
        private ServiceCosmosDb service;

        public CochesController(ServiceCosmosDb service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            await this.service.CreateDatabaseAsync();
            ViewData["MENSAJE"] = "Base de datos creada";
            return View();
        }

        public async Task<IActionResult> MisCoches()
        {
            List<Coche> coches = await this.service.GetCochesAsync();
            return View(coches);
        }
        [HttpPost]
        public async Task<IActionResult> MisCoches(string marca)
        {
            List<Coche> coches = await this.service.GetCochesMarcaAsync(marca);
            return View(coches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coche car, string existemotor)
        {
            if (existemotor == null)
            {
                car.Motor = null;
            }
            await this.service.CreateCocheAsync(car);
            return RedirectToAction("MisCoches");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteCocheAsync(id);
            return RedirectToAction("MisCoches");
        }

        public async Task<IActionResult> Details(string id)
        {
            Coche car = await this.service.FindCocheAsync(id);
            return View(car);
        }

        public async Task<IActionResult> Edit(string id)
        {
            Coche car = await this.service.FindCocheAsync(id);
            return View(car);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Coche car)
        {
            await this.service.UpdateCocheAsync(car);
            return RedirectToAction("MisCoches");
        }
    }
}
