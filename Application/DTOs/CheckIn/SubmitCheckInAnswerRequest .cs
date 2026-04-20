using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.CheckIn;

public class SubmitCheckInAnswerRequest
{
    public string Content { get; set; } = string.Empty;
}
