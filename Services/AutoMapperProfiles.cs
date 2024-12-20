﻿using AutoMapper;
using BudgetManagement.Models;

namespace BudgetManagement.Services {
    public class AutoMapperProfiles: Profile {

        public AutoMapperProfiles() {

            CreateMap<Account, AccountCreateViewModel>();
            CreateMap<TransactionsUpdateViewModel, Transaction>().ReverseMap();
        }
    }
}
