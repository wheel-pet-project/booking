using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.VehicleAggregate.DomainEvents;

public record VehicleAddedDomainEvent : DomainEvent
{
    public VehicleAddedDomainEvent(Guid sagaId, Guid vehicleId)
    {
        if (sagaId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(sagaId)} cannot be null");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        
        SagaId = sagaId;
        VehicleId = vehicleId;
    }
    
    public Guid SagaId { get; }
    public Guid VehicleId { get; }
}