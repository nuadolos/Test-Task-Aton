using TestDB.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//ƒобавление пула контекстов UserContext,
//  указав вручную строку подключени€
//  (без этого проект невозможно открыть с помощью команд cmd)
builder.Services.AddDbContextPool<UserContext>(options =>
    options.UseSqlServer(
        "Server=.\\sqlexpress;Database=TestDB;Trusted_Connection=True;"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

//ѕолучение контекста дл€ повторного воссоздани€ базы данных
//  с помощью миграции, наход€ща€с€ в папке Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UserContext>();

    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
