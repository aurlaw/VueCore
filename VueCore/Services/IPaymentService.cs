using System;
using System.Threading;
using System.Threading.Tasks;
using VueCore.Models.Domain;

namespace VueCore.Services
{
    public interface IPaymentService
    {
        Task<Payment> GetPaymentAsync(string paymentId, CancellationToken cancellationToken = default);
        Task<Payment> SchedulePaymentAsync(string userId, decimal amount, DateTime scheduleAt, CancellationToken cancellationToken = default);
        Task PaymentCompleteAsync(string paymentId, CancellationToken cancellationToken = default);
    }
}