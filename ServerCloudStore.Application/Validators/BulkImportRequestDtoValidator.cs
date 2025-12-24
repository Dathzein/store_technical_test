using FluentValidation;
using ServerCloudStore.Application.DTOs.BulkImport;

namespace ServerCloudStore.Application.Validators;

/// <summary>
/// Validador para BulkImportRequestDto
/// </summary>
public class BulkImportRequestDtoValidator : AbstractValidator<BulkImportRequestDto>
{
    public BulkImportRequestDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => x.GenerateCount.HasValue || x.CsvStream != null)
            .WithMessage("Debe proporcionar GenerateCount o CsvStream");

        RuleFor(x => x.GenerateCount)
            .GreaterThan(0)
            .LessThanOrEqualTo(100000)
            .WithMessage("GenerateCount debe estar entre 1 y 100,000")
            .When(x => x.GenerateCount.HasValue);

        RuleFor(x => x.CsvStream)
            .Must(stream => stream == null || stream.CanRead)
            .WithMessage("El stream CSV debe ser legible")
            .When(x => x.CsvStream != null);
    }
}

