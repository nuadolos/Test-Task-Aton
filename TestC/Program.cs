using TestDB.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Добавление пула контекстов UserContext,
//  указав строку подключения из appsettings.json
builder.Services.AddDbContextPool<UserContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TestConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

//Получение контекста для повторного воссоздания базы данных
//  с помощью миграции, находящаяся в папке Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UserContext>();

    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
