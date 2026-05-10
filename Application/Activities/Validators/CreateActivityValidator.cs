using System;
using System.Collections.Generic;
using System.Text;
using Application.Activities.Commands;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class CreateActivityValidator : BaseActivityValidator<CreateActivity.Command, CreateActivityEntity>
    {
        public CreateActivityValidator() : base(x => x.Activity)
        {
        }
    }
}
