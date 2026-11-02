using FluentValidation;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Validators
{
    /// <summary>
    /// Validator for Profile DTO
    /// Example of validating user profile information
    /// </summary>
    public class ProfileDtoValidator : AbstractValidator<ProfileDto>
    {
        public ProfileDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

            RuleFor(x => x.Bio)
                .MaximumLength(300).WithMessage("Bio cannot exceed 300 characters");

            RuleFor(x => x.PreferredLevel)
                .NotEmpty().WithMessage("Preferred level is required")
                .Must(x => x == "Beginner" || x == "Intermediate" || x == "Advanced")
                .WithMessage("Preferred level must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.Skills)
                .NotNull().WithMessage("Skills list is required")
                .Must(x => x.Count <= 20).WithMessage("Cannot have more than 20 skills");
        }
    }
}
