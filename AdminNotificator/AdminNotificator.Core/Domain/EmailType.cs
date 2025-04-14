using System;
using System.Collections.Generic;

namespace AdminNotificator.Core.Domain;

public class EmailType
{
    /// <summary>
    /// Уникальный номер типа рассылки
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Количество дней в компании
    /// </summary>
    public int? ExperianceDays { get; set; }

    /// <summary>
    /// Заголовок письма
    /// </summary>
    public required string EmailTitle { get; set; }

    /// <summary>
    /// Отправлять письма только сотрудникам указанных отделов
    /// </summary>
    public int[]? IntersectDepartmentIds { get; set; }

    /// <summary>
    /// Отправлять письма всем сотркдникам кроме указанных списка отделов
    /// </summary>
    public int[]? ExceptDepartmentIds { get; set; }

    /// <summary>
    /// Отправлять письма только сотрудникам указанных организаций
    /// </summary>
    public string[]? IntersectOrganizationNames { get; set; }

    /// <summary>
    /// Название типа письма
    /// </summary>
    public required string BodyName { get; set; }

    /// <summary>
    /// Отправлять письма только сотрудникам указанных городов
    /// </summary>
    public string[]? IntersectTowns { get; set; }

    /// <summary>
    /// Не отправлять письма сотрудникам указанных городов
    /// </summary>
    public string[]? ExceptTowns { get; set; }


    /// <summary>
    /// Не отправляють после указаной даты
    /// </summary>
    public DateTime? DontSendAfterDate { get; set; }

    /// <summary>
    /// Кол-во дней декретного отпуска, через которое нужно отправить письмо
    /// </summary>
    public int? MaternityDays { get; set; }

    /// <summary>
    /// для какого пола письмо
    /// </summary>
    public string[]? ForGenders { get; set; }

    /// <summary>
    /// Через сколько дней возможна повторная рассылка писем c этим типом
    /// </summary>
    public int? DayCountsForResend { get; set; }

    /// <summary>
    /// Отправлять письма только сотрудникам указанных должностей
    /// </summary>
    public List<string>? IntersectUserPosts { get; set; }

    /// <summary>
    /// Отправлять письма всем сотрудникам кроме указанных должностей
    /// </summary>
    public List<string>? ExceptUserPosts { get; set; }

    public required string SenderEmail { get; set; }

    /// <summary>
    /// Эмайлы тех, кому надо отправить скрытую копию
    /// </summary>
    public string[] Bcc { get; set; } = Array.Empty<string>();
}