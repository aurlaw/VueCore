using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VueCore.Data;
using VueCore.Models.Domain;

namespace VueCore.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IDbContextFactory<BlogContext> blogContextFactory, ILogger<DocumentService> logger)
        {
            _blogContextFactory = blogContextFactory;
            _logger = logger;
        }

        public async Task<Document> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _blogContextFactory.CreateDbContext();
            return await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Document> SaveDocumentAsync(string name, string notes, CancellationToken cancellationToken = default)
        {
            var document = new Document
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await using var dbContext = _blogContextFactory.CreateDbContext();
            await dbContext.Documents.AddAsync(document, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return document;
        }

        public async Task UpdateAsync(string id, string name, string notes, string fileUrl, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var existingDoc = await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if(existingDoc != null) 
            {
                existingDoc.Name = name;
                existingDoc.Notes = notes;
                existingDoc.FileUrl = fileUrl;
                existingDoc.UpdatedAt = DateTime.UtcNow;

                dbContext.Update(existingDoc);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateWithFileAsync(string id, string fileUrl, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var existingDoc = await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if(existingDoc != null) 
            {
                existingDoc.FileUrl = fileUrl;
                existingDoc.UpdatedAt = DateTime.UtcNow;

                dbContext.Update(existingDoc);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}