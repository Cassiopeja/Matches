using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Matches.Core;
using Matches.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Matches.Services
{
    public class TemplateLoadService : IHostedService
    {
        private const string TemplatesDirectoryName = "templates";
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TemplateLoadService(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment)
        {
            _serviceProvider = serviceProvider;
            _webHostEnvironment = webHostEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var templates = GetGardTemplates();
            if (templates.Count == 0) return Task.CompletedTask;

            using var scope = _serviceProvider.CreateScope();
            var gameManager = scope.ServiceProvider.GetRequiredService<IGameManager>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TemplateLoadService>>();
            foreach (var cardTemplate in templates)
            {
                var result = gameManager.AddCardTemplate(cardTemplate);
                if (result.IsFailure) logger.LogError("Error during adding card template: {Error}", result.Error);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private List<CardTemplate> GetGardTemplates()
        {
            var templates = new List<CardTemplate>();
            var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, TemplatesDirectoryName);
            if (!Directory.Exists(templatePath)) return templates;
            var directories = Directory.GetDirectories(templatePath);
            foreach (var directory in directories)
            {
                var name = new FileInfo(directory).Name;

                var backs = Directory.GetFiles(directory, "back.*");
                if (backs.Length <= 0) continue;
                var backFileName = Path.GetFileName(backs[0]);

                var frontCardImages = Directory.GetFiles(directory).Select(
                        Path.GetFileName).Where(file => !file.Equals(backFileName))
                    .Select(file => GetPathToTemplateImage(name, file));
                templates.Add(new CardTemplate(name, frontCardImages,
                    GetPathToTemplateImage(name, backFileName)));
            }

            return templates;
        }

        private string GetPathToTemplateImage(string templateName, string fileName)
        {
            return Path.Combine(TemplatesDirectoryName, templateName, fileName);
        }
    }
}