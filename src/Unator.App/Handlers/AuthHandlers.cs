using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Unator.App.Data;
using Unator.App.Services;
using Unator.Extensions;
using Unator.Extensions.Routing;
using Unator.Templating;

namespace Unator.App.Handlers;

public class AuthFormBody
{
    [Required, EmailAddress, MinLength(1)]
    public required string Email { get; set; }

    [Required, MinLength(1)]
    public required string Password { get; set; }
}

public class AuthHandlers
{
    public static readonly UnatorRoute AuthRoute = UnatorRoute.New("auth");

    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet(AuthRoute.Pattern, async (HttpResponse res) =>
        {
            res.StatusCode = 200;
            //var wave = Wave.Html(res, StatusCodes.Status200OK);
            //await GetStartedPage(wave, null, null, null);
        });

        //builder.MapPost(GetStartedRoute.Pattern, GetStartedHandler).DisableAntiforgery();
        builder.MapGet(LogoutRoute.Pattern, Logout).RequireAuthorization().DisableAntiforgery();
    }

    public static string GetStartedPage(
        string? email, string? emailError, string? passwordError
    )
    {
        return UI.Form.Attr("method", "post").Wrap(
            Componnents.Input(
                name: nameof(AuthFormBody.Email), type: "email",
                label: "Email", placeholder: "Your email",
                isRequired: true, error: emailError, value: email
            ),
            Componnents.Input(
                name: nameof(AuthFormBody.Password), type: "password",
                label: "Password", placeholder: "Password",
                isRequired: true, error: passwordError
            ),
            UI.Button["Confirm"]
        );
    }

    //public static async Task GetStartedHandler(
    //    HttpResponse res,
    //    [FromForm] AuthFormBody form,
    //    [FromServices] UserManager<UnatorUser> userManager,
    //    [FromServices] SignInManager<UnatorUser> signInManager,
    //    [FromQuery] string? comeback = null
    //)
    //{
    //    if (!form.IsValid())
    //    {
    //        await GetStartedPage(Wave.Html(res, StatusCodes.Status400BadRequest),
    //            form.Email, "Email is required", "Password is required"
    //        );
    //        return;
    //    }

    //    var user = await userManager.FindByEmailAsync(form.Email);
    //    if (user is not null)
    //    {
    //        var result = await signInManager.PasswordSignInAsync(user, form.Password, true, false);
    //        if (!result.Succeeded)
    //        {
    //            var wave = Wave.Html(res, StatusCodes.Status400BadRequest);
    //            await GetStartedPage(wave, form.Email, "Looks good", "We can't login you with provided password");
    //            return;
    //        }

    //        res.Redirect(DashboardHandlers.DashboardRoute.Url());
    //        return;
    //    }
    //    else
    //    {
    //        var newUser = UnatorUser.New(form.Email);
    //        var createRes = await userManager.CreateAsync(newUser, form.Password);
    //        var signInRes = await signInManager.PasswordSignInAsync(newUser, form.Password, true, false);
    //        if (createRes.Succeeded && signInRes.Succeeded)
    //        {
    //            res.Redirect(DashboardHandlers.DashboardRoute.Url());
    //            return;
    //        }

    //        string? emailError = null;
    //        List<string> passwordErrors = [];
    //        foreach (var error in createRes.Errors)
    //        {
    //            if (error.Code == nameof(IdentityErrorDescriber.DuplicateEmail))
    //            {
    //                emailError = "Email is already take by another account";
    //            }
    //            else if (error.Code == nameof(IdentityErrorDescriber.InvalidEmail))
    //            {
    //                emailError = "Email is invalid";
    //            }
    //            else if (error.Code.StartsWith("Password"))
    //            {
    //                passwordErrors.Add(error.Description);
    //            }
    //        }

    //        await GetStartedPage(Wave.Html(res, StatusCodes.Status400BadRequest),
    //            form.Email, emailError, string.Join(".", passwordErrors)
    //        );
    //    }
    //}

    public static readonly UnatorRoute LogoutRoute = AuthRoute.Add("logout");

    public static async Task Logout(
        HttpContext ctx,
        [FromServices] SignInManager<DbUser> signInManager
    )
    {
        await signInManager.SignOutAsync();
        ctx.Response.Redirect("/");
    }

    public static readonly UnatorRoute ForgotPasswordRoute = AuthRoute.Add("forgot-password");

    public static async Task ForgotPasswordHandler(
        HttpContext ctx
    )
    {
    }
}