using FluentValidation;
using DiscordChatExporter.Core.Discord;
using ExportAPI.Proto;

namespace ExportAPI.Validators
{
    public class CreateExportRequestValidator : AbstractValidator<CreateExportRequest>
    {
        public CreateExportRequestValidator()
        {
            RuleFor(request => request.Token).NotEmpty().WithMessage("A Discord token was not provided.");
			RuleFor(request => request.ChannelId).Must(channelId => {
				var parsed = Snowflake.TryParse(channelId);
				return parsed != null;
			}).WithMessage("A valid channel ID was not provided.");
        }
    }
}