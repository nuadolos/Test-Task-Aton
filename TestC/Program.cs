using TestDB.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//���������� ���� ���������� UserContext,
//  ������ ������� ������ �����������
//  (��� ����� ������ ���������� ������� � ������� ������ cmd)
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

//��������� ��������� ��� ���������� ����������� ���� ������
//  � ������� ��������, ����������� � ����� Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UserContext>();

    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
