using System;

public class PromoCodeEventArgs : EventArgs
{
	private string _code;

	private int _numCharacters;

	private int _numBoards;

	private int _numKeys;

	private int _numCoins;

	public string code
	{
		get
		{
			return _code;
		}
	}

	public int numCharacters
	{
		get
		{
			return _numCharacters;
		}
	}

	public int numBoards
	{
		get
		{
			return _numBoards;
		}
	}

	public int numKeys
	{
		get
		{
			return _numKeys;
		}
	}

	public int numCoins
	{
		get
		{
			return _numCoins;
		}
	}

	public PromoCodeEventArgs(string code, int nCharacters, int nBoards, int nKeys, int nCoins)
	{
		_code = code;
		_numCharacters = nCharacters;
		_numBoards = nBoards;
		_numKeys = nKeys;
		_numCoins = nCoins;
	}

	public override string ToString()
	{
		return string.Format("[PromoCodeEventArgs: l-code={0} 2-numCharacters={1} 3-numBoards={2} 4-numKeys={3} 5-numCoins={4}", _code, _numCharacters, _numBoards, _numKeys, _numCoins);
	}
}
