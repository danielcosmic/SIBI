using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace backend.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> EnviarBienvenidaAsync(string correoDestino, string nombre, string contrasenaTemp)
    {
        try
        {
            var cfg = _config.GetSection("Email");
            var host     = cfg["SmtpHost"]!;
            var port     = int.Parse(cfg["SmtpPort"]!);
            var usuario  = cfg["Usuario"]!;
            var password = cfg["Contrasena"]!;

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(cfg["NombreRemitente"], usuario));
            mensaje.To.Add(new MailboxAddress(nombre, correoDestino));
            mensaje.Subject = "Bienvenido/a al Sistema SIBI · EIC UCR";

            mensaje.Body = new TextPart("html") { Text = GenerarHtml(nombre, correoDestino, contrasenaTemp) };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(usuario, password);
            await client.SendAsync(mensaje);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de bienvenida a {Correo}", correoDestino);
            return false;
        }
    }

    private static string GenerarHtml(string nombre, string correo, string contrasena) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        </head>
        <body style="margin:0;padding:0;background:#f0f4f8;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f0f4f8;padding:32px 16px;">
            <tr><td align="center">
              <table width="100%" style="max-width:560px;background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,61,122,0.10);">

                <!-- Encabezado -->
                <tr>
                  <td style="background:linear-gradient(135deg,#0066cc 0%,#003d7a 100%);padding:32px 40px;text-align:center;">
                    <p style="margin:0 0 4px 0;font-size:22px;font-weight:700;color:#ffffff;letter-spacing:1px;">SIBI</p>
                    <p style="margin:0;font-size:13px;color:#a8cef5;letter-spacing:0.5px;">Sistema de Inventario de Bienes Institucionales</p>
                    <p style="margin:8px 0 0 0;font-size:12px;color:#7ab8f0;">Escuela de Ingeniería Civil · Universidad de Costa Rica</p>
                  </td>
                </tr>

                <!-- Cuerpo -->
                <tr>
                  <td style="padding:36px 40px 28px 40px;">
                    <p style="margin:0 0 8px 0;font-size:20px;font-weight:600;color:#003d7a;">Bienvenido/a, {EscHtml(nombre)}</p>
                    <p style="margin:0 0 24px 0;font-size:14px;color:#555;line-height:1.6;">
                      Se ha creado una cuenta en el sistema SIBI de la EIC. A continuación encontrás tus credenciales de acceso:
                    </p>

                    <!-- Credenciales -->
                    <table width="100%" cellpadding="0" cellspacing="0" style="background:#f0f6ff;border:1px solid #c7dff7;border-radius:12px;margin-bottom:24px;">
                      <tr>
                        <td style="padding:20px 24px;">
                          <p style="margin:0 0 12px 0;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.8px;color:#0066cc;">Tus credenciales</p>
                          <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                              <td style="padding:6px 0;">
                                <span style="display:inline-block;width:90px;font-size:13px;color:#666;">Correo</span>
                                <span style="font-size:14px;font-weight:600;color:#003d7a;">{EscHtml(correo)}</span>
                              </td>
                            </tr>
                            <tr>
                              <td style="padding:6px 0;">
                                <span style="display:inline-block;width:90px;font-size:13px;color:#666;">Contraseña</span>
                                <span style="font-size:18px;font-weight:700;color:#003d7a;letter-spacing:3px;font-family:Consolas,monospace;">{EscHtml(contrasena)}</span>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>

                    <!-- Aviso contraseña temporal -->
                    <table width="100%" cellpadding="0" cellspacing="0" style="background:#fff8e1;border:1px solid #ffe082;border-radius:10px;margin-bottom:24px;">
                      <tr>
                        <td style="padding:14px 20px;">
                          <p style="margin:0;font-size:13px;color:#795548;line-height:1.6;">
                            <strong>&#9888; Contraseña temporal:</strong> Al ingresar por primera vez, el sistema te pedirá que establezcas una contraseña personal. Esta acción es obligatoria antes de poder usar el sistema.
                          </p>
                        </td>
                      </tr>
                    </table>

                    <p style="margin:0;font-size:14px;color:#555;line-height:1.6;">
                      Si tenés alguna duda, comunicate con la administradora del sistema.
                    </p>
                  </td>
                </tr>

                <!-- Pie -->
                <tr>
                  <td style="background:#f8fafc;border-top:1px solid #e8eef5;padding:20px 40px;text-align:center;">
                    <p style="margin:0;font-size:12px;color:#94a3b8;">
                      SIBI · Escuela de Ingeniería Civil · Universidad de Costa Rica<br/>
                      Este es un correo automático, por favor no respondas a este mensaje.
                    </p>
                  </td>
                </tr>

              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;

    private static string EscHtml(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}
