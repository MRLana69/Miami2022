using System;
using System.Collections.Generic;
using SYBO.Subway.Characters;
using SYBO.Subway.Level;
using SYBO.Subway.Routes;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class RoofedCulling
	{
		[Serializable]
		public class CustomRoofedCulling
		{
			public EnvironmentType EnvironmentType;

			public Vector4 CullingBounds = Vector4.zero;
		}

		private class CullingBlock
		{
			private Vector4 _value = Vector4.zero;

			public bool Low;

			public bool Finalized;

			public float CullStartGameZ
			{
				get
				{
					return _value.x;
				}
				set
				{
					_value.x = value;
				}
			}

			public float CullStartGameY
			{
				get
				{
					return _value.y;
				}
				set
				{
					_value.y = value;
				}
			}

			public float CullEndGameZ
			{
				get
				{
					return _value.z;
				}
				set
				{
					_value.z = value;
				}
			}

			public float CullEndGameY
			{
				get
				{
					return _value.w;
				}
				set
				{
					_value.w = value;
				}
			}

			public Vector4 ToVector4()
			{
				return _value;
			}
		}

		private const string CurrentCullingKeyword = "_RoofedCullCurrent";

		private const string NextCullingKeyword = "_RoofedCullNext";

		private const string GamePositionOffsetZKeyword = "_RoofedCullingGamePositionOffsetZ";

		private LinkedList<CullingBlock> _cullingBlockList = new LinkedList<CullingBlock>();

		private CullingBlock _currentCullingBlock;

		private CullingBlock _nextCullingBlock;

		private CullingBlock _emptyCullingblock = new CullingBlock
		{
			Finalized = true
		};

		private CullingBlock _menuCullingblock = new CullingBlock
		{
			CullStartGameZ = -1000f,
			CullStartGameY = -1000f,
			CullEndGameZ = 1000f,
			CullEndGameY = 1000f,
			Finalized = true
		};

		private Theme _theme;

		private CullingBlock CurrentCullingBlock
		{
			get
			{
				return _currentCullingBlock;
			}
			set
			{
				_currentCullingBlock = value;
				Shader.SetGlobalVector("_RoofedCullCurrent", _currentCullingBlock.ToVector4());
			}
		}

		private CullingBlock NextCullingBlock
		{
			get
			{
				return _nextCullingBlock;
			}
			set
			{
				_nextCullingBlock = value;
				Shader.SetGlobalVector("_RoofedCullNext", _nextCullingBlock.ToVector4());
			}
		}

		public void Initialize()
		{
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Combine(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
			Game.OnGameStarted += Game_OnGameStarted;
			Game.OnGameEnded += Game_OnGameEnded;
			_theme = ThemeManager.Instance.Theme;
		}

		private void Game_OnGameStarted()
		{
			NextCullingBlock = _emptyCullingblock;
		}

		private void Game_OnGameEnded()
		{
			_cullingBlockList.Clear();
		}

		public void UpdateCulling()
		{
			if (!Game.Instance.IsInGame.Value)
			{
				if (CurrentCullingBlock != _menuCullingblock)
				{
					CurrentCullingBlock = _menuCullingblock;
					NextCullingBlock = _menuCullingblock;
				}
			}
			else if (_cullingBlockList.Count > 0)
			{
				LinkedListNode<CullingBlock> first = _cullingBlockList.First;
				if (CurrentCullingBlock != first.Value)
				{
					CurrentCullingBlock = first.Value;
					if (first.Next != null)
					{
						NextCullingBlock = first.Next.Value;
					}
				}
			}
			else if (CurrentCullingBlock != _emptyCullingblock)
			{
				CurrentCullingBlock = _emptyCullingblock;
				NextCullingBlock = _emptyCullingblock;
			}
			int num = Mathf.FloorToInt(Character.Instance.GamePosition.z) - 90;
			if ((float)num > CurrentCullingBlock.CullEndGameZ)
			{
				if (_cullingBlockList.Count > 0 && CurrentCullingBlock == _cullingBlockList.First.Value)
				{
					_cullingBlockList.RemoveFirst();
				}
				if (_cullingBlockList.Count > 0)
				{
					LinkedListNode<CullingBlock> first2 = _cullingBlockList.First;
					CurrentCullingBlock = first2.Value;
					if (first2.Next != null)
					{
						NextCullingBlock = first2.Next.Value;
					}
				}
				else if (CurrentCullingBlock != _emptyCullingblock)
				{
					CurrentCullingBlock = _emptyCullingblock;
					NextCullingBlock = _emptyCullingblock;
				}
			}
			Shader.SetGlobalFloat("_RoofedCullingGamePositionOffsetZ", GamePositionRoot.Instance.CurrentGameOffsetZ);
		}

		private void RouteChunk_OnEnvironmentSegmentSpawned(EnvironmentKind environmentKind, int gamePositionZ, Route route)
		{
			if (route.Type != RouteType.Default)
			{
				return;
			}
			bool cullLow = false;
			bool cullCustom = false;
			Vector4 customCullingBounds = Vector4.zero;
			bool cullEnvironment;
			EvaluateEnvironment(environmentKind.Type, out cullEnvironment, ref cullLow, ref cullCustom, ref customCullingBounds);
			bool flag = false;
			if (cullEnvironment)
			{
				if (_cullingBlockList.Count > 0)
				{
					CullingBlock value = _cullingBlockList.Last.Value;
					if (!value.Finalized)
					{
						value.CullEndGameZ = gamePositionZ;
						if (CurrentCullingBlock == value)
						{
							CurrentCullingBlock = value;
						}
						if (value.Low != cullLow || cullCustom)
						{
							value.Finalized = true;
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (cullCustom)
					{
						_cullingBlockList.AddLast(GenerateCustomCullingBlock(gamePositionZ, customCullingBounds));
					}
					else
					{
						_cullingBlockList.AddLast(GenerateCullingBlock(gamePositionZ, cullLow));
					}
				}
			}
			else
			{
				if (_cullingBlockList.Count <= 0)
				{
					return;
				}
				CullingBlock value2 = _cullingBlockList.Last.Value;
				if (!value2.Finalized)
				{
					value2.CullEndGameZ = gamePositionZ;
					value2.Finalized = true;
					if (CurrentCullingBlock == value2)
					{
						CurrentCullingBlock = value2;
					}
				}
			}
		}

		private void EvaluateEnvironment(EnvironmentType environmentType, out bool cullEnvironment, ref bool cullLow, ref bool cullCustom, ref Vector4 customCullingBounds)
		{
			cullEnvironment = false;
			for (int i = 0; i < _theme.RoofedCullingEnvironmentsHigh.Length; i++)
			{
				if (environmentType == _theme.RoofedCullingEnvironmentsHigh[i])
				{
					cullEnvironment = true;
					cullLow = false;
					break;
				}
			}
			if (!cullEnvironment)
			{
				for (int j = 0; j < _theme.RoofedCullingEnvironmentsLow.Length; j++)
				{
					if (environmentType == _theme.RoofedCullingEnvironmentsLow[j])
					{
						cullEnvironment = true;
						cullLow = true;
						break;
					}
				}
			}
			if (cullEnvironment)
			{
				return;
			}
			for (int k = 0; k < _theme.RoofedCullingEnvironmentsCustom.Length; k++)
			{
				if (environmentType == _theme.RoofedCullingEnvironmentsCustom[k].EnvironmentType)
				{
					cullEnvironment = true;
					cullCustom = true;
					customCullingBounds = _theme.RoofedCullingEnvironmentsCustom[k].CullingBounds;
					break;
				}
			}
		}

		private CullingBlock GenerateCullingBlock(float gamePosZ, bool low)
		{
			CullingBlock cullingBlock = new CullingBlock();
			cullingBlock.CullStartGameZ = gamePosZ;
			cullingBlock.CullStartGameY = -1000f;
			cullingBlock.CullEndGameZ = gamePosZ;
			cullingBlock.CullEndGameY = ((!low) ? 1000 : 90);
			cullingBlock.Low = low;
			return cullingBlock;
		}

		private CullingBlock GenerateCustomCullingBlock(float gamePosZ, Vector4 customCullingBounds)
		{
			CullingBlock cullingBlock = new CullingBlock();
			cullingBlock.CullStartGameZ = gamePosZ + customCullingBounds.x;
			cullingBlock.CullStartGameY = customCullingBounds.y;
			cullingBlock.CullEndGameZ = gamePosZ + customCullingBounds.z;
			cullingBlock.CullEndGameY = customCullingBounds.w;
			cullingBlock.Finalized = true;
			return cullingBlock;
		}
	}
}
