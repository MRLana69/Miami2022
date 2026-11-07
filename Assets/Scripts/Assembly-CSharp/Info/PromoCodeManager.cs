using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Managers;
using Data.Save;
using Kiloo.Common.Backend.CodeRedemption;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace Info
{
	public class PromoCodeManager
	{
		private class PromoCodeData
		{
			public string appVersion { get; set; }

			public string codeId { get; set; }

			public bool testCode { get; set; }

			public bool restoration { get; set; }

			public string[] boardIds { get; set; }

			public string keys { get; set; }

			public string coins { get; set; }

			public string[] characterIds { get; set; }
		}

		private class CelebrationData
		{
			public CelebrationRewardType rewardType { get; set; }

			public UpgradeType upgradeType { get; set; }

			public int amount { get; set; }

			public UnlockItemSource unlockItemSource { get; set; }

			public Characters.CharacterId characterId { get; set; }

			public Boards.BoardId boardId { get; set; }

			public CelebrationRewardOrigin origin { get; set; }
		}

		private static PromoCodeManager _instance;

		private const string BASE_URL = "https://coderedemption.kiloo-games.com";

		private IJsonSerializer _jsonSerializer;

		private ICodeRedemptionServer<PromoCodeData> _codeRedemptionServer;

		private string _code;

		private Action<int> _showErrorCode;

		private Coroutine _getPayloadRoutine;

		private const string RESTORATION = "Restoration";

		private const string PROMOTIONAL = "Promotional";

		public int restoredNumberOfCharacters;

		public int restoredNumberOfKeys;

		public int restoredNumberOfCoins;

		public int restoredNumberOfBoards;

		private const int NO_CODE_ENTERED = 0;

		private const int UPDATE_APP_ERROR = 100;

		private const int CONTACT_SUPPORT_ERROR = 101;

		private const int CONTENT_ALREADY_OWNED = 102;

		private const int TRY_AGAIN_ERROR = 7000;

		private const int CODE_DOES_NOT_EXIST = 7004;

		private const int CODE_ALREADY_USED = 7007;

		public Action<object, PromoCodeEventArgs> promoCodeUsed { get; set; }

		public static PromoCodeManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PromoCodeManager();
				}
				return _instance;
			}
		}

		public void StopGetRoutine()
		{
			if (_getPayloadRoutine != null)
			{
				CoroutineUtil.StopCoroutine(_getPayloadRoutine);
				_getPayloadRoutine = null;
			}
		}

		public void ActivateCode(string code, Action<int> errorCode)
		{
			_code = code;
			_showErrorCode = errorCode;
			if (string.IsNullOrEmpty(_code))
			{
				_showErrorCode(0);
				return;
			}
			if (!PromoCodeIsUsed())
			{
				GetPayloadAsync();
				return;
			}
			Timer.ExecuteNextFrame(() =>
			{
				_showErrorCode(7007);
			});
			PromoCodeErrorLogging("ClientError", "Code already used");
		}

		private bool IsAppVersionHighEnough(string version)
		{
			string bundleVersion = DeviceUtility.GetBundleVersion();
			if (string.IsNullOrEmpty(version))
			{
				PromoCodeErrorLogging("ClientError", "Error in payload");
				_showErrorCode(101);
				return false;
			}
			char[] separator = new char[1] { '.' };
			string[] array = version.Split(separator);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = int.Parse(array[i]);
			}
			string[] array3 = bundleVersion.Split(separator);
			int[] array4 = new int[array3.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				array4[j] = int.Parse(array3[j]);
			}
			for (int k = 0; k < array4.Length && array2[k] >= array4[k]; k++)
			{
				if (array2[k] > array4[k])
				{
					PromoCodeErrorLogging("ClientError", "Need to update");
					_showErrorCode(100);
					return false;
				}
			}
			return true;
		}

		private void GetPayloadAsync()
		{
			if (_jsonSerializer == null)
			{
				_jsonSerializer = new KilooSerializer
				{
					indent = false
				};
			}
			if (_codeRedemptionServer == null)
			{
				_codeRedemptionServer = new CodeRedemptionServer<PromoCodeData>(new Uri("https://coderedemption.kiloo-games.com"), _jsonSerializer);
			}
			_getPayloadRoutine = _codeRedemptionServer.GetPayloadAsync(_code, (PromoCodeData result, string error) =>
			{
				_getPayloadRoutine = null;
				if (error != null)
				{
					if (error.Contains("Request failed"))
					{
						_showErrorCode(7000);
						PromoCodeErrorLogging("ClientError", "Get payload timeout");
					}
					else
					{
						string[] array = SplitErrorMessage(error);
						if (array != null && array.Length == 2)
						{
							_showErrorCode(int.Parse(array[1]));
							string[] separator = new string[1] { "(" };
							string[] array2 = array[0].Split(separator, StringSplitOptions.None);
							PromoCodeErrorLogging("ServerError", array2[0]);
						}
						else
						{
							_showErrorCode(101);
							PromoCodeErrorLogging("ServerError", error);
						}
					}
				}
				else if (result != null)
				{
					if (ProcessPayload(result))
					{
						PostPayloadConfirmationAsync();
					}
				}
				else
				{
					PromoCodeErrorLogging("ClientError", "Error in payload");
					_showErrorCode(101);
				}
			});
		}

		private string[] SplitErrorMessage(string errorMessage)
		{
			string[] separator = new string[1] { "Errorcode: " };
			return errorMessage.Split(separator, StringSplitOptions.None);
		}

		private bool PromoCodeIsUsed()
		{
			List<string> promoCodes = UserSave.promoCodes;
			if (promoCodes != null)
			{
				return promoCodes.Contains(_code);
			}
			return false;
		}

		private void SavePromoCode()
		{
			if (!Globals.DEBUG_PROMO_CODE_SAVE_DISABLE)
			{
				UserSave.promoCodes.Add(_code);
			}
		}

		private bool PromoCodeIdIsUsed(PromoCodeData payload)
		{
			if (!string.IsNullOrEmpty(payload.codeId))
			{
				List<string> promoCodeIds = UserSave.promoCodeIds;
				if (promoCodeIds != null)
				{
					return promoCodeIds.Contains(payload.codeId);
				}
			}
			return false;
		}

		private void SavePromoCodeId(PromoCodeData payload)
		{
			if (!string.IsNullOrEmpty(payload.codeId) && !Globals.DEBUG_PROMO_CODE_ID_SAVE_DISABLE)
			{
				UserSave.promoCodeIds.Add(payload.codeId);
			}
		}

		private void PostPayloadConfirmationAsync()
		{
			_codeRedemptionServer.PostPayoutConfirmationAsync(_code, (bool success, string error) =>
			{
				PromoCodeConfirmationLogging((!success) ? "NotConfirmed" : "Confirmed");
			});
		}

		private bool ProcessPayload(PromoCodeData payload)
		{
			if (!IsAppVersionHighEnough(payload.appVersion))
			{
				return false;
			}
			if (payload.testCode && Globals.BUILD_PURPOSE == Globals.BuildPurpose.LIVE)
			{
				_showErrorCode(7004);
				return false;
			}
			if (PromoCodeIdIsUsed(payload))
			{
				_showErrorCode(7007);
				PromoCodeErrorLogging("ClientError", "Code already used");
				return false;
			}
			bool flag;
			if (payload.restoration)
			{
				flag = PrepareRestorationContent(payload);
				if (flag)
				{
					UIScreenController.Instance.ClosePopup();
					UIScreenController.Instance.PushPopup("PromoCodeRestoredPopup");
				}
			}
			else
			{
				flag = PrepareCelebrationScreens(payload);
				if (flag)
				{
					UIScreenController.Instance.ClosePopup();
					RewardManager.canShowMultipleQueuedCelebrations = true;
					UIScreenController.Instance.PushCelebrationPopup();
				}
			}
			if (flag)
			{
				SavePromoCode();
				SavePromoCodeId(payload);
			}
			return flag;
		}

		private void UnlockCharacters(List<Characters.CharacterId> charactersToUnlock)
		{
			if (charactersToUnlock.Count <= 0)
			{
				return;
			}
			foreach (Characters.CharacterId item in charactersToUnlock)
			{
				CharacterManager.UnlockCharacter(item, true);
			}
			CharacterScreen.forceRefreshOnCharacterScreen = true;
		}

		private void UnlockBoards(List<Boards.BoardId> boardsToUnlock)
		{
			if (boardsToUnlock.Count <= 0)
			{
				return;
			}
			foreach (Boards.BoardId item in boardsToUnlock)
			{
				BoardManager.UnlockBoard(item);
			}
		}

		private bool PrepareRestorationContent(PromoCodeData payload)
		{
			List<Characters.CharacterId> list = new List<Characters.CharacterId>();
			if (payload.characterIds != null && payload.characterIds.Length != 0)
			{
				List<Characters.CharacterId> list2 = ValidateCharacterIds(payload.characterIds);
				if (list2.Count == 0)
				{
					_showErrorCode(101);
					PromoCodeErrorLogging("ClientError", "Error in payload");
					return false;
				}
				foreach (Characters.CharacterId item in list2)
				{
					if (!CharacterManager.IsCharacterOwned(item))
					{
						list.Add(item);
					}
				}
			}
			List<Boards.BoardId> list3 = new List<Boards.BoardId>();
			if (payload.boardIds != null && payload.boardIds.Length != 0)
			{
				List<Boards.BoardId> list4 = ValidateBoardIds(payload.boardIds);
				if (list4.Count == 0)
				{
					_showErrorCode(101);
					PromoCodeErrorLogging("ClientError", "Error in payload");
					return false;
				}
				foreach (Boards.BoardId item2 in list4)
				{
					if (!BoardManager.IsBoardOwned(item2))
					{
						list3.Add(item2);
					}
				}
			}
			int result = 0;
			if (!string.IsNullOrEmpty(payload.keys) && !int.TryParse(payload.keys, out result))
			{
				_showErrorCode(101);
				PromoCodeErrorLogging("ClientError", "Error in payload");
				return false;
			}
			int result2 = 0;
			if (!string.IsNullOrEmpty(payload.coins) && !int.TryParse(payload.coins, out result2))
			{
				_showErrorCode(101);
				PromoCodeErrorLogging("ClientError", "Error in payload");
				return false;
			}
			if (list.Count + list3.Count + result + result2 <= 0)
			{
				_showErrorCode(102);
				PromoCodeErrorLogging("ClientError", "Content already owned");
				return false;
			}
			restoredNumberOfCharacters = list.Count;
			restoredNumberOfBoards = list3.Count;
			restoredNumberOfCoins = result2;
			restoredNumberOfKeys = result;
			UnlockCharacters(list);
			UnlockBoards(list3);
			InventorySave.IncreaseCoins(result2, "Promo_code");
			InventorySave.IncreaseKeys(result, "Promo_code");
			PromoCodeUsedFlurryLogging(payload.restoration, list.Count, list3.Count, result2, result);
			return true;
		}

		private List<Boards.BoardId> ValidateBoardIds(string[] boardStrings)
		{
			List<Boards.BoardId> list = new List<Boards.BoardId>();
			foreach (string value in boardStrings)
			{
				try
				{
					Boards.BoardId item = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), value);
					list.Add(item);
				}
				catch (Exception)
				{
					return new List<Boards.BoardId>();
				}
			}
			return list;
		}

		private List<Characters.CharacterId> ValidateCharacterIds(string[] characterStrings)
		{
			List<Characters.CharacterId> list = new List<Characters.CharacterId>();
			foreach (string value in characterStrings)
			{
				try
				{
					Characters.CharacterId item = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), value);
					list.Add(item);
				}
				catch (Exception)
				{
					return new List<Characters.CharacterId>();
				}
			}
			return list;
		}

		private void SetupCharacterCelebration(List<Characters.CharacterId> charactersToUnlock)
		{
			foreach (Characters.CharacterId item in charactersToUnlock)
			{
				CelebrationData celebrationData = new CelebrationData();
				celebrationData.origin = CelebrationRewardOrigin.CharacterUnlock;
				celebrationData.rewardType = CelebrationRewardType.character;
				celebrationData.unlockItemSource = UnlockItemSource.PromoCode;
				celebrationData.characterId = item;
				CelebrationData celebrationData2 = celebrationData;
				SetupCelebrationScreen(celebrationData2);
			}
			UnlockCharacters(charactersToUnlock);
		}

		private void SetupBoardCelebration(List<Boards.BoardId> boardsToUnlock)
		{
			foreach (Boards.BoardId item in boardsToUnlock)
			{
				CelebrationData celebrationData = new CelebrationData();
				celebrationData.rewardType = CelebrationRewardType.specialBoard;
				celebrationData.unlockItemSource = UnlockItemSource.PromoCode;
				celebrationData.origin = CelebrationRewardOrigin.BoardUnlock;
				celebrationData.boardId = item;
				CelebrationData celebrationData2 = celebrationData;
				SetupCelebrationScreen(celebrationData2);
			}
			UnlockBoards(boardsToUnlock);
		}

		private bool PrepareCelebrationScreens(PromoCodeData payload)
		{
			List<Characters.CharacterId> list = new List<Characters.CharacterId>();
			if (payload.characterIds != null && payload.characterIds.Length != 0)
			{
				List<Characters.CharacterId> list2 = ValidateCharacterIds(payload.characterIds);
				if (list2.Count == 0)
				{
					_showErrorCode(101);
					PromoCodeErrorLogging("ClientError", "Error in payload");
					return false;
				}
				foreach (Characters.CharacterId item in list2)
				{
					if (!CharacterManager.IsCharacterOwned(item))
					{
						list.Add(item);
					}
				}
			}
			List<Boards.BoardId> list3 = new List<Boards.BoardId>();
			if (payload.boardIds != null && payload.boardIds.Length != 0)
			{
				List<Boards.BoardId> list4 = ValidateBoardIds(payload.boardIds);
				if (list4.Count == 0)
				{
					_showErrorCode(101);
					PromoCodeErrorLogging("ClientError", "Error in payload");
					return false;
				}
				foreach (Boards.BoardId item2 in list4)
				{
					if (!BoardManager.IsBoardOwned(item2))
					{
						list3.Add(item2);
					}
				}
			}
			int result = 0;
			if (!string.IsNullOrEmpty(payload.keys) && !int.TryParse(payload.keys, out result))
			{
				_showErrorCode(101);
				PromoCodeErrorLogging("ClientError", "Error in payload");
				return false;
			}
			int result2 = 0;
			if (!string.IsNullOrEmpty(payload.coins) && !int.TryParse(payload.coins, out result2))
			{
				_showErrorCode(101);
				PromoCodeErrorLogging("ClientError", "Error in payload");
				return false;
			}
			if (list.Count + list3.Count + result + result2 <= 0)
			{
				_showErrorCode(102);
				PromoCodeErrorLogging("ClientError", "Content already owned");
				return false;
			}
			SetupCharacterCelebration(list);
			SetupBoardCelebration(list3);
			if (result != 0)
			{
				CelebrationData celebrationData = new CelebrationData();
				celebrationData.rewardType = CelebrationRewardType.keys;
				celebrationData.amount = result;
				celebrationData.origin = CelebrationRewardOrigin.PromoCode;
				CelebrationData celebrationData2 = celebrationData;
				SetupCelebrationScreen(celebrationData2);
			}
			if (result2 != 0)
			{
				CelebrationData celebrationData = new CelebrationData();
				celebrationData.rewardType = CelebrationRewardType.coins;
				celebrationData.amount = result2;
				celebrationData.origin = CelebrationRewardOrigin.PromoCode;
				CelebrationData celebrationData3 = celebrationData;
				SetupCelebrationScreen(celebrationData3);
			}
			PromoCodeUsedFlurryLogging(payload.restoration, list.Count, list3.Count, result2, result);
			promoCodeUsed.SafeInvoke(this, new PromoCodeEventArgs(payload.codeId, list.Count, list3.Count, result, result2));
			return true;
		}

		private void PromoCodeErrorLogging(string errorType, string errorCourse)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(errorType, errorCourse);
			Flurry.LogEventWithSeveralParameters("Promo code error", dictionary);
		}

		private void PromoCodeConfirmationLogging(string value)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Result", value);
			Flurry.LogEventWithSeveralParameters("Promo code confirmation", dictionary);
		}

		private void PromoCodeUsedFlurryLogging(bool restoration, int numOfCharacters, int numOfBoards, int numOfCoins, int numOfKeys)
		{
			string value = ((!restoration) ? "Promotional" : "Restoration");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("CodeType", value);
			dictionary.Add("Characters", numOfCharacters.ToString());
			dictionary.Add("Boards", numOfBoards.ToString());
			dictionary.Add("Coins", numOfCoins.ToString());
			dictionary.Add("Keys", numOfKeys.ToString());
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("Promo Code Used", parameters);
		}

		private void SetupCelebrationScreen(CelebrationData celebrationData)
		{
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = celebrationData.origin;
			celebrationReward.rewardType = celebrationData.rewardType;
			celebrationReward.upgradeType = celebrationData.upgradeType;
			celebrationReward.amount = celebrationData.amount;
			celebrationReward.characterId = celebrationData.characterId;
			celebrationReward.boardId = celebrationData.boardId;
			celebrationReward.syboAnalyticsSourceMetaOrNull = celebrationData.unlockItemSource.ToString();
			celebrationReward.unlockItemSource = celebrationData.unlockItemSource;
			CelebrationReward reward = celebrationReward;
			RewardManager.AddRewardToUnlock(reward);
		}

		[Conditional("PROMO_CODE_MANAGER")]
		private static void Log(string msg)
		{
		}
	}
}
