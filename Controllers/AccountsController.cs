﻿using AutoMapper;
using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Controllers {
    public class AccountsController: Controller {
        private readonly ITypesAccountRepository typesAccountRepository;
        private readonly IUsersService usersService;
        private readonly IAccountsRepository accountsRepository;
        private readonly IMapper mapper;

        public AccountsController(ITypesAccountRepository typesAccountRepository, 
            IUsersService usersService, 
            IAccountsRepository accountsRepository,
            IMapper mapper) {
            this.typesAccountRepository = typesAccountRepository;
            this.usersService = usersService;
            this.accountsRepository = accountsRepository;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index() {
            var userId = usersService.GetUserId();
            var accounts = await accountsRepository.Search(userId);

            var model = accounts
                .GroupBy(x => x.AccountType)
                .Select(group => new IndexAccountsViewModel {
                AccountType = group.Key,
                Accounts = group.AsEnumerable()
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create() {

            var userId = usersService.GetUserId();

            var viewModel = new AccountCreateViewModel {
                AccountsTypes = await GetTypesAccount(userId)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreateViewModel account) {


            var userId = usersService.GetUserId();
            var accountType = await typesAccountRepository.GetById(account.AccountTypeId, userId);

            if (accountType == null) {
               return RedirectToAction("MineNotFound", "Home");
            }

            if (!ModelState.IsValid) {
                account.AccountsTypes = await GetTypesAccount(userId);
                return View(account);
            }

            await accountsRepository.Create(account);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {

            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetForId(id, userId);
            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            if (!ModelState.IsValid) {
                return View(account);
            }

            var viewModel = mapper.Map<AccountCreateViewModel>(account);
            viewModel.AccountsTypes = await GetTypesAccount(userId);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountCreateViewModel accountToEdit) {

            var userId = usersService.GetUserId();
            var accountType = await typesAccountRepository.GetById(accountToEdit.AccountTypeId, userId);

            if (accountType == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            var account = await accountsRepository.GetForId(accountToEdit.Id, userId);
            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            if (!ModelState.IsValid) {
                accountToEdit.AccountsTypes = await GetTypesAccount(userId);
                return View(account);
            }

            await accountsRepository.Update(accountToEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetForId(id, userId);
            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            if (!ModelState.IsValid) {
                return View(account);
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }

            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetForId(id, userId);
            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            await accountsRepository.Delete(account.Id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> GetTypesAccount(int userId) {
            var typesAccounts = await typesAccountRepository.GetTypesAccount(userId);
            return typesAccounts.Select(x => new SelectListItem {
                Text = x.Name,
                Value = x.Id.ToString()
            });

        }

    }
}