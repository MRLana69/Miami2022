public struct MissionIndicator
{
	public int versionNo;

	public int missionSet;

	public int missionNumberInSet;

	public MissionIndicator(int versionNumber, int missionSet, int missionIndexInSet)
	{
		versionNo = versionNumber;
		this.missionSet = missionSet;
		missionNumberInSet = missionIndexInSet;
	}
}
