using Application.Activities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Validators
{
    public class BaseActivityValidator<T, TDto> : AbstractValidator<T> 
        where TDto : BaseActivityEntity
    {
        public BaseActivityValidator(Func<T, TDto> getActivity)
        {
            RuleFor(x => getActivity(x).Title).NotEmpty().WithMessage("Title is required.").MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
            RuleFor(x => getActivity(x).Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => getActivity(x).Category).NotEmpty().WithMessage("Category is required.");
            RuleFor(x => getActivity(x).Date).NotEmpty().WithMessage("Date is required.").GreaterThan(DateTime.Now).WithMessage("Date must be in the future.");
            RuleFor(x => getActivity(x).City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => getActivity(x).Venue).NotEmpty().WithMessage("Venue is required.");
            RuleFor(x => getActivity(x).Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
            RuleFor(x => getActivity(x).Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        }
    }
}
