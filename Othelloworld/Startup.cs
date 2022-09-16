using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Othelloworld.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Othelloworld.Data.Repos;
using Microsoft.AspNetCore.Identity;
using Othelloworld.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using Othelloworld.Services;

namespace Othelloworld
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var identityUrl = Configuration.GetValue<string>("IdentityUrl");
			var callBackUrl = Configuration.GetValue<string>("CallBackUrl");
			var sessionCookieLifetime = Configuration.GetValue("SessionCookieLifetimeMinutes", 60);

			services.AddDbContext<OthelloDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("OThelloWorldDatabase")));

			//services.AddAuthentication(options =>
			//{
			//	options.DefaultScheme = IdentityConstants.ApplicationScheme;
			//	options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
			//})
			//	//.AddIdentityCookies();
			//	.AddCookie(options => 
			//		options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
			//	.AddOpenIdConnect(options =>
			//	{
			//		options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//		options.Authority = identityUrl.ToString();
			//		options.SignedOutRedirectUri = callBackUrl.ToString();
			//		options.ClientId = "othelloworld";
			//		options.ClientSecret = "secret";
			//		options.ResponseType = "code id_token";
			//		options.SaveTokens = true;
			//		options.GetClaimsFromUserInfoEndpoint = true;
			//		options.RequireHttpsMetadata = false;
			//		options.UsePkce = true;
			//	});

			//var builder = services.AddIdentityCore<Account>(options => { 
			//	options.Stores.MaxLengthForKeys = 128;
			//	options.User.RequireUniqueEmail = true;
			//});

			//new IdentityBuilder(builder.UserType, typeof(IdentityRole), services)
			//	.AddEntityFrameworkStores<OthelloDbContext>()
			//	.AddDefaultTokenProviders();

			services.AddDefaultIdentity<Account>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<OthelloDbContext>();

			services.AddAuthentication(auth =>
			{
				auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = Configuration.GetValue<string>("Issuer"),
					ValidateAudience = true,
					ValidAudience = Configuration.GetValue<string>("Audience"),
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SigningKey")))
				};
			});

			services
				.AddScoped<IGameRepository, GameRepository>()
				.AddScoped<IGameService, GameService>()
				.AddScoped<IPlayerRepository, PlayerRepository>()
				.AddScoped<IAccountService, AccountService>();

			services.AddControllers()
				.AddJsonOptions(x => {
					x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
					x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				});

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Othelloworld API",
					Description = "API Documentation for Othelloworld application",
					TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "Othello Contact",
						Url = new Uri("https://example.com/contact")
					},
					License = new OpenApiLicense
					{
						Name = "World License",
						Url = new Uri("https://example.com/license")
					}
				});
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header
						},
						new List<string>()
					}
				});

				// using System.Reflection;
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			});

			services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();

				endpoints.MapControllerRoute(
									name: "default",
									pattern: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseReactDevelopmentServer(npmScript: "start");
				}
			});
		}
	}
}
