using System.Net;
using Amp.Data.Nido;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;

namespace Amp.Api.Nido;

/// <summary>
/// Sends booking emails via Azure Communication Services: a notification to the business and a
/// confirmation to the customer. Best-effort — failures are logged, never thrown, so a mail
/// problem can't break a booking. No-ops when ACS isn't configured.
/// </summary>
public class BookingEmailService
{
    private readonly EmailClient? _client;
    private readonly AcsConfig _cfg;
    private readonly ILogger<BookingEmailService> _log;

    public BookingEmailService(AcsConfig cfg, ILogger<BookingEmailService> log)
    {
        _cfg = cfg;
        _log = log;
        if (!string.IsNullOrWhiteSpace(cfg.ConnectionString))
            _client = new EmailClient(cfg.ConnectionString);
    }

    private bool Enabled => _client is not null && !string.IsNullOrWhiteSpace(_cfg.SenderAddress);

    public async Task SendBookingEmailsAsync(NidoAppointment a)
    {
        if (!Enabled)
        {
            _log.LogInformation("ACS not configured — skipping booking emails.");
            return;
        }

        var prettyDate = FormatDate(a.Date);

        if (!string.IsNullOrWhiteSpace(_cfg.BusinessEmail))
        {
            await TrySend(
                _cfg.BusinessEmail,
                $"Nieuwe afspraakaanvraag — {a.Name} ({prettyDate} {a.Time})",
                BusinessHtml(a, prettyDate),
                BusinessText(a, prettyDate),
                replyTo: a.Email);
        }

        if (!string.IsNullOrWhiteSpace(a.Email))
        {
            await TrySend(
                a.Email,
                "Je afspraakaanvraag bij Nido Suave",
                CustomerHtml(a, prettyDate),
                CustomerText(a, prettyDate));
        }
    }

    private async Task TrySend(string to, string subject, string html, string text, string? replyTo = null)
    {
        try
        {
            var content = new EmailContent(subject) { Html = html, PlainText = text };
            var message = new EmailMessage(_cfg.SenderAddress, to, content);
            if (!string.IsNullOrWhiteSpace(replyTo))
                message.ReplyTo.Add(new EmailAddress(replyTo));

            await _client!.SendAsync(WaitUntil.Started, message);
            _log.LogInformation("Queued booking email to {Recipient}.", to);
        }
        catch (RequestFailedException ex)
        {
            _log.LogError(ex, "Failed to send booking email to {Recipient}.", to);
        }
    }

    private static string FormatDate(string isoDate) =>
        DateTime.TryParse(isoDate, out var d)
            ? d.ToString("dddd d MMMM yyyy", new System.Globalization.CultureInfo("nl-NL"))
            : isoDate;

    private static string Esc(string s) => WebUtility.HtmlEncode(s ?? string.Empty);

    private static string BusinessHtml(NidoAppointment a, string prettyDate) => $@"
<h2>Nieuwe afspraakaanvraag</h2>
<p>Er is een nieuwe aanvraag binnengekomen via de website.</p>
<table cellpadding=""6"" style=""border-collapse:collapse"">
  <tr><td><strong>Behandeling</strong></td><td>{Esc(a.Service)}</td></tr>
  <tr><td><strong>Datum</strong></td><td>{Esc(prettyDate)}</td></tr>
  <tr><td><strong>Tijd</strong></td><td>{Esc(a.Time)}</td></tr>
  <tr><td><strong>Naam</strong></td><td>{Esc(a.Name)}</td></tr>
  <tr><td><strong>E-mail</strong></td><td>{Esc(a.Email)}</td></tr>
  <tr><td><strong>Telefoon</strong></td><td>{Esc(a.Phone)}</td></tr>
  <tr><td><strong>Bericht</strong></td><td>{Esc(a.Notes)}</td></tr>
</table>
<p>Neem contact op met de klant om te bevestigen.</p>";

    private static string BusinessText(NidoAppointment a, string prettyDate) =>
        $"Nieuwe afspraakaanvraag\n\nBehandeling: {a.Service}\nDatum: {prettyDate}\nTijd: {a.Time}\n" +
        $"Naam: {a.Name}\nE-mail: {a.Email}\nTelefoon: {a.Phone}\nBericht: {a.Notes}\n";

    private static string CustomerHtml(NidoAppointment a, string prettyDate) => $@"
<p>Hoi {Esc(a.Name)},</p>
<p>Bedankt voor je aanvraag bij <strong>Nido Suave</strong>. We hebben het volgende ontvangen:</p>
<table cellpadding=""6"" style=""border-collapse:collapse"">
  <tr><td><strong>Behandeling</strong></td><td>{Esc(a.Service)}</td></tr>
  <tr><td><strong>Datum</strong></td><td>{Esc(prettyDate)}</td></tr>
  <tr><td><strong>Tijd</strong></td><td>{Esc(a.Time)}</td></tr>
</table>
<p>Dit is nog een aanvraag — Denise neemt persoonlijk contact met je op om de afspraak te bevestigen.</p>
<p>Warme groet,<br/>Nido Suave</p>";

    private static string CustomerText(NidoAppointment a, string prettyDate) =>
        $"Hoi {a.Name},\n\nBedankt voor je aanvraag bij Nido Suave.\n\n" +
        $"Behandeling: {a.Service}\nDatum: {prettyDate}\nTijd: {a.Time}\n\n" +
        "Dit is nog een aanvraag — Denise neemt persoonlijk contact met je op om te bevestigen.\n\n" +
        "Warme groet,\nNido Suave\n";
}
