namespace Infrastructure.Adapters.Kafka;

public class KafkaTopicsConfiguration
{
    public required string VehicleAddedTopic { get; set; }
    public required string VehicleDeletedTopic { get; set; }
    public required string DrivingLicenseApprovedTopic { get; set; }
    public required string DrivingLicenseExpiredTopic { get; set; }
    public required string BookingCreatedTopic { get; set; }
    public required string BookingCanceledTopic { get; set; }
    public required string VehicleAddingProccessedTopic { get; set; }
}