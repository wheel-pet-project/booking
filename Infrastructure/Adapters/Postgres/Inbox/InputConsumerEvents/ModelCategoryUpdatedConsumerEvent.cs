using Application.UseCases.Commands.VehicleModel.ChangeVehicleModelCategory;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class ModelCategoryUpdatedConsumerEvent(Guid eventId, Guid modelId, char category) : IConvertibleToCommand
{
    public Guid EventId { get; } = eventId;
    public Guid ModelId { get; } = modelId;
    public char Category { get; } = category;

    public IRequest<Result> ToCommand()
    {
        return new ChangeVehicleModelCategoryCommand(ModelId, Category);
    }
}