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

        public ArrendamentosController(ApplicationDbContext context, UserManager<ApplicationUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
		private async Task<List<ApplicationUser>> ObterLocadoresAsync()
		{
			var gestores = await _userManager.GetUsersInRoleAsync("Gestor");
			var adminEmpresas = await _userManager.GetUsersInRoleAsync("AdminEmpresa");
			var funcionario = await _userManager.GetUsersInRoleAsync("Funcionario");

			var locadores = gestores.Union(adminEmpresas).Union(funcionario).Distinct().ToList();

			return locadores;
		}

		// GET: Arrendamentos
		[Authorize(Roles = "Admin,AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Index()
		{
			ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			var arrendamentos = _context.Arrendamento
				.Include(a => a.habitacao)
				.Include(a => a.locador)
				.OrderByDescending(c => c.DataInicio);

			var locadores = await ObterLocadoresAsync();
			ViewData["ListaDeLocadores"] = new SelectList(locadores, "Id", "Email");

			return View(await arrendamentos.ToListAsync());
		}

		[HttpPost]
		[Authorize(Roles = "Admin,AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Index(TipoHabitacao? Tipo, string? Categoria, string? Locador, string? OrderBy)
		{
			ViewData["ListaDeCategorias"] = new SelectList(_context.Categoria.Where(c => c.Disponivel).ToList(), "Id", "Nome");

			var locadores = await ObterLocadoresAsync();
			ViewData["ListaDeLocadores"] = new SelectList(locadores, "Id", "Email");

			var query = _context.Arrendamento
				.Include(a => a.locador)
                .AsQueryable();

			// Aplicar filtro para Tipo se estiver preenchido
			if (Tipo.HasValue)
			{
				query = query.Where(c => c.habitacao.Tipo == Tipo);
			}

			if (!string.IsNullOrEmpty(Locador) && Locador != "Selecione o locador")
			{
				// Supondo que Locador seja o ID do usuário
				var locadorId = Locador;
				query = query.Where(c => c.locador.Id == locadorId);
			}

			// Aplicar filtro de Categoria se estiver preenchido
			if (!string.IsNullOrEmpty(Categoria) && Categoria != "Selecione a categoria")
			{
				int categoriaId;
				if (int.TryParse(Categoria, out categoriaId))
				{
					query = query.Where(c => c.habitacao.Categoria.Id == categoriaId);
				}
			}

			// Aplicar lógica de ordenação
			if (OrderBy == "PrecoCrescente")
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

			var resultado = await query.Include(a => a.habitacao).ToListAsync();
			return View(resultado);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Pesquisa(string TextoAPesquisar, TipoHabitacao? Tipo, DateTime? dataInicio, DateTime? dataFim, int? periodoMinimo)
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
                if (User.IsInRole("Cliente"))
				    pesquisaViewModel.ListaDeArrendamentos = await _context.Arrendamento.Where(c => c.habitacao.Reservado == false).OrderByDescending(c=>c.DataInicio).ToListAsync();
                else
                    pesquisaViewModel.ListaDeArrendamentos = await _context.Arrendamento.Where(c => c.locador.Id.Equals(_userManager.GetUserId)).OrderByDescending(c => c.DataInicio).ToListAsync();
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
		[Authorize(Roles = "Admin,AdminEmpresa,Gestor,Funcionario,Cliente")]
		public async Task<IActionResult> Details(int? id)
        {

			if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .Include(a => a.habitacao)
                .Include(a => a.locador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // GET: Arrendamentos/Create
        [Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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
		[Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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
		[Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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
		[Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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
        [Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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
        //[Authorize(Roles = "AdminEmpresa,Funcionario,Gestor")]
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


        public async Task<IActionResult> MeusArrendamentos() //lado do cliente
        {
			ViewData["MyHabitacoes"] = new SelectList(_context.Arrendamento.Where(c => c.habitacao.ReservadoCliente.Id.Equals(_userManager.GetUserIdAsync)).ToList(), "Id", "Nome");

			var user = await _userManager.GetUserAsync(User);
			var arrendamentos = _context.Arrendamento
                .Include(a => a.habitacao)
                .Include(a => a.locador)
                .Include(a => a.habitacao.ReservadoCliente)
                .Where(a => a.habitacao.ReservadoCliente.Id.Equals(user.Id))
                .OrderByDescending(c => c.DataInicio);

			return View(await arrendamentos.ToListAsync());
		}

        public async Task<IActionResult> ReservarCliente()  //Lado do gestor
        {
            ViewData["ListaHabitacoesSemReserva"] = new SelectList(_context.Arrendamento.Where(c => c.habitacao.Reservado).ToList(), "Id", "Nome");

            var arrendamentos = _context.Arrendamento
                .Include(a => a.habitacao)
                .Include(a => a.locador)
                .Where(a => a.habitacao.Reservado == false)
                .OrderByDescending(c => c.DataInicio);

            var locadores = await ObterLocadoresAsync();
            ViewData["ListaDeLocadores"] = new SelectList(locadores, "Id", "Email");

            return View(await arrendamentos.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservarCliente(int id) //lado do cliente
        {
            if (ModelState.IsValid)
            {
                if (_context.Arrendamento == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
                }
                var arrendamento = await _context.Arrendamento.FindAsync(id);
                if (arrendamento != null)
                {
                    var habit = await _context.Habitacao.FindAsync(arrendamento.habitacaoId);
                    if (habit != null)
                    {
                        habit.Reservado = false;
                        habit.ReservadoCliente = await _userManager.GetUserAsync(User);
                        arrendamento.Aceite = false;
                        habit.QuererReserva = true;
						_context.Habitacao.Update(habit);
						_context.Arrendamento.Update(arrendamento);

						await _context.SaveChangesAsync();
					}
                }
            }
            return RedirectToAction(nameof(Index));
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DesreservarCliente(int id) //lado do cliente
		{
			if (ModelState.IsValid)
			{
				if (_context.Arrendamento == null)
				{
					return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
				}
				var arrendamento = await _context.Arrendamento.FindAsync(id);
				if (arrendamento != null)
				{
					var habit = await _context.Habitacao.FindAsync(arrendamento.habitacaoId);
					if (habit != null)
					{
						habit.Reservado = false;
						habit.ReservadoCliente = null;
						arrendamento.Aceite = false;
						habit.QuererReserva = false;
						_context.Habitacao.Update(habit);
						_context.Arrendamento.Update(arrendamento);

						await _context.SaveChangesAsync();
					}
				}
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> ReservaAceita() //Ver em Aceitar/Negar uma reserva, lado do gestor
        {
			var user = await _userManager.GetUserAsync(User);
			var locadorId = await _userManager.GetUserIdAsync(user);
			var arrendamentos = await _context.Arrendamento
                .Include(a => a.habitacao)
                .Include(a => a.locador)
                .Include(a => a.habitacao.ReservadoCliente)
				.Where(c => c.locadorId == locadorId)
				.ToListAsync();

			if (arrendamentos != null && arrendamentos.Any())
			{
				ViewData["habitacoesAPedirReserva"] = new SelectList(arrendamentos, "Id", "Habitacao", arrendamentos.First());
				return View(arrendamentos);
			}
			else
			{
				return View();
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservaAceita(int id, [Bind("Id, EstadoEntregue, EquipamentosOpcionais, DanosHabitacao, Observacoes")] Arrendamento arrend) //lado do gestor
		{
            if (!ModelState.IsValid)
            {
                if (_context.Arrendamento == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
                }
				var arrendamento = await _context.Arrendamento
							.Include(a => a.habitacao)
                            .Include(a => a.locador)
                            .Include(a => a.habitacao.ReservadoCliente)
							.FirstOrDefaultAsync(a => a.Id == id); 
                if (arrendamento != null && arrendamento.habitacao.QuererReserva)
                {
					var habit = arrendamento.habitacao;
					if (habit != null)
                    {
                        habit.Reservado = true;
                        arrendamento.EstadoEntregue = arrend.EstadoEntregue;
                        arrendamento.EquipamentosOpcionais = arrend.EquipamentosOpcionais;
                        arrendamento.DanosHabitacao = arrend.DanosHabitacao;
                        arrendamento.Aceite = true;
                        _context.Habitacao.Update(habit);
                        _context.Arrendamento.Update(arrendamento);
                    }
                }

                await _context.SaveChangesAsync();
            }
                return RedirectToAction(nameof(Index));
        }
        //Para recusar
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ReservaRejeitada(int id, [Bind("Id, EstadoEntregue, EquipamentosOpcionais, DanosHabitacao, Observacoes")] Arrendamento arrend) //lado do gestor
		{
			if (_context.Arrendamento == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
			}
			var arrendamento = await _context.Arrendamento
						.Include(a => a.habitacao)
						.Include(a => a.locador)
						.Include(a => a.habitacao.ReservadoCliente)
						.FirstOrDefaultAsync(a => a.Id == id);
			if (arrendamento != null && arrendamento.habitacao.QuererReserva)
			{
                arrendamento.habitacao.QuererReserva = false;
				arrendamento.habitacao.Reservado = false;
				arrendamento.Aceite = false;
				_context.Arrendamento.Update(arrendamento);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> EntregaDoCliente()  //lado do gestor
        {
			//ViewData["habitacaoEntregue"] = new SelectList(_context.Arrendamento.Where(c => c.habitacao.Reservado == true && c.habitacao.ReservadoCliente.Id.Equals(_userManager.GetUserIdAsync)).ToList(), "Id", "Nome");
			ViewData["habitacaoEntregue"] = new SelectList(_context.Arrendamento.Where(c => c.habitacao.Reservado == true && c.habitacao.ReservadoCliente.Id.Equals(_userManager.GetUserIdAsync)).ToList(), "Id", "Nome");
			return View();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EntregaDoCliente(int id, [Bind("Id, EstadoRecebido, EquipamentosOpcionaisC, DanosHabitacaoC, ObservacoesC")] Arrendamento arrend) //lado do gestor
        {
            if (id != arrend.Id)
            {
                return NotFound();
            }
            if (_context.Arrendamento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
            }
            if (ModelState.IsValid && arrend.EstadoRecebido != null)
            {
                try
                {
                    var arrendamento = await _context.Arrendamento.FindAsync(id);
                    if (arrendamento != null)
                    {
                        arrendamento.EstadoRecebido = arrend.EstadoRecebido;
                        arrendamento.EquipamentosOpcionaisC = arrend.EquipamentosOpcionaisC;
                        arrendamento.DanosHabitacaoC = arrend.DanosHabitacaoC;
                        arrendamento.ObservacoesC = arrend.ObservacoesC;
                        arrendamento.habitacao.Reservado = false;
                        arrendamento.habitacao.ReservadoCliente = null;
                        arrendamento.habitacao.QuererReserva = false;
                        arrendamento.Aceite = false;
                        _context.Arrendamento.Update(arrendamento);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArrendamentoExists(arrend.Id))
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
            ViewData["habitacaoEntregue"] = new SelectList(_context.Habitacao, "Id", "Descricao", arrend.habitacaoId);
            return View(nameof(Index));
        }


        ///Para adicionar o texto
        public async Task<IActionResult> AceitarReserva(int? id)
        {
			if (id == null || _context.Arrendamento == null)
			{
				return NotFound();
			}

			var arrendamento = await _context.Arrendamento
				.FirstOrDefaultAsync(m => m.Id == id);
			if (arrendamento == null)
			{
				return NotFound();
			}

			return View(nameof(AceitarReserva), arrendamento);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarReserva(int id, [Bind("Id, EstadoEntregue, EquipamentosOpcionais, DanosHabitacao, Observacoes")] Arrendamento arrend) //lado do gestor
        {
			if (id != arrend.Id)
			{
				return NotFound();
			}
			if (_context.Arrendamento == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
			}
			if (ModelState.IsValid && arrend.EstadoEntregue != null)
			{
				try
				{
					var arrendamento = await _context.Arrendamento.FindAsync(id);
					if (arrendamento != null)
					{
						arrendamento.EstadoEntregue = arrend.EstadoEntregue;
						arrendamento.EquipamentosOpcionais = arrend.EquipamentosOpcionais;
						arrendamento.DanosHabitacao = arrend.DanosHabitacao;
						arrendamento.Observacoes = arrend.Observacoes;
						_context.Arrendamento.Update(arrendamento);
					}
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ArrendamentoExists(arrend.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}
			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> EntregaDeCliente(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(nameof(EntregaDeCliente), arrendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EntregaDeCliente(int id, [Bind("Id, EstadoRecebido, EquipamentosOpcionaisC, DanosHabitacaoC, ObservacoesC")] Arrendamento arrend) //lado do gestor
        {
            if (id != arrend.Id)
            {
                return NotFound();
            }
            if (_context.Arrendamento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Arrendamento' is null.");
            }
            if (ModelState.IsValid && arrend.EstadoRecebido != null)
            {
                try
                {
                    var arrendamento = await _context.Arrendamento.FindAsync(id);
                    if (arrendamento != null)
                    {
                        arrendamento.EstadoRecebido = arrend.EstadoRecebido;
                        arrendamento.EquipamentosOpcionaisC = arrend.EquipamentosOpcionaisC;
                        arrendamento.DanosHabitacaoC = arrend.DanosHabitacaoC;
                        arrendamento.ObservacoesC = arrend.ObservacoesC;
                        _context.Arrendamento.Update(arrendamento);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArrendamentoExists(arrend.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
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