namespace EcoAssistant.API.Dtos
{
    public record GroupRequestDto(
        int DeviceId,
        Guid GroupId,
        GroupRole Role,
        GroupStatus Status,
        string DeviceName
    );

    public record GroupResponseDto(
        int DeviceId,
        Guid GroupId,
        GroupRole Role,
        GroupStatus Status,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}