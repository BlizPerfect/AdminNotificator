using System;
using AdminNotificator.Core;
using AdminNotificator.Core.Domain;
using AdminNotificator.Core.DTOs;
using AdminNotificator.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<EmailTypeDTO, EmailType>()
        .ForMember(o => o.Id,
            opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
        .ForMember(o => o.IntersectUserPosts,
            opt => opt.Condition(o => o.IntersectUserPosts != null))
        .ForMember(o => o.ExceptUserPosts,
            opt => opt.Condition(o => o.ExceptUserPosts != null))
        .ForMember(o => o.IntersectTowns,
            opt => opt.Condition(o => o.IntersectTowns != null))
        .ForMember(o => o.ExceptTowns,
            opt => opt.Condition(o => o.ExceptTowns != null))
        .ForMember(o => o.IntersectDepartmentIds,
            opt => opt.Condition(o => o.IntersectDepartmentIds != null))
        .ForMember(o => o.ExceptDepartmentIds,
            opt => opt.Condition(o => o.ExceptDepartmentIds != null));

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddDbContext<AdminNotificatorDbContext>();
builder.Services.AddScoped<IRepository<EmailType>, Repository<EmailType>>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();