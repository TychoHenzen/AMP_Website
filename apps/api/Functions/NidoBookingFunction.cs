using System.Text.Json;
using System.Text.RegularExpressions;
using Amp.Api.Nido;
using Amp.Data.Nido;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Amp.Api.Functions;

/// <summary>
/// Nido Suave appointment booking. Public (anonymous) endpoints consumed by the WASM site:
///   GET  /api/nido/availability?date=yyyy-MM-dd   -> slots for that day
///   POST /api/nido/appointments                   -> create a booking
/// </summary>
public class NidoBookingFunction
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);
    private static readonly Regex EmailRx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly INidoAppointmentRepository _repo;
    private readonly BookingEmailService _email;

    public NidoBookingFunction(INidoAppointmentRepository repo, BookingEmailService email)
    {
        _repo = repo;
        _email = email;
    }

    [Function("nido-availability")]
    public async Task<IActionResult> Availability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nido/availability")] HttpRequest req)
    {
        var dateStr = req.Query["date"].ToString();
        if (!DateOnly.TryParseExact(dateStr, "yyyy-MM-dd", out var date))
            return new BadRequestObjectResult(new { error = "Geef een geldige datum op (yyyy-MM-dd)." });

        var nlNow = NidoSchedule.NlNow();
        var today = DateOnly.FromDateTime(nlNow);

        if (date < today)
            return new OkObjectResult(new AvailabilityResponse(dateStr, false, true, Array.Empty<SlotDto>()));

        if (!NidoSchedule.IsOpen(date))
            return new OkObjectResult(new AvailabilityResponse(dateStr, false, false, Array.Empty<SlotDto>()));

        var booked = (await _repo.GetByDateAsync(dateStr)).Select(a => a.Time).ToHashSet();
        var nowTime = TimeOnly.FromDateTime(nlNow);

        var slots = NidoSchedule.Slots(date).Select(t =>
        {
            var available = !booked.Contains(t);
            if (available && date == today && TimeOnly.TryParse(t, out var to) && to <= nowTime)
                available = false; // past time today
            return new SlotDto(t, available);
        }).ToArray();

        return new OkObjectResult(new AvailabilityResponse(dateStr, true, false, slots));
    }

    [Function("nido-create-appointment")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nido/appointments")] HttpRequest req)
    {
        BookingRequest? body;
        try
        {
            body = await JsonSerializer.DeserializeAsync<BookingRequest>(req.Body, JsonOpts);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { error = "Ongeldige aanvraag." });
        }

        if (body is null)
            return new BadRequestObjectResult(new { error = "Ongeldige aanvraag." });

        var errors = Validate(body, out var date);
        if (errors.Count > 0)
            return new BadRequestObjectResult(new { errors });

        if (!NidoSchedule.Slots(date).Contains(body.Time))
            return new BadRequestObjectResult(new { error = "Gekozen tijd is niet beschikbaar." });

        var booked = (await _repo.GetByDateAsync(body.Date!)).Select(a => a.Time).ToHashSet();
        if (booked.Contains(body.Time!))
            return new ConflictObjectResult(new { error = "Dit tijdslot is net bezet. Kies een ander tijdstip." });

        var created = await _repo.CreateAsync(new NidoAppointment
        {
            Name = body.Name!.Trim(),
            Email = body.Email!.Trim(),
            Phone = body.Phone!.Trim(),
            Service = body.Service!.Trim(),
            Date = body.Date!,
            Time = body.Time!,
            Notes = (body.Notes ?? string.Empty).Trim(),
            Status = "pending"
        });

        await _email.SendBookingEmailsAsync(created); // best-effort; never throws

        return new ObjectResult(new BookingResponse(created.Id, created.Status, created.Date, created.Time))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    /// <summary>
    /// Admin: list upcoming (today onward) bookings. Protected by a Functions key
    /// (AuthorizationLevel.Function) — pass ?code=&lt;key&gt;.
    /// </summary>
    [Function("nido-list-appointments")]
    public async Task<IActionResult> List(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "nido/appointments")] HttpRequest req)
    {
        var fromDate = DateOnly.FromDateTime(NidoSchedule.NlNow()).ToString("yyyy-MM-dd");
        var items = await _repo.GetUpcomingAsync(fromDate);
        var result = items.Select(a => new
        {
            id = a.Id,
            date = a.Date,
            time = a.Time,
            name = a.Name,
            email = a.Email,
            phone = a.Phone,
            service = a.Service,
            notes = a.Notes,
            status = a.Status
        });
        return new OkObjectResult(result);
    }

    private static List<string> Validate(BookingRequest b, out DateOnly date)
    {
        var errors = new List<string>();
        date = default;

        if (string.IsNullOrWhiteSpace(b.Name)) errors.Add("Naam is verplicht.");
        if (string.IsNullOrWhiteSpace(b.Email) || !EmailRx.IsMatch(b.Email!)) errors.Add("Geldig e-mailadres is verplicht.");
        if (string.IsNullOrWhiteSpace(b.Phone)) errors.Add("Telefoonnummer is verplicht.");
        if (string.IsNullOrWhiteSpace(b.Service)) errors.Add("Kies een behandeling.");
        if (string.IsNullOrWhiteSpace(b.Time)) errors.Add("Kies een tijdstip.");

        if (!DateOnly.TryParseExact(b.Date, "yyyy-MM-dd", out date))
        {
            errors.Add("Geef een geldige datum op.");
            return errors;
        }

        var today = DateOnly.FromDateTime(NidoSchedule.NlNow());
        if (date < today) errors.Add("Datum ligt in het verleden.");
        else if (!NidoSchedule.IsOpen(date)) errors.Add("Op deze dag zijn er geen behandelingen.");

        return errors;
    }
}
