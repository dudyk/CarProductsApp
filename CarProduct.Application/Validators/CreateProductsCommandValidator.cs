using CarProduct.Application.Commands;
using FluentValidation;

namespace CarProduct.Application.Validators
{
    public class CreateProductsCommandValidator : AbstractValidator<CreateProductsCommand>
    {
        public CreateProductsCommandValidator()
        {
            RuleFor(x => x.PagesCountForScrape)
                .GreaterThan(0)
                .WithMessage("The number of pages for a scrape must be at least one");

            RuleFor(x => x.Zip)
                .Must(x => x.ToString().Length <= 5)
                .When(r => r.Zip.HasValue)
                .WithMessage("Zip length cannot exceed 5 integers");
        }
    }
}
