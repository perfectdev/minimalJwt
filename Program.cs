using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalJwt.Models;
using MinimalJwt.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen(setup => {
    var jwtSecurityScheme = new OpenApiSecurityScheme {
        BearerFormat = "JWT",
        Name = "JWT Authentification",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new OpenApiReference {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IMovieService, MovieService>();

var app = builder.Build();
app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (UserLogin userLogin, IUserService service) => Login(userLogin, service));

app.MapGet("/", 
           (IMovieService service) => GetListMovies(service));
app.MapPost("/create",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
            (Movie movie, IMovieService service) => CreateMovie(movie, service));
app.MapGet("/get", (int id, IMovieService service) => GetMovie(id, service));
app.MapPut("/update",
           [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
           (Movie movie, IMovieService service) => UpdateMovie(movie, service));
app.MapDelete("/delete",
              [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
              (int movieId, IMovieService service) => DeleteMovie(movieId, service));

app.UseSwaggerUI();
app.Run();
return;

IResult Login(UserLogin userLogin, IUserService service) {
    var msgUserNotFound = new ErrorResult("400", "User not found!", "ERR0001");
    if (string.IsNullOrWhiteSpace(userLogin.UserName) ||
        string.IsNullOrWhiteSpace(userLogin.Password))
        return Results.BadRequest(msgUserNotFound);
    var loggedInUser = service.Get(userLogin);
    if (loggedInUser is null)
        return Results.BadRequest(msgUserNotFound);
    var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, loggedInUser.UserName),
        new Claim(ClaimTypes.Email, loggedInUser.Email),
        new Claim(ClaimTypes.GivenName, loggedInUser.GivenName),
        new Claim(ClaimTypes.Surname, loggedInUser.Surname),
        new Claim(ClaimTypes.Role, loggedInUser.Role)
    };
    var expires = DateTime.UtcNow.AddDays(int.Parse(builder.Configuration["Jwt:Expires"]!));
    var token = new JwtSecurityToken(
        builder.Configuration["Jwt:Issuer"],
        builder.Configuration["Jwt:Audience"],
        claims,
        expires: expires,
        notBefore: DateTime.UtcNow,
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256
        )
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(tokenString);
}

IResult CreateMovie(Movie movie, IMovieService service) {
    var createdMovie = service.Create(movie);
    return Results.Ok(createdMovie);
}

IResult GetListMovies(IMovieService service) {
    return Results.Ok(service.List());
}

IResult GetMovie(int id, IMovieService service) {
    var foundMovie = service.Get(id);
    if (foundMovie is null)
        return Results.BadRequest(new ErrorResult("400", "Movie not found!", "ERR0002"));
    return Results.Ok(service.Get(id));
}

IResult UpdateMovie(Movie movie, IMovieService service) {
    var updatedMovie = service.Update(movie);
    if (updatedMovie is null)
        Results.BadRequest(new ErrorResult("400", "Movie not found!", "ERR0002"));
    return Results.Ok(updatedMovie);
}

IResult DeleteMovie(int id, IMovieService service) {
    var result = service.Delete(id);
    if (!result)
        Results.BadRequest(new ErrorResult("400", "Something went wrong!", "ERR0003"));
    return Results.Ok(result);
}