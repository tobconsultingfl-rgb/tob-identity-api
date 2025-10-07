using FluentValidation;
using Primecoat.Quotes.Domain.Models;

namespace Primecoat.Quotes.Domain.Validation
{
    public class QuoteValidator : AbstractValidator<QuoteBasicInfo>
    {
        public QuoteValidator()
        { 
        }
    }
}
