using Application.UseCases.Commands.Booking.BookVehicle;
using Application.UseCases.Commands.Booking.CancelVehicleBooking;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.ArgumentException;
using FluentResults;
using Grpc.Core;
using MediatR;

namespace Api.Adapters.Grpc;

public class BookingV1(IMediator mediator) : Booking.BookingBase
{
    public override async Task<BookVehicleResponse> BookVehicle(BookVehicleRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new BookVehicleCommand(
            ParseGuidOrThrow(request.VehicleId),
            ParseGuidOrThrow(request.CustomerId)));

        return response.IsSuccess
            ? new BookVehicleResponse()
            : ParseErrorToRpcException<BookVehicleResponse>(response.Errors);
    }

    public override async Task<CancelBookingResponse> CancelBookingVehicle(CancelBookingRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new CancelVehicleBookingCommand(ParseGuidOrThrow(request.BookingId)));
        
        return response.IsSuccess
            ? new CancelBookingResponse()
            : ParseErrorToRpcException<CancelBookingResponse>(response.Errors);
    }
    
    private T ParseErrorToRpcException<T>(List<IError> errors)
    {
        if (errors.Exists(x => x is NotFound))
            throw new RpcException(new Status(StatusCode.NotFound, string.Join(' ', errors.Select(x => x.Message))));

        if (errors.Exists(x => x is CommitFail))
            throw new RpcException(new Status(StatusCode.Unavailable, string.Join(' ', errors.Select(x => x.Message))));
        

        throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(' ', errors.Select(x => x.Message))));
    }

    private Guid ParseGuidOrThrow(string potentialId)
    {
        return Guid.TryParse(potentialId, out var id)
            ? id
            : throw new ValueOutOfRangeException($"{nameof(potentialId)} is invalid uuid");
    }
}