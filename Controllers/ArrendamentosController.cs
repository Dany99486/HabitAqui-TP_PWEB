using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ficha1_P1_V1.Data;
using Ficha1_P1_V1.Models;
using Ficha1_P1_V1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Ficha1_P1_V1.Controllers
{
    public class ArrendamentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArrendamentosController(ApplicationDbContext context,UserManager<ApplicationUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Arrendamentos
        public async Task<IActionResult> Index()
        {
	        var arrendamentos = _context.Arrendamento.Include(a => a.habitacao).OrderByDescending(c=>c.DataInicio);//Include(a => a.locador);
            return View(await arrendamentos.ToListAsync());
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
		public async Task<IActionResult> Pesquisa(string TextoAPesquisar, TipoHabitacao? Tipo,DateTime? dataInicio,DateTime? dataFim,int? periodoMinimo)
        {
            PesquisaViewModel pesquisaViewModel = new PesquisaViewModel();
			ViewData["Title"] ="Pesquisar Habitações";


			if (string.IsNullOrWhiteSpace(TextoAPesquisar)
				    ||!Tipo.HasValue
				    ||!dataInicio.HasValue
				    ||!dataFim.HasValue
				    ||!periodoMinimo.HasValue
			    )
			{
                pesquisaViewModel.NumResultados = -1;
					pesquisaViewModel.ListaDeArrendamentos=await _context.Arrendamento.OrderByDescending(c=>c.DataInicio).ToListAsync();
			}
			else
			{
				pesquisaViewModel.ListaDeArrendamentos =
					await _context.Arrendamento.Where(c => c.habitacao.Localizacao.Contains(TextoAPesquisar)
													&& c.habitacao.Tipo == Tipo
                                                    && c.DataInicio <= dataInicio
                                                    && c.DataFim >= dataFim
                                                    && c.PeriodoMinimo <= periodoMinimo
													).ToListAsync();
                pesquisaViewModel.NumResultados = pesquisaViewModel.ListaDeArrendamentos.Count();
			}

			return View(pesquisaViewModel);
        }

		// GET: Arrendamentos/Details/5

		//[Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .Include(a => a.habitacao)
                //.Include(a => a.locador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // GET: Arrendamentos/Create

        //[Authorize(Roles = "Funcionario,Gestor")]
        public IActionResult Create()
        {
            ViewData["habitacaoId"] = new SelectList(_context.Habitacao, "Id", "Descricao");
            //ViewData["locadorId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            return View();
        }

        // POST: Arrendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Create([Bind("Id,DataInicio,DataFim,PeriodoMinimo,PeriodoMaximo,Preco,habitacaoId")] Arrendamento arrendamento)
        {
            ModelState.Remove(nameof(arrendamento.habitacao));

            ModelState.Remove(nameof(arrendamento.locador));
            ModelState.Remove(nameof(arrendamento.locadorId));

            if (ModelState.IsValid)
            {
                arrendamento.locadorId = _userManager.GetUserId(User);
                arrendamento.DataInicio = DateTime.Now;
                _context.Add(arrendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["habitacaoId"] = new SelectList(_context.Habitacao, "Id", "Descricao", arrendamento.habitacaoId);
            //ViewData["locadorId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", arrendamento.locadorId);
            return View(arrendamento);
        }

        // GET: Arrendamentos/Edit/5
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento.FindAsync(id);
            if (arrendamento == null)
            {
                return NotFound();
            }
            ViewData["habitacaoId"] = new SelectList(_context.Habitacao, "Id", "Id", arrendamento.habitacaoId);
            //ViewData["locadorId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", arrendamento.locadorId);
            return View(arrendamento);
        }

        // POST: Arrendamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataInicio,DataFim,PeriodoMinimo,PeriodoMaximo,Preco,habitacaoId")] Arrendamento arrendamento)
        {
            arrendamento.DataInicio = DateTime.Now;
            if (id != arrendamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arrendamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArrendamentoExists(arrendamento.Id))
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
            ViewData["habitacaoId"] = new SelectList(_context.Habitacao, "Id", "Descricao", arrendamento.habitacaoId);
            //ViewData["locadorId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", arrendamento.locadorId);
            return View(arrendamento);
        }

        // GET: Arrendamentos/Delete/5
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .Include(a => a.habitacao)
                //.Include(a => a.locador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // POST: Arrendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Arrendamento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Arrendamento'  is null.");
            }
            var arrendamento = await _context.Arrendamento.FindAsync(id);
            if (arrendamento != null)
            {
                _context.Arrendamento.Remove(arrendamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArrendamentoExists(int id)
        {
          return (_context.Arrendamento?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
