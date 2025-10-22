
namespace EcoAssistant.API.Dtos
{
    // DTO for creating/updating a UserGroup entry
    public record UserGroupRequestDto(
        Guid UserId,
        Guid GroupId,
        GroupRole Role,
        GroupStatus Status
    );

    public record UserGroupResponseDto(
        Guid UserId,
        Guid GroupId,
        GroupRole Role,
        GroupStatus Status,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}