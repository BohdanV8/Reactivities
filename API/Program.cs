using API.Middlewares;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Intarfaces;
using Domain;
using FluentValidation;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(opt =>
{
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddOpenApi();
builder.Services.AddMediatR(x => {
    x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>(); 
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfiles).Assembly);
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsActivityHost", policy =>
    {
        policy.AddRequirements(new IsHostRequirment());
    });
});

builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService, PhotoService>();
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000", "https://localhost:3000"));
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
using var scope = app.Services.CreateScope();
//не можна
try
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DBInitializer.seedData(context, scope.ServiceProvider.GetRequiredService<UserManager<User>>());
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("/api").MapIdentityApi<User>();
////// --- ТИМЧАСОВИЙ МАРШРУТ ДЛЯ ДЕБАГУ ---
//app.MapGet("/api/test-user", async (UserManager<User> userManager) =>
//{
//    // 1. Чи взагалі Identity бачить цього юзера по Email?
//    var user = await userManager.FindByEmailAsync("tom@gmail.com");
//    if (user == null)
//    {
//        return Results.NotFound("Identity каже: Користувача james@gmail.com НЕ ЗНАЙДЕНО!");
//    }

//    // 2. Чи підходить пароль?
//    var isPasswordCorrect = await userManager.CheckPasswordAsync(user, "Pa$$w0rd");

//    // 3. Віддаємо всю правду
//    return Results.Ok(new
//    {
//        Message = "Юзера знайдено!",
//        Username = user.UserName,
//        PasswordIsCorrect = isPasswordCorrect,
//        EmailConfirmed = user.EmailConfirmed,
//        PasswordHash = user.PasswordHash?.Substring(0, 15) + "..." // Покажемо початок хешу
//    });
//});
////// ------------------------------------

app.Run();
