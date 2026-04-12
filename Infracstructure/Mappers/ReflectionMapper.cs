using Application.DTOs.Reflection;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Mappers;

public static class ReflectionMapper
{
    public static ReflectionDto ToDto(this Reflection reflection)
    {
        return new ReflectionDto
        {
            Id = reflection.Id,
            UserId = reflection.UserId,
            RoomId = reflection.RoomId,
            EmotionEntryId = reflection.EmotionEntryId,
            MoodAfter = reflection.MoodAfter,
            HelpfulScore = reflection.HelpfulScore,
            Content = reflection.Content,
            CreatedAt = reflection.CreatedAt
        };
    }

    public static List<ReflectionDto> ToDtoList(this List<Reflection> reflections)
    {
        var dtos = new List<ReflectionDto>();
        foreach (var reflection in reflections)
        {
            dtos.Add(reflection.ToDto());
        }
        return dtos;
    }
}
