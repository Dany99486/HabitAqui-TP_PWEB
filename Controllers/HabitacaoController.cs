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
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Ficha1_P1_V1.ViewModels;

namespace Ficha1_P1_V1.Controllers
{
    public class HabitacaoController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public HabitacaoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Habitacao
        public async Task<IActionResult> Index()
        {
	        ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			return View(await _context.Habitacao.ToListAsync());
        }

        /*
		public async Task<IActionResult> Pesquisar(string? TextoAPesquisar)
		{
			PesquisaViewModel pesquisaVM = new PesquisaViewModel();
			ViewData["Title"] = "Pesquisar cursos";

			if (string.IsNullOrWhiteSpace(TextoAPesquisar))
				pesquisaVM.ListaDehabitacoes = await _context.Habitacao.Include("categoria").OrderBy(c => c.Categoria).ToListAsync();
			else
			{
                pesquisaVM.ListaDehabitacoes =
                    await _context.Categoria.Include("categoria").Where(c => c.Nome.Contains(TextoAPesquisar);
				pesquisaVM.TextoAPesquisar = TextoAPesquisar;
				foreach (Curso c in pesquisaVM.ListaDeCursos)
				{
					c.Nome = AltCorSubSTR(c.Nome, pesquisaVM.TextoAPesquisar);
					c.DescricaoResumida = AltCorSubSTR(c.DescricaoResumida, pesquisaVM.TextoAPesquisar);
				}
			}
			pesquisaVM.NumResultados = pesquisaVM.ListaDeCursos.Count();

			return View(pesquisaVM);
		}*/

		public async Task<IActionResult> ParqueIndex()
		{
			var user = await _userManager.GetUserAsync(User);
			if (User.IsInRole("Funcionario"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else if (User.IsInRole("Gestor"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else
				ViewData["Lista"] = new SelectList(_context.Habitacao.ToList().ToList(), "Id", "Nome");

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
                habitacao.Reservado = false;
                habitacao.Estado = true;

				var funcId = await _userManager.GetUserAsync(User);
				if (User.IsInRole("Funcionario"))
                {
                    habitacao.FuncionarioDaHabitacaoId = funcId.Id;
                }
                if (User.IsInRole("Gestor"))
                {
					habitacao.GestorDaHabitacaoId = funcId.Id;
				}
                _context.Add(habitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ParqueIndex));
            }
            if (User.IsInRole("Cliente") || User.IsInRole("Inativo"))
            {
				ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");
			}
			var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Funcionario"))
			    ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
            else if (User.IsInRole("Gestor"))
                ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
            else
                ViewData["Lista"] = new SelectList(_context.Habitacao.ToList().ToList(), "Id", "Nome");

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
			if (User.IsInRole("Cliente") || User.IsInRole("Inativo"))
			{
				ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");
			}
			var user = await _userManager.GetUserAsync(User);
			if (User.IsInRole("Funcionario"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else if (User.IsInRole("Gestor"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else
				ViewData["Lista"] = new SelectList(_context.Habitacao.ToList().ToList(), "Id", "Nome");

			return View(habitacao);
        }

        // POST: Habitacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Localizacao,Tipo,CategoriaId,Descricao,estado,reservado")] Habitacao habitacao)
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
                return RedirectToAction(nameof(ParqueIndex));
            }
			if (User.IsInRole("Cliente") || User.IsInRole("Inativo"))
			{
				ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");
			}
			var user = await _userManager.GetUserAsync(User);
			if (User.IsInRole("Funcionario"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else if (User.IsInRole("Gestor"))
				ViewData["Lista"] = new SelectList(_context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id).ToList(), "Id", "Nome");
			else
				ViewData["Lista"] = new SelectList(_context.Habitacao.ToList().ToList(), "Id", "Nome");

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
                return Problem("Entity set 'ApplicationDbContext.Habitacao' is null.");
            }
			var habitacao = await _context.Habitacao.FindAsync(id);
			
			if (habitacao != null)
            {
                if (habitacao.Reservado)
                {
                    ModelState.AddModelError("Reservado", "Não é possível apagar uma habitação reservada");
                }
                else
                {
                    _context.Habitacao.Remove(habitacao);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ParqueIndex));
        }

        private bool HabitacaoExists(int id)
        {
          return (_context.Habitacao?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AtivaDesativa(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hab = await _context.Habitacao.FindAsync(id);

            if (hab == null)
            {
                return NotFound();
            }

            if (hab.Estado)
            {
                hab.Estado = false;
            }
            else
            {
                hab.Estado = true;
            }

            ModelState.Remove(nameof(Habitacao));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hab);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacaoExists(hab.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(ParqueIndex));
        }
    }
}
