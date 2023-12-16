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
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;

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

		private async Task<List<ApplicationUser>> ObterLocadoresAsync()
		{
			var usersWithRoles = await _userManager.GetUsersInRoleAsync("AdminEmpresa");
			var distinctUsers = usersWithRoles.Distinct().ToList();

			return distinctUsers;
		}

		// GET: Habitacao
		public async Task<IActionResult> Index()
        {
	        ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			var locadores = await ObterLocadoresAsync();
			ViewData["ListaDeLocadores"] = new SelectList(locadores, "Id", "Email");

			return View(await _context.Habitacao.ToListAsync());
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Pesquisa(TipoHabitacao? Tipo, string? Categoria, string? OrderByAct)
		{
			PesquisaViewModel pesquisaViewModel = new PesquisaViewModel();

            bool estado = false;

            if (OrderByAct != null && OrderByAct.Equals("Ativo"))
                estado = true;

			pesquisaViewModel.ListaDehabitacoes = await _context.Habitacao.Where(c => c.Categoria.Nome.ToLower() == Categoria.ToLower()
                                                || c.Tipo == Tipo
                                                || c.Estado == estado).ToListAsync();
            ViewData["ListaDeHabitacao"] = pesquisaViewModel;
            return View(ViewData["ListaDeHabitacao"]);
		}

		public async Task<IActionResult> ParqueIndex()
		{
			var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Funcionario"))
                ViewData["Lista"] = await _context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id).ToListAsync();
            else if (User.IsInRole("Gestor"))
                ViewData["Lista"] = await _context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id).ToListAsync();
            else
                ViewData["Lista"] = await _context.Habitacao.ToListAsync();

			return View(/*await _context.Habitacao.ToListAsync()*/ ViewData["Lista"]);
		}

        [HttpPost]
        [Authorize(Roles = "Admin,AdminEmpresa,Gestor,Funcionario,Cliente")]
        public async Task<IActionResult> ParqueIndex(TipoHabitacao? Tipo, string? Categoria, string? OrderByAct)
        {
            ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

            var query = _context.Habitacao.AsQueryable();

            // Aplicar filtro para Tipo se estiver preenchido
            if (Tipo.HasValue)
            {
                query = query.Where(c => c.Tipo == Tipo);
            }

            // Aplicar filtro de Categoria se estiver preenchido
            if (!string.IsNullOrEmpty(Categoria) && Categoria != "Selecione a categoria")
            {
                int categoriaId;
                if (int.TryParse(Categoria, out categoriaId))
                {
                    query = query.Where(c => c.Id == categoriaId);
                }
            }

            // Aplicar lógica de ordenação
            if (OrderByAct == "Ativo")
            {
                query = query.OrderBy(c => c.Estado);
            }
            else
            {
                query = query.OrderByDescending(c => !c.Estado);
            }

            var resultado = await query.ToListAsync();
            return View(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> ParqueIndexPesquisa(TipoHabitacao? Tipo, string? Categoria, string? OrderByAct)
		{
			var user = await _userManager.GetUserAsync(User);
			bool estado = false;

			if (OrderByAct != null && OrderByAct.Equals("Ativo"))
				estado = true;

			if (User.IsInRole("Funcionario"))
				ViewData["Lista"] = await _context.Habitacao.Where(c => c.FuncionarioDaHabitacaoId == user.Id
                                        && (c.Categoria.Nome.ToLower() == Categoria.ToLower()
												|| c.Tipo == Tipo
												|| c.Estado == estado)).ToListAsync();
			else if (User.IsInRole("Gestor"))
				ViewData["Lista"] = await _context.Habitacao.Where(c => c.GestorDaHabitacaoId == user.Id
										&& (c.Categoria.Nome.ToLower() == Categoria.ToLower()
												|| c.Tipo == Tipo
												|| c.Estado == estado)).ToListAsync();
			else
				ViewData["Lista"] = await _context.Habitacao.Where(c => c.Categoria.Nome.ToLower() == Categoria.ToLower()
												|| c.Tipo == Tipo
												|| c.Estado == estado).ToListAsync();

			return View(/*await _context.Habitacao.ToListAsync()*/ ViewData["Lista"]);
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
		public async Task<IActionResult> Create([Bind("Id,Localizacao,Tipo,CategoriaId,Descricao")] Habitacao habitacao)
        {
	        ModelState.Remove(nameof(Habitacao.Categoria));

			if (ModelState.IsValid)
            {
                habitacao.Reservado = false;
                habitacao.Estado = true;
                habitacao.EstadoHabitacao = "Disponivel, em condições novas";

				var funcId = await _userManager.GetUserAsync(User);
				if (User.IsInRole("Funcionario"))
                {
                    habitacao.FuncionarioDaHabitacaoId = funcId.Id;
                }
                if (User.IsInRole("Gestor"))
                {
					habitacao.GestorDaHabitacaoId = funcId.Id;
				}
                habitacao.EmpresaId = funcId.empresaId;
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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
		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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

		[Authorize(Roles = "AdminEmpresa,Gestor,Funcionario")]
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
