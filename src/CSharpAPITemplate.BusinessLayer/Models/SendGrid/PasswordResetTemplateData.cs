using Newtonsoft.Json;

namespace SkillZilla.BusinessLayer.SendGrid.TemplateData;

/// <summary>
/// Model for filling password reset email template data. 
/// </summary>
[JsonObject]
public class PasswordResetTemplateData
{
	public PasswordResetTemplateData(string url)
	{
		Url = url;
	}

	/// <summary>
	/// Reset password page url for SkillZilla.Console application with token in params.
	/// </summary>
	[JsonProperty("url")] 
	public string Url { get; set; }
}