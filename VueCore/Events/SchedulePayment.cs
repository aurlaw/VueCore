using MediatR;
using VueCore.Models.Domain;

namespace VueCore.Events
{
    /// <summary>
    /// Published when scheduled payment is sent
    /// </summary>
    public record SchedulePayment(Payment Payment, ApplicationUser User) : INotification;
}
