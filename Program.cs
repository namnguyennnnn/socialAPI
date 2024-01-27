using DoAn4.Data;
using Microsoft.EntityFrameworkCore;
using DoAn4.Services.EmailService;
using DoAn4.Interfaces;
using DoAn4.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.FriendshipService;
using DoAn4.Services.NotificationService;
using DoAn4.Services.VideoService;
using DoAn4.Services.PostService;
using DoAn4.Services.ImageService;
using DoAn4.Services.FileService;
using Microsoft.OpenApi.Models;
using DoAn4.Services.UserService;
using DoAn4.Services.LikeService;
using DoAn4.Services.UserOTPService;
using DoAn4.Services.CommentService;
using DoAn4.Hubs;
using DoAn4.Services.MessageService;
using DoAn4.Services.ConversationService;
using DoAn4.Helper;
using DoAn4.Services.SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
// SignalR
builder.Services.AddSignalR();
// Cors config
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000",
                                                  "https://localhost:7194")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowCredentials();
                          });
});

builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DoAn4Database");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();

builder.Services.AddScoped<INotifyRepository, NotifyRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();


builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IVideoService, VideoService>();


builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ILikeService, LikeService>();

builder.Services.AddScoped<IUserOTPRepository, UserOTPRepository>();
builder.Services.AddScoped<IUserOTPService, UserOTPService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();

builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddTransient<IFileService, FileService>();

//EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

// JWT 
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHub<ChatHub>("/chatHub");

app.UseAuthentication();

app.MapControllers();

app.Run();
