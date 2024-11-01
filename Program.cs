using BudgetManagement.Interfaces;
using BudgetManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITypesAccountRepository, TypesAccountRepository>();
builder.Services.AddTransient<IUsersService, UsersServices>();
builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.Services.AddTransient<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddTransient<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddTransient<IReportService, ReportService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transactions}/{action=Index}/{id?}");

await app.RunAsync();
