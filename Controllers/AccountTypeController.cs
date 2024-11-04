using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers {
    public class AccountTypeController: Controller {
        private readonly ITypesAccountRepository _typesAccountRepository;
        private readonly IUsersService _usersService;

        public AccountTypeController(ITypesAccountRepository typesAccountRepository, IUsersService usersService) {
            _typesAccountRepository = typesAccountRepository;
            // Add the users service for validation in the Create action
            _usersService = usersService;
        }


        public async Task<IActionResult> Index(PaginationViewModel pagination) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            var userId = _usersService.GetUserId();
            var accountTypes = await _typesAccountRepository.GetTypesAccount(userId, pagination);
            var response = new PaginationResponseViewModel<AccountType> {
                TotalRecords = await _typesAccountRepository.GetTotalRecords(userId),
                CurrentPage = pagination.Page,
                RecordsPerPage = pagination.RecordsPerPage,
                Data = accountTypes.ToList(),
                BaseURL = Url.Action()
            };
            return View(response);
        }


        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType) {

            if (!ModelState.IsValid) {
                return View(accountType);
            }

            accountType.UserId = _usersService.GetUserId();

            var accountTypeExits = await _typesAccountRepository.TypeAccountExitsForUserId(accountType.Name, accountType.UserId);

            if (accountTypeExits) {
                ModelState.AddModelError(nameof(accountType.Name), $"Account type {accountType.Name} already exists");
                return View(accountType);
            }

            await _typesAccountRepository.Create(accountType);

            // Redirect to the list of type of accounts action after successful creation
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var accountType = await _typesAccountRepository.GetById(id, userId);
            if (accountType == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountType accountType) {
            if (!ModelState.IsValid) {
                return View(accountType);
            }
            accountType.UserId = _usersService.GetUserId();
            var accountTypeExits = await _typesAccountRepository.GetById(accountType.Id, accountType.UserId);
            if (accountTypeExits is null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            await _typesAccountRepository.Update(accountType);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var accountType = await _typesAccountRepository.GetById(id, userId);
            if (accountType == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var accountType = await _typesAccountRepository.GetById(id, userId);
            if (accountType == null) {
                return RedirectToAction("MineNotFound", "Home");
            }
            await _typesAccountRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CheckTypeAccountExits(string name, int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var accountTypeExits = await _typesAccountRepository.TypeAccountExitsForUserId(name, userId, id);

            if (accountTypeExits) {
                return Json($"Account type {name} already exists");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Sort([FromBody] int[] sortedIds, PaginationViewModel pagination) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var userId = _usersService.GetUserId();
            var accountTypes = await _typesAccountRepository.GetTypesAccount(userId, pagination);
            var idsAccountsType = accountTypes.Select(x => x.Id);
            var idsAccountsTypeNotUser = idsAccountsType.Except(sortedIds).ToList();
            if (idsAccountsTypeNotUser.Count > 0) {
                return Forbid();
            }

            var sorterAccountTypes = sortedIds.Select((id, index) => new AccountType { Id = id, Order = index + 1 }).AsEnumerable();

            await _typesAccountRepository.Sort(sorterAccountTypes);


            return Ok();
        }
    }
}
