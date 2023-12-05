using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ficha1_P1_V1.Data;
using Ficha1_P1_V1.Models;
using Microsoft.AspNetCore.Identity;

namespace Ficha1_P1_V1.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empresa
        public async Task<IActionResult> Index()
        {
              return _context.Empresa != null ? 
                          View(await _context.Empresa.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Empresa'  is null.");
        }

        // GET: Empresa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(m => m.EmpresaId == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpresaId,Nome,Localidade,Telefone,Email")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();

                var lastEmpresaId = _context.Empresa.Max(c => c.EmpresaId);
                //Default User para esta Empresa - Admin
                var defaultUser = new ApplicationUser
                {
                    empresaId = lastEmpresaId,
                    UserName = "admin" + empresa.EmpresaId + "@localhost.com",
                    Email = "admin" + empresa.EmpresaId + "@localhost.com",
                    PrimeiroNome = "Administrador" + empresa.EmpresaId,
                    UltimoNome = "Empresa",
                    EmailConfirmed = true, //Importante desbloquear (confirmar para usar logo a conta)
                    PhoneNumberConfirmed = true
                };
                
                var user = await _userManager.FindByEmailAsync(defaultUser.Email);
                if (_userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    if (user == null)
                    {
                        await _userManager.CreateAsync(defaultUser, "Is3C..00"); //Password do AdminEmpresa
                        await _userManager.AddToRoleAsync(defaultUser,
                        Roles.AdminEmpresa.ToString());
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "EmpresaId", "Email", empresa.EmpresaId);
            return View(empresa);
        }

        // GET: Empresa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }

        // POST: Empresa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpresaId,Nome,Localidade,Telefone,Email")] Empresa empresa)
        {
            if (id != empresa.EmpresaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.EmpresaId))
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
            return View(empresa);
        }

        // GET: Empresa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(m => m.EmpresaId == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresa == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Empresa'  is null.");
            }
            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa != null)
            {
                _context.Empresa.Remove(empresa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
          return (_context.Empresa?.Any(e => e.EmpresaId == id)).GetValueOrDefault();
        }


        //Admin Empresa

        public async Task<IActionResult> ListaEmpresa(string id)
        {
            var users = _userManager.Users.Where(u => u.empresaId.ToString() == id);
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserEmpresa(string RoleName, string email)
        {
            var user = await _userManager.FindByIdAsync(email);
            if (user != null && RoleName != null)
            {
                await _userManager.AddToRoleAsync(user, RoleName);
            }
            return RedirectToAction("ListaEmpresa", new { id = user.empresaId });
        }
    }
}
