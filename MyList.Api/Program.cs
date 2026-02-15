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

// app.UseHttpsRedirection();


// These request handlers use DI in their method parameters. The framework 
// will check the DI container, retrieve the dependency, and pass it on to 
// the handler. 
//
// (AppDbContext db)

//--- USERS ---//

/* 
    Create a User 
*/
app.MapPost("/api/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", user);

});

/* 
    Get all users 
*/
app.MapGet("/api/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

/* 
    Get a User by ID
*/
app.MapGet("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();

});

/* 
    Delete a User by ID
*/
app.MapDelete("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//--- SHOWS ---// 

/* 
    Create Shows
*/
app.MapPost("/api/shows", async (List<Show> shows, AppDbContext db) =>
{
    int added = 0;
    int updated = 0;

    // load existing shows 
    var existingShows = await db.Shows.ToListAsync();

    foreach (var show in shows)
    {
        // check if show exists in db 
        var existing = existingShows.FirstOrDefault(s =>
            s.Date == show.Date && s.Venue == show.Venue);

        // new show, insert 
        if (existing is null)
        {
            db.Shows.Add(show);
            added++;
        }
        // exists, update
        else
        {
            existing.Artists = show.Artists;
            existing.DoorsTime = show.DoorsTime;
            existing.StartTimes = show.StartTimes;
            existing.EndTime = show.EndTime;
            existing.City = show.City;
            existing.Price = show.Price;
            existing.Recommended = show.Recommended;
            existing.WillSellOut = show.WillSellOut;
            existing.U21DrinkTix = show.U21DrinkTix;
            existing.AllAges = show.AllAges;
            existing.AgeMeta = show.AgeMeta;
            existing.PitWarning = show.PitWarning;
            existing.NoInsOuts = show.NoInsOuts;
            existing.SoldOut = show.SoldOut;
            updated++;
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { added, updated });
});

/* 
    Delete a show by ID
*/
app.MapDelete("/api/shows/{id}", async (int id, AppDbContext db) =>
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
app.MapGet("/api/shows/{id}", async (int id, AppDbContext db) =>
{
    var show = await db.Shows.FindAsync(id);
    return show is not null ? Results.Ok(show) : Results.NotFound();
});


app.Run();
