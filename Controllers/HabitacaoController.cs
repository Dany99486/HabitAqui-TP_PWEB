using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ficha1_P1_V1.Data;
using Ficha1_P1_V1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ficha1_P1_V1.Controllers
{
    public class HabitacaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Habitacao
        public async Task<IActionResult> Index()
        {
	        ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList().ToList(), "Id", "Nome");


			return View(await _context.Habitacao.ToListAsync());
        }

        // GET: Habitacao/Details/5
        //[Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao.Include("Categoria")
				.FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            return View(habitacao);
        }

        // GET: Habitacao/Create
        //[Authorize(Roles = "Funcionario,Gestor")]
        public IActionResult Create()
        {
	        ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");


			return View();
        }

        // POST: Habitacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Create([Bind("Id,Localizacao,Tipo,CategoriaId,Descricao")] Habitacao habitacao)
        {
	        ModelState.Remove(nameof(Habitacao.Categoria));

			if (ModelState.IsValid)
            {
                _context.Add(habitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			return View(habitacao);
        }

        // GET: Habitacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao.FindAsync(id);
            if (habitacao == null)
            {
                return NotFound();
            }
            ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			return View(habitacao);
        }

        // POST: Habitacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Localizacao,Tipo,CategoriaId,Descricao")] Habitacao habitacao)
        {
            if (id != habitacao.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(Habitacao.Categoria));

			if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacaoExists(habitacao.Id))
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
			ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			return View(habitacao);
        }

        // GET: Habitacao/Delete/5
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            return View(habitacao);
        }

        // POST: Habitacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Habitacao == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Habitacao'  is null.");
            }
            var habitacao = await _context.Habitacao.FindAsync(id);
            if (habitacao != null)
            {
                _context.Habitacao.Remove(habitacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacaoExists(int id)
        {
          return (_context.Habitacao?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
