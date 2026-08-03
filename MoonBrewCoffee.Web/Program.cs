using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoonBrewCoffee.Web;
using MoonBrewCoffee.Application.Profiles;
using MoonBrewCoffee.Application.Services.Implementations;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Implementations;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Repository.Implementations;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;
using MoonBrewCoffee.Web.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<MoonBrewContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MoonBrew.Session.V2";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICartService, SessionCartService>();

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IIngredienteRepository, IngredienteRepository>();
builder.Services.AddScoped<IComboRepository, ComboRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IIngredienteService, IngredienteService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IProductoIngredienteRepository, ProductoIngredienteRepository>();
builder.Services.AddScoped<IProcesoPreparacionRepository,ProcesoPreparacionRepository>();
builder.Services.AddScoped<IProcesoPreparacionService,ProcesoPreparacionService>();
builder.Services.AddScoped<IEstacionCocinaRepository,EstacionCocinaRepository>();
builder.Services.AddScoped<IEstacionCocinaService,EstacionCocinaService>();
builder.Services.AddScoped<IProductoIngredienteService, ProductoIngredienteService>();
builder.Services.AddScoped<IComboProductoRepository, ComboProductoRepository>();
builder.Services.AddScoped<IComboProductoService, ComboProductoService>();
builder.Services.AddScoped<IMenuProductoRepository,MenuProductoRepository>();
builder.Services.AddScoped<IMenuComboRepository,MenuComboRepository>();
builder.Services.AddScoped<IMenuProductoService,MenuProductoService>();
builder.Services.AddScoped<IMenuComboService,MenuComboService>();


// Todos los perfiles se encuentran en el ensamblado Application.
// Registrar el ensamblado una sola vez evita configuraciones duplicadas.
builder.Services.AddAutoMapper(_ => { }, typeof(UsuarioProfile).Assembly);

// Internacionalización centralizada para toda la interfaz.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("es-CR"),
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("es-CR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseSession();

app.UseRouting();

var administrativeControllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "Home",
    "Dashboard",
    "Productos",
    "Categorias",
    "Ingredientes",
    "Combos",
    "Menus",
    "Usuarios",
    "ProcesosPreparacion"
};

app.Use(async (context, next) =>
{
    var controller = context.GetRouteValue("controller")?.ToString();
    var action = context.GetRouteValue("action")?.ToString();
    var isPublicImage = string.Equals(action, "Image", StringComparison.OrdinalIgnoreCase) &&
        (string.Equals(controller, "Productos", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(controller, "Combos", StringComparison.OrdinalIgnoreCase));

    if (!isPublicImage && controller is not null && administrativeControllers.Contains(controller))
    {
        var currentUserService = context.RequestServices.GetRequiredService<ICurrentUserService>();
        var currentUser = currentUserService.GetCurrent();

        if (currentUser is null)
        {
            var returnUrl = $"{context.Request.Path}{context.Request.QueryString}";
            var loginUrl = $"/Cuenta/IniciarSesion?returnUrl={Uri.EscapeDataString(returnUrl)}";
            context.Response.Redirect(loginUrl);
            return;
        }

        if (!currentUser.EsAdministrador)
        {
            context.Response.Redirect("/Cliente");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
