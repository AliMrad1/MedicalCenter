using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("PatientScheme",o => o.TokenValidationParameters = new (){
            ValidateIssuer = true, 
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "blabla",
            ValidAudience = "blabla", 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefjklmnopqrstuvwxyzsgfdgdgdhggfhfghfghgfjffgfgh")) 
        }
    )
    .AddJwtBearer("DoctorScheme",o => o.TokenValidationParameters = new (){
            ValidateIssuer = true, 
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "blabla",
            ValidAudience = "blabla", 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefjklmnopqrstuvwxyzsgfdgdgdhggfhfghfghgfjffgfgh")) 
        }
    );


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PatientPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add("PatientScheme");
        policy.RequireRole("patient");
    });

    options.AddPolicy("DoctorPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add("DoctorScheme");
        policy.RequireRole("doctor");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
