using BackpackProblem.WebApi.Models;
using FluentValidation;

namespace BackpackProblem.WebApi.Validators
{
    public class GetRandomModelValidator : AbstractValidator<GetRandomModel>
    {
        public GetRandomModelValidator()
        {
            RuleFor(r => r.ContainerHeight).GreaterThan(0);
            RuleFor(r => r.ContainerWidth).GreaterThan(0);
            RuleFor(r => r.MaxItemHeight).GreaterThan(0);
            RuleFor(r => r.MaxItemWidth).GreaterThan(0);
            RuleFor(r => r.NumberOfItems).GreaterThan(0).LessThanOrEqualTo(20);
        }
    }
}
