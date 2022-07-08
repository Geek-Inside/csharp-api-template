using Newtonsoft.Json;

namespace SkillZilla.BusinessLayer.SendGrid.TemplateData;

/// <summary>
/// Model for filling email confirmation template data. 
/// </summary>
[JsonObject]
public class EmailConfirmationTemplateData
{
	public EmailConfirmationTemplateData(string url)
	{
		Url = url;
	}

	/// <summary>
	/// Email confirmation page url for SkillZilla.Console application with token in params.
	/// </summary>
	[JsonProperty("url")] 
	public string Url { get; set; }
}