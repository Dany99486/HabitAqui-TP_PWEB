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
using Microsoft.AspNetCore.Identity;

namespace Ficha1_P1_V1.Controllers
{
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvaliacoesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

		// GET: Avaliacaos
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Index()
        {
            /*var applicationDbContext = _context.Avaliacao.Include(a => a.Arrendamento);
            return View(await applicationDbContext.ToListAsync());*/
            return _context.Avaliacao != null ?
             View(await _context.Avaliacao.ToListAsync()) :
             Problem("Entity set 'ApplicationDbContext.Habitacao' is null.");
        }

		// GET: Avaliacaos/Details/5
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Avaliacao == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Arrendamento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

		// GET: Avaliacaos/Create
		[Authorize(Roles = "Cliente")]
		public IActionResult? Create()
        {
            ViewData["ArrendamentoId"] = new SelectList(_context.Arrendamento.Where(c=>c.habitacao.ReservadoCliente.Id.Equals(_userManager.GetUserIdAsync)), "Id", "ArrendamentoId");
            return View();
        }

        // POST: Avaliacaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Cliente")]
		public async Task<IActionResult> Create([Bind("Id,Classificacao,Comentario,ArrendamentoId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                if (_context.Arrendamento.Find(avaliacao.ArrendamentoId) == null)
                {
                    return NotFound();
                }
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArrendamentoId"] = new SelectList(_context.Arrendamento, "Id", "Id", avaliacao.ArrendamentoId);
            return View(avaliacao);
        }

		// GET: Avaliacaos/Edit/5
		[Authorize(Roles = "Cliente")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Avaliacao == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            ViewData["ArrendamentoId"] = new SelectList(_context.Arrendamento, "Id", "Id", avaliacao.ArrendamentoId);
            return View(avaliacao);
        }

        // POST: Avaliacaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Cliente")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Classificacao,Comentario,ArrendamentoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoExists(avaliacao.Id))
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
            ViewData["ArrendamentoId"] = new SelectList(_context.Arrendamento, "Id", "Id", avaliacao.ArrendamentoId);
            return View(avaliacao);
        }

		// GET: Avaliacaos/Delete/5
		//[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Avaliacao == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Arrendamento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // POST: Avaliacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario,Cliente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Avaliacao == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Avaliacao'  is null.");
            }
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao != null)
            {
                _context.Avaliacao.Remove(avaliacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoExists(int id)
        {
          return (_context.Avaliacao?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
