using CodeStage.AntiCheat.ObscuredTypes;

[System.Serializable]
public class Range
{
	public ObscuredInt min, max;

	public Range(ObscuredInt min, ObscuredInt max)
	{
		this.min = min;
		this.max = max;
	}
}

[System.Serializable]
public class RangeFloat
{
	public ObscuredFloat min, max;

	public RangeFloat(ObscuredFloat min, ObscuredFloat max)
	{
		this.min = min;
		this.max = max;
	}
}
