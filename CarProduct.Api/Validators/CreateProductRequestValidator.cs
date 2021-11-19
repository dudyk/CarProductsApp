using CarProduct.Api.Models;
using FluentValidation;

namespace CarProduct.Api.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.PageCountForScrape)
                .GreaterThan(0)
                .WithMessage("The number of pages for a scrape must be at least one");

            RuleFor(x => x.Zip)
                .Must(x => x.ToString().Length <= 5)
                .When(r => r.Zip.HasValue)
                .WithMessage("Zip length cannot exceed 5 integers");
        }
    }
}
