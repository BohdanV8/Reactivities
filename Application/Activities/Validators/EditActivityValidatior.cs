using Application.Activities.Commands;
using Application.Activities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Validators
{
    public class EditActivityValidator : BaseActivityValidator<EditActivity.Command, EditActivityEntity>
    {
        public EditActivityValidator() : base(x => x.activity)
        {
            RuleFor(x => x.activity.Id).NotEmpty().WithMessage("Activity ID is required.");
        }
    }
}
