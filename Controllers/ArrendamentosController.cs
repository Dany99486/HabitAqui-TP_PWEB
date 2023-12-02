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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

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

        [HttpPost]
        public IActionResult Index(TipoHabitacao? Tipo, int? Quartos, string? OrderBy)
        {
	        ViewData["ListaDeCategorias"] = new SelectList(_context.Habitacao.OrderBy(c => c.Localizacao).ToList(), "Id", "Localizacao");
	        var query = _context.Arrendamento.AsQueryable();
	        // Aplicar filtro para TextoAPesquisar se estiver preenchido
	        // Aplicar filtro para Tipo se estiver preenchido
	        if (Tipo.HasValue)
	        {
		        query = query.Where(c => c.habitacao.Tipo == Tipo);
		        //query = query.Where(c => c.DataFim > DateTime.Now);
	        }

	        if (OrderBy== "PrecoCrescente")
	        {
		        query = query.OrderBy(c => c.Preco);
	        }
            else if (OrderBy == "PrecoDecrescente")
	        {
		        query = query.OrderByDescending(c => c.Preco);
	        }
	        else if (OrderBy == "AvaliacaoCrescente")
	        {
		        query = query.OrderBy(c => c.Avaliacao);
	        }
	        else if (OrderBy == "AvaliacaoDecrescente")
	        {
		        query = query.OrderByDescending(c => c.Avaliacao);
	        }
	        if (Quartos.HasValue)
	        {
		        query = query.Where(c => c.habitacao.Quartos >= Quartos);
	        }


			var resultado = query.ToList();
	        return View(resultado);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
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
                arrendamento.Avaliacao = null; //Ainda não foi avaliado

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

/*
        public async Task<IActionResult> DetalhesAvaliacao(int? habitacaoId)
        {
            var habitacaoArrendada = _context.Arrendamento
                .SingleOrDefault(h => h.Id == habitacaoId);

            if (habitacaoArrendada == null)
            {
                // Lidar com o caso em que a habitação não está arrendada
                return View();
            }

            var viewModel = new Arrendamento
            {
                habitacaoId = habitacaoArrendada.Id
                // Adicionar outras propriedades necessárias
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> DetalhesAvaliacao(int? habitacaoId, int x)
        {
            /*var viewModel = new Arrendamento
            {
                habitacaoId = habitacaoId
            };

            return View(viewModel);

            /*
            if (User == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                // Obtém o ID do Utilizador a partir do contexto do utilizador
                string utilizadorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (utilizadorId != null)
                {
                }
            }

            return RedirectToAction("Login", "Conta");

            //var u = await _userManager.GetUserAsync(User);

            //if (u != null)
            //{
                //int uId = Convert.ToInt32(u.Id);

                var habitacaoArrendada = _context.Arrendamento
                    .SingleOrDefault(h => h.Id == habitacaoId);

                if (habitacaoArrendada == null)
                {
                    // Lidar com o caso em que a habitação não está arrendada
                    return NotFound();
                }

                var viewModel = new Arrendamento
                {
                    habitacaoId = habitacaoArrendada.Id
                    // Adicionar outras propriedades necessárias
                };

                return View(viewModel);
            //}

            // Se não estiver autenticado, vai para a página de login
            //return RedirectToAction("Login", "Conta");
        }
        
        public async Task<IActionResult> CriaAvaliacao(int? id)
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
            ViewData["ListaDeArrendamentos"] = new SelectList(_context.Arrendamento.OrderBy(c => c.Avaliacao).ToList(), "Id", "Avaliacao", arrendamento.Avaliacao);

            return View(arrendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriaAvaliacao(int id, [Bind("avaliacao")] Arrendamento arrendamento)
        {
            if (id != arrendamento.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(arrendamento.Avaliacao));

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
            //ViewData["Avaliacao"] = new SelectList(_context.Arrendamento.OrderBy(c => c.Avaliacao).ToList(), "Id", "Avaliacao", arrendamento.Avaliacao);

            return View();
        }
        */