using System.Collections.Generic;
using Kiloo.Common.Extensions;

namespace UI.VideoAdPrizes
{
	public class VideoAdPrizePools
	{
		public List<VideoAdPrize> introPrizePool = new List<VideoAdPrize>
		{
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.MysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox)
		};

		private List<VideoAdPrize> randomPrizePool = new List<VideoAdPrize>
		{
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.SuperMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.Keys),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.Coins, 1000),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox),
			new VideoAdPrize(VideoAdPrizeType.MiniMysteryBox)
		};

		public List<VideoAdPrize> GetShuffledCloneOfRandomPrizePool(int seed)
		{
			List<VideoAdPrize> list = new List<VideoAdPrize>();
			foreach (VideoAdPrize item in randomPrizePool)
			{
				list.Add(item);
			}
			list.Shuffle(seed);
			return list;
		}
	}
}
