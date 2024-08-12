using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Unator.App.Data;
using Unator.App.Services;
using Unator.Extensions;
using Unator.Extensions.Routing;
using Unator.Templating;

namespace Unator.App.Handlers;

public class EmailHandlers
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet(SendEmailRoute.Pattern, SendEmailHandler);

        builder.MapGet(BroadcastRoute.Pattern, BroadcastGet);
        builder.MapPost(BroadcastRoute.Pattern, BroadcastPost).DisableAntiforgery();

        builder.MapGet(ContactRoute.Pattern, ContactsGet);
        builder.MapPost(ContactRoute.Pattern, ContactsPost).DisableAntiforgery();
    }

    public static readonly UnatorRoute SendEmailRoute = UnatorRoute.New("email");

    public static async Task SendEmailHandler(HttpContext ctx)
    {
        var html = Componnents.BaseLayout("Email", "Creation of email page").Wrap(UI.Form.Class("grid").Wrap(
            UI.Div.Wrap(
                   UI.Label[
                       "Email preset",
                       UI.Select.Wrap(
                           UI.Option.Flag("selected").Attr("value", "")["Use new email sender"],
                           UI.Option["Roman Koshchei - roman@flurium.com"],
                           UI.Option["Flurium - roman@flurium.com"]
                       ),
                       UI.Small["If you select preset then it's prioritized over inputs"]
                   ],
                   UI.Div.Class("grid")[
                       UI.Label["Your name", UI.Input.Placeholder("Roman Koshchei")],
                       UI.Label["Your email", UI.Input.Placeholder("email@example.com")]
                   ],
                   UI.P.Wrap(UI.Label.Wrap(UI.Input.Attr("type", "checkbox"), "Save sender?")),
                   UI.Label.Wrap("Subject", UI.Input.Placeholder("Story about ...")),
                   UI.Button["Send"]
               ),
               UI.Div.Wrap(
                   UI.Label.Wrap("Markdown content", UI.Textarea.Attr("rows", "15").Placeholder(
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

    public static readonly UnatorRoute ContactRoute = UnatorRoute.New("contacts");

    public static async Task ContactsGet(
       HttpContext ctx,
       [FromServices] UnatorDb db,
       [FromQuery] string? email
    )
    {
        IQueryable<DbContact> query = db.Contacts;
        if (email != null) { query = query.Where(x => x.Email.StartsWith(email)); }

        var contacts = await query.Take(50).AsNoTracking().ToListAsync();
        var html = Componnents.BaseLayout("Contacts").Wrap(
            UI.Hgroup[
                UI.H1["Contacts"],
                UI.H2["List of all emails that are or were subscribed"]
            ],
            UI.Form.Attr("method", "POST").Attr("action", ContactRoute.Url())[
                UI.Label["Add new contact"],
                UI.Fieldset.Role("group")[
                    UI.Input.Name(nameof(ContactForm.Email)).Type("email").Placeholder("Email of Contact"),
                    UI.Button["Add"]
                ]
            ],
            UI.Form.Attr("action", ContactRoute.Url()).Role("search")[
                UI.Input.Name("email").Type("search").Placeholder("Search email").Attr("value", email ?? ""),
                UI.Button["Search"]
            ],
            UI.Div.Class("overflow-auto")[
                UI.Table.Class("striped")[
                    UI.Thead[
                        UI.Tr[
                            UI.Th["Email"],
                            UI.Th["Subscribed"],
                            UI.Th["Created"]
                        //UI.Th["Id"]
                        ]
                    ],
                    UI.Tbody[UI.Many(
                        contacts.Select(x => UI.Tr[
                            UI.Th[x.Email],
                            UI.Td[x.Subscribed ? "True" : "False"],
                            UI.Td[x.Created.ToString("f")]
                        //UI.Th[x.Id]
                        ])
                    )]
                ]
            ]
        );
        await ctx.Response.WriteAsync(html);
    }

    public class ContactForm
    {
        [Required, EmailAddress, MinLength(1)]
        public string Email { get; } = string.Empty;
    }

    public static async Task ContactsPost(
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
        ctx.Response.Redirect(ContactRoute.Url());
    }
}