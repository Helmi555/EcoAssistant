using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.DTOs
{
    public record GroupDto(Guid Id, string Name, string? Description, Guid? IndustryCategoryId)
    {
        public static GroupDto FromEntity(Group group) =>
            new(group.Id, group.Name, group.Description, group.IndustryCategoryId);

        public Group ToEntity() =>
            new Group
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                IndustryCategoryId = this.IndustryCategoryId
            };
    }
}