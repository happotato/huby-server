using System;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Huby.Data;

namespace Huby
{
    public sealed class DateTimeConverter : JsonConverter<DateTime>
    {
        public string Format { get; set; } = "r";

        public override DateTime Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions opt)
        {
            return DateTime.Parse(r.GetString());
        }

        public override void Write(Utf8JsonWriter w, DateTime dt, JsonSerializerOptions opt)
        {
            w.WriteStringValue(dt.ToString(Format));
        }
    }

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(opt => {
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });

            services.AddDbContext<ApplicationDatabase>(builder => {
                builder.UseNpgsql(Configuration["PostgreSQL:ConnectionString"]);
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddIdentityCore<User>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
            }).AddUserStore<ApplicationUserStore>();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["JWT:Key"]);

                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = "Issuer",
                    ValidAudience = "Audience",
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                };
            });

            {
                var key = Encoding.UTF8.GetBytes(Configuration["Crypto:Key"]);
                var iv = Encoding.UTF8.GetBytes(Configuration["Crypto:IV"]);

                var alg = new RijndaelManaged();
                alg.Padding = PaddingMode.PKCS7;

                var encryptor = alg.CreateEncryptor(key, iv);
                var decryptor = alg.CreateDecryptor(key, iv);

                services.AddSingleton<ISimpleCrypto>(new SimpleCrypto(encryptor, decryptor));
            }

            {
                var region = Configuration["S3:Region"];
                var accessKey = Configuration["S3:AccessKey"];
                var secretKey = Configuration["S3:SecretKey"];
                var serviceUrl =  Configuration["S3:ServiceUrl"];
                var imageBucketName = Configuration["S3:ImageBucketName"];

                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                    ServiceURL = serviceUrl,
                    ForcePathStyle = true,
                    UseHttp = Env.IsDevelopment()
                };

                services.AddSingleton(new AmazonS3Client(accessKey, secretKey, config));
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
            });

            app.UseRouting();
            app.UseResponseCompression();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
