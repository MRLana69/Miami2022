using UnityEngine;

namespace Kiloo.GameCenter
{
	public class GameCenterLoadPhotoRequest : GameCenterRequest
	{
		public string playerID { get; set; }

		public GameCenterPlugin.PhotoSize size { get; set; }

		public int textureWidth { get; set; }

		public int textureHeight { get; set; }

		public Texture2D result { get; internal set; }
	}
}
