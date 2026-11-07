using System;
using System.Collections;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UnityEngine;

public static class FacebookImageDownloader
{
	public static void DownloadImages(string iconImageUrl, string coverImageUrl, Action<Texture2D, Texture2D> onFinishedDownloading)
	{
		CoroutineUtil.StartCoroutine(StartImageDownload(iconImageUrl, coverImageUrl, onFinishedDownloading));
	}

	private static IEnumerator StartImageDownload(string iconImageUrl, string coverImageUrl, Action<Texture2D, Texture2D> onFinishedDownloading)
	{
		Texture2D iconImage = null;
		Texture2D coverImage = null;
		yield return LoadImage(iconImageUrl, (Texture2D x) =>
		{
			iconImage = x;
		});
		yield return LoadImage(coverImageUrl, (Texture2D x) =>
		{
			coverImage = x;
		});
		onFinishedDownloading.SafeInvoke(iconImage, coverImage);
	}

	private static IEnumerator LoadImage(string url, Action<Texture2D> result)
	{
		Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
		using (WWW www = new WWW(url))
		{
			yield return www;
			www.LoadImageIntoTexture(texture);
			if ((bool)texture)
			{
				result(texture);
			}
		}
	}
}
