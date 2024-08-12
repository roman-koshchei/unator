using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Unator.App.Data;
using Unator.App.Services;
using Unator.Extensions;
using Unator.Extensions.Routing;
using Unator.Templating;
using static Unator.Templating.UI;

namespace Unator.App.Handlers;

public class EmailHandlers
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet(SendEmailRoute.Pattern, SendEmailHandler);

        builder.MapGet(BroadcastRoute.Pattern, BroadcastGet);
        builder.MapPost(BroadcastRoute.Pattern, BroadcastPost).DisableAntiforgery();

        builder.MapPost(ContactRoute.Pattern, ContactPost).DisableAntiforgery();
    }

    public static readonly UnatorRoute SendEmailRoute = UnatorRoute.New("email");

    public static async Task SendEmailHandler(HttpContext ctx)
    {
        var html = Componnents.BaseLayout("Email", "Creation of email page").Wrap(Form.Class("grid").Wrap(
            Div.Wrap(
                   Label[
                       "Email preset",
                       Select.Wrap(
                           Option.Flag("selected").Attr("value", "")["Use new email sender"],
                           Option["Roman Koshchei - roman@flurium.com"],
                           Option["Flurium - roman@flurium.com"]
                       ),
                       Small["If you select preset then it's prioritized over inputs"]
                   ],
                   Div.Class("grid")[
                       Label["Your name", Input.Placeholder("Roman Koshchei")],
                       Label["Your email", Input.Placeholder("email@example.com")]
                   ],
                   P.Wrap(Label.Wrap(Input.Attr("type", "checkbox"), "Save sender?")),
                   Label.Wrap("Subject", Input.Placeholder("Story about ...")),
                   Button["Send"]
               ),
               Div.Wrap(
                   Label.Wrap("Markdown content", Textarea.Attr("rows", "15").Placeholder(
                       "# Heading 1 \n\n Some content of paragraph \n\n - list item 1"
                   ))
               )
        ));

        await ctx.Response.WriteAsync(html);
    }

    public static readonly UnatorRoute BroadcastRoute = UnatorRoute.New("broadcast");

    public static async Task BroadcastGet(HttpContext ctx)
    {
        var html = Componnents.BaseLayout("Broadcast").Wrap(
            UI.H1["Broadcast"],
            UI.Form.Attr("method", "POST").Attr("action", ContactRoute.Url())[
                UI.Label["Contact"],
                UI.Fieldset.Role("group")[
                    UI.Input.Name(nameof(ContactForm.Email)).Type("email").Placeholder("Email of Contact"),
                    UI.Button["Add"]
                ]
            ],
            UI.Form.Attr("method", "POST").Attr("action", BroadcastRoute.Url())[
                UI.Label[
                    "Subject",
                    UI.Input.Name(nameof(BroadcastForm.Subject)).Placeholder("Subject")
                ],
                UI.Button["Send"]
            ]
        );
        await ctx.Response.WriteAsync(html);
    }

    public class BroadcastForm
    {
        [Required, MinLength(1)]
        public string Subject { get; set; } = string.Empty;
    }

    public static async Task BroadcastPost(
        HttpContext ctx,
        [FromServices] UnatorDb db,
        [FromForm] BroadcastForm form
    )
    {
        try
        {
            var dbEmail = new DbEmail
            {
                FromName = "Roman",
                FromEmail = "roman@flurium.com",
                Subject = form.Subject,
                Content = form.Subject
            };
            await db.Emails.AddAsync(dbEmail);

            var contacts = await db.Contacts
                .Where(x => x.Subscribed)
                .Select(x => new { x.Id, x.Email })
                .ToListAsync();

            // can be replaced with array, because we know size
            var receivers = new Queue<Receiver>(contacts.Count);
            foreach (var contact in contacts)
            {
                var receiver = new DbReceiver() { EmailId = dbEmail.Id, ContactId = contact.Id };
                receivers.Enqueue(new Receiver(contact.Email, contact.Id));
                await db.Receivers.AddAsync(receiver);
            }

            await db.SaveChangesAsync();

            EmailWorker.Push(dbEmail, receivers);
        }
        catch
        {
            // Show form one more time with error
        }

        ctx.Response.Redirect(BroadcastRoute.Url());
    }

    public static readonly UnatorRoute ContactRoute = BroadcastRoute.Add("contact");

    public class ContactForm
    {
        [Required, EmailAddress, MinLength(1)]
        public string Email { get; } = string.Empty;
    }

    public static async Task ContactPost(
       HttpContext ctx,
       [FromServices] UnatorDb db,
       [FromForm] ContactForm form
    )
    {
        if (form.IsValid())
        {
            await db.Contacts.AddAsync(new() { Email = form.Email });
            await db.SaveChangesAsync();
        }
    }
}