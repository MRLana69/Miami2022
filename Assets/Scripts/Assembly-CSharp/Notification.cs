using System.Collections.Generic;

public class Notification
{
	public string type { get; set; }

	public string message { get; set; }

	public Dictionary<string, object> userInfo { get; set; }
}
