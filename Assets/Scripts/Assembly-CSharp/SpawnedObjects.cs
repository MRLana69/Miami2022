using System;
using Kiloo;
using UnityEngine;

public class SpawnedObjects
{
	private static int dailyLetterCounter;

	private static int doubleScoreMultiplierCounter;

	private static int jetpackPickupCounter;

	private static int jumpBoosterCounter;

	private static int magnetBoosterCounter;

	private static int mysteryBoxCounter;

	private static int saveMeTokenCounter;

	private static int huntTokenCounter;

	private static int coinCounter;

	private static int pogoCounter;

	private static int superMysteryBoxCounter;

	private static int others;

	public static int metersCounter;

	private static int total;

	private static double startTime;

	private static double endTime;

	private static void resetCounter()
	{
		dailyLetterCounter = 0;
		doubleScoreMultiplierCounter = 0;
		jetpackPickupCounter = 0;
		jumpBoosterCounter = 0;
		magnetBoosterCounter = 0;
		mysteryBoxCounter = 0;
		saveMeTokenCounter = 0;
		huntTokenCounter = 0;
		coinCounter = 0;
		others = 0;
		metersCounter = 0;
		total = 0;
		pogoCounter = 0;
		superMysteryBoxCounter = 0;
	}

	public static void countUP(string powerUp)
	{
		if (powerUp.ToLower().Trim().Equals("dailyletter"))
		{
			dailyLetterCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("doublescoremultiplier"))
		{
			doubleScoreMultiplierCounter++;
		}
		else if (powerUp.ToLower().Trim().Contains("jetpack"))
		{
			jetpackPickupCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("jumpbooster"))
		{
			jumpBoosterCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("magnetbooster"))
		{
			magnetBoosterCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("mysterybox"))
		{
			mysteryBoxCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("savemetoken"))
		{
			saveMeTokenCounter++;
		}
		else if (powerUp.ToLower().Trim().Contains("hunttoken"))
		{
			huntTokenCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("coin"))
		{
			coinCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("meter"))
		{
			metersCounter++;
		}
		else if (powerUp.ToLower().Trim().Equals("pogo"))
		{
			pogoCounter++;
		}
		else if (powerUp.ToLower().Trim().Contains("supermysterybox"))
		{
			superMysteryBoxCounter++;
		}
		else
		{
			others++;
		}
		if (!powerUp.ToLower().Trim().Equals("dailyletter") && !powerUp.ToLower().Trim().Equals("coin") && !powerUp.ToLower().Trim().Equals("hunttoken") && !powerUp.ToLower().Trim().Equals("meter"))
		{
			total++;
		}
	}

	public static void startPowerUpCount()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		startTime = (CDateTime.UtcNow - dateTime).TotalSeconds;
	}

	public static void endPowerUpCount()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		endTime = (CDateTime.UtcNow - dateTime).TotalSeconds;
	}

	public static void printInConsole()
	{
		float num = Mathf.Round((float)(endTime - startTime)) - 3f;
		resetCounter();
	}
}
