using AutoMapper;
using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers {
    public class CategoriesController: Controller {

        private readonly ITypesAccountRepository typesAccountRepository;
        private readonly IUsersService usersService;
        private readonly IAccountsRepository accountsRepository;
        private readonly IMapper mapper;
        private readonly ICategoriesRepository categoriesRepository;

        public CategoriesController(ITypesAccountRepository typesAccountRepository,
            IUsersService usersService,
            IAccountsRepository accountsRepository,
            IMapper mapper,
            ICategoriesRepository categoriesRepository) {
            this.typesAccountRepository = typesAccountRepository;
            this.usersService = usersService;
            this.accountsRepository = accountsRepository;
            this.mapper = mapper;
            this.categoriesRepository = categoriesRepository;
        }

        [HttpGet]
        public IActionResult Create() {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category) {
            if (!ModelState.IsValid) {
                return View(category);
            }

            var userId = usersService.GetUserId();
            category.UserId = userId;
            await categoriesRepository.Create(category);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index() {
            var userId = usersService.GetUserId();
            var categories = await categoriesRepository.GetAllForUser(userId);
            return View(categories);
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            var userId = usersService.GetUserId();
            var category = await categoriesRepository.GetById(id, userId);
            if (category == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryUpdate) {
            if (!ModelState.IsValid) {
                return View(categoryUpdate);
            }
            var userId = usersService.GetUserId();
            var categoryDb = await categoriesRepository.GetById(categoryUpdate.Id, userId);
            if (categoryDb == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            categoryUpdate.UserId = userId;
            await categoriesRepository.Update(categoryUpdate);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            var userId = usersService.GetUserId();
            var category = await categoriesRepository.GetById(id, userId);
            if (category == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            var userId = usersService.GetUserId();
            var categoryDb = await categoriesRepository.GetById(id, userId);
            if (categoryDb == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            await categoriesRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
