﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.EventSourcing.CRM.API.Services;
using CQRS.EventSourcing.CRM.Application.Customers;
using CQRS.EventSourcing.CRM.Application.Infrastructure.Mapper.Profiles;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Persistence;
using CQRS.EventSourcing.CRM.Persistence.EventStore;
using Dapper.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CQRS.EventSourcing.CRM.API
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
            // Add AutoMapper
            services.AddAutoMapper(typeof(CustomerProfile).Assembly);

            //Add MediatR
            services.AddMediatR(typeof(CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer.CreateCustomerCommand.Handler).GetTypeInfo().Assembly);

            // Add DbContext using SQL Server Provider
            services.AddDbContext<CRMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CRMDatabase")));
            services.AddDbContext<ICRMDbContext, CRMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CRMDatabase")));

            // Add IEventStore
            services.AddSingleton<IDbExecutorFactory>(x =>
                new SqlExecutorFactory(Configuration.GetConnectionString("CRMDatabase")));
            services.AddTransient<IEventStore, DbEventStore>();

            // Add Customer Snapshotting Background Service
            services.AddTransient<CustomerSnapshotter>();
            services.AddHostedService<TimedCustomerSnapshottingService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseMvcWithDefaultRoute();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
