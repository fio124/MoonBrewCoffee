using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;

namespace MoonBrewCoffee.Controllers
{
    public class CombosController : Controller
    {
        private readonly IComboRepository _comboRepository;

        public CombosController(IComboRepository comboRepository)
        {
            _comboRepository = comboRepository;
        }

        // GET: Combos
        public async Task<IActionResult> Index()
        {
            var combos = await _comboRepository.GetAllAsync();
            return View(combos);
        }

        // GET: Combos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _comboRepository.GetByIdAsync(id.Value);
            if (combo == null)
            {
                return NotFound();
            }

            return View(combo);
        }

        // GET: Combos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Combos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCombo,Nombre,Descripcion,PrecioCombo,ImagenURL,Activo")] Combo combo)
        {
            if (ModelState.IsValid)
            {
                await _comboRepository.AddAsync(combo);
                return RedirectToAction(nameof(Index));
            }
            return View(combo);
        }

        // GET: Combos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _comboRepository.GetByIdAsync(id.Value);
            if (combo == null)
            {
                return NotFound();
            }
            return View(combo);
        }

        // POST: Combos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCombo,Nombre,Descripcion,PrecioCombo,ImagenURL,Activo")] Combo combo)
        {
            if (id != combo.IdCombo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _comboRepository.UpdateAsync(combo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _comboRepository.ExistsAsync(combo.IdCombo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(combo);
        }

        // GET: Combos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _comboRepository.GetByIdAsync(id.Value);
            if (combo == null)
            {
                return NotFound();
            }

            return View(combo);
        }

        // POST: Combos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _comboRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ComboExists(int id)
        {
            return await _comboRepository.ExistsAsync(id);
        }
    }
}
