using System;
using System.Runtime.InteropServices;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class AndroidVideoPlayerPlugin : IAndroidVideoPlayerPlugin, INativeClass
	{
		[DllImport("glbridge")]
		private static extern IntPtr CreateVideoPlayer(IntPtr jniEnv, int listenerId, string url, IntPtr surfaceTexture, IntPtr surface);

		[DllImport("glbridge")]
		private static extern void VideoPlayerPrepare(IntPtr videoPlayer);

		[DllImport("glbridge")]
		private static extern void VideoPlayerPlay(IntPtr videoPlayer);

		[DllImport("glbridge")]
		private static extern void VideoPlayerPause(IntPtr videoPlayer);

		[DllImport("glbridge")]
		private static extern void VideoPlayerSetOnPreparedMethod(IntPtr onPreparedMethod);

		[DllImport("glbridge")]
		private static extern void VideoPlayerSetOnFrameAvailableMethod(IntPtr onFrameAvailableMethod);

		public IntPtr NativeCreateVideoPlayer(IntPtr jniEnv, int listenerId, string url, IntPtr surface, IntPtr surfaceTexture)
		{
			return CreateVideoPlayer(jniEnv, listenerId, url, surface, surfaceTexture);
		}

		public void NativeVideoPlayerPrepare(IntPtr videoPlayer)
		{
			VideoPlayerPrepare(videoPlayer);
		}

		public void NativeVideoPlayerPlay(IntPtr videoPlayer)
		{
			VideoPlayerPlay(videoPlayer);
		}

		public void NativeVideoPlayerPause(IntPtr videoPlayer)
		{
			VideoPlayerPause(videoPlayer);
		}

		public void NativeVideoPlayerSetOnPreparedMethod(IntPtr onPreparedMethod)
		{
			VideoPlayerSetOnPreparedMethod(onPreparedMethod);
		}

		public void NativeVideoPlayerSetOnFrameAvailableMethod(IntPtr onFrameAvailableMethod)
		{
			VideoPlayerSetOnFrameAvailableMethod(onFrameAvailableMethod);
		}
	}
}
