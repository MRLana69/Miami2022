using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Backend.Cloud
{
	public interface ICloudServer
	{
		string kilooTag { get; }

		Coroutine ListItemsAsync(string forKilooTag, [NotNull] CloudServerRequestCompleteHandler<CloudServerResult> completeHandler);

		CloudServerResult ListItems(string forKilooTag);

		Coroutine UpdateItemsAsync(string forKilooTag, [NotNull] IEnumerable<ICloudItem> items, [NotNull] CloudServerRequestCompleteHandler<CloudServerResult> completeHandler);

		CloudServerResult UpdateItems(string forKilooTag, [NotNull] IEnumerable<ICloudItem> items);

		Coroutine CommitItemsAsync(string forKilooTag, [NotNull] IEnumerable<ICloudItem> items, [NotNull] CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler);

		CloudServerCommitResult CommitItems(string forKilooTag, [NotNull] IEnumerable<ICloudItem> items);
	}
}
