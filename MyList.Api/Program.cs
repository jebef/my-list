using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MyList.Api.Data;
using MyList.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

//--- DI ---//
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


// These request handlers use DI in their method parameters. The framework 
// will check the DI container, retrieve the dependency, and pass it on to 
// the handler. 
//
// (AppDbContext db)

/* 
    Create shows
*/
app.MapPost("/api/shows", async (List<Show> shows, AppDbContext db) =>
{
    db.Shows.AddRange(shows);
    await db.SaveChangesAsync();
    return Results.Created("api/shows", shows);
});

/* 
    Delete a show by ID
*/
app.MapDelete("api/shows/{id}", async (int id, AppDbContext db) =>
{
    var show = await db.Shows.FindAsync(id);
    if (show is null) return Results.NotFound();

    db.Shows.Remove(show);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

/* 
    Get all shows 
*/
app.MapGet("/api/shows", async (AppDbContext db) =>
{
   var shows = await db.Shows.ToListAsync();
   return Results.Ok(shows);
});

/* 
    Get a show by ID 
*/
app.MapGet("/api/shows/{id}", async (int id, AppDbContext db)=>
{
   var show = await db.Shows.FindAsync(id);
   return show is not null ? Results.Ok(show) : Results.NotFound(); 
});


app.Run();