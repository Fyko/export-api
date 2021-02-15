using System.Text.Json;
using Newtonsoft.Json.Serialization;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
	private readonly SnakeCaseNamingStrategy _newtonsoftSnakeCaseNamingStrategy
		= new SnakeCaseNamingStrategy();

	public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

	public override string ConvertName(string name)
	{
		/* A conversion to snake case implementation goes here. */

		return _newtonsoftSnakeCaseNamingStrategy.GetPropertyName(name, false);
	}
}