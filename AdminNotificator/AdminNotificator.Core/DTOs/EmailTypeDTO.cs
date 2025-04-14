using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminNotificator.Core.DTOs;

public record EmailTypeDTO(
    [Range(0, int.MaxValue, ErrorMessage = "Количество дней в компании должно быть ≥ 0")]
    int? ExperianceDays,
    [Required]
    [MaxLength(50, ErrorMessage = "Заголовок письма должен иметь длину не более 50 символов")]
    string EmailTitle,
    int[]? IntersectDepartmentIds,
    int[]? ExceptDepartmentIds,
    string[]? IntersectOrganizationNames,
    [Required]
    [MaxLength(50, ErrorMessage = "Название типа письма должено иметь длину не более 50 символов")]
    string BodyName,
    string[]? IntersectTowns,
    string[]? ExceptTowns,
    DateTime? DontSendAfterDate,
    [Range(0, 140,
        ErrorMessage =
            "Соглсано ст. 255 ТК РФ отпуск по беременности и родам не должен превышать 140 календарных дней")]
    int? MaternityDays,
    [MaxLength(2, ErrorMessage = "Существует только 2 пола")]
    string[]? ForGenders,
    [Range(0, int.MaxValue,
        ErrorMessage = "Повторная рассылка уведомлений возможна только через положительное число дней")]
    int? DayCountsForResend,
    HashSet<string>? IntersectUserPosts,
    HashSet<string>? ExceptUserPosts,
    [Required]
    [MaxLength(50, ErrorMessage = "email отправителя должен иметь длину не более 50 символов")]
    string SenderEmail,
    string[]? Bcc);