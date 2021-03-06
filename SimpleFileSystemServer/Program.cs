using SimpleFileSystemServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.UseExceptionHandler("/error");
app.MapControllers();

var _ = new Global(builder.Configuration);

app.Run();
