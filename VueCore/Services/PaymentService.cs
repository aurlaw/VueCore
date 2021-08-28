using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VueCore.Data;
using VueCore.Models.Domain;

namespace VueCore.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IDbContextFactory<BlogContext> blogContextFactory, ILogger<PaymentService> logger)
        {
            _blogContextFactory = blogContextFactory;
            _logger = logger;
        }

        public async Task PaymentCompleteAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var existingPayment = await dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
            if(existingPayment != null)
            {
                existingPayment.Status = "Complete";
                existingPayment.UpdatedAt = DateTime.UtcNow;

                dbContext.Update(existingPayment);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Payment> SchedulePaymentAsync(string userId, decimal amount, DateTime scheduleAt, CancellationToken cancellationToken = default)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString("N"),
                UserId = userId,
                Amount = amount,
                Status = "Scheduled",
                ScheduledAt = scheduleAt,
                CreatedAt  = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await using var dbContext = _blogContextFactory.CreateDbContext();
            await dbContext.Payments.AddAsync(payment, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return payment;
        }
    }
}
