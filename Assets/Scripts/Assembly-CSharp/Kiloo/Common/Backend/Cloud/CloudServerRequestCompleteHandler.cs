namespace Kiloo.Common.Backend.Cloud
{
	public delegate void CloudServerRequestCompleteHandler<in T>(T result) where T : CloudServerResult;
}
