using System.IO;

// Stream.CopyTo do .NET novo não devolve o total copiado; o código do jogo usava uma versão que devolvia
public static class GjStreamUtil
{
	public static int CopyCount(Stream from, Stream to)
	{
		byte[] buffer = new byte[81920];
		int total = 0;
		int read;
		while ((read = from.Read(buffer, 0, buffer.Length)) > 0)
		{
			to.Write(buffer, 0, read);
			total += read;
		}
		return total;
	}
}
