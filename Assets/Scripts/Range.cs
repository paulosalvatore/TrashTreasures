[System.Serializable]
public class Range
{
	public int min, max;

	public Range(int min, int max)
	{
		this.min = min;
		this.max = max;
	}
}

[System.Serializable]
public class RangeFloat
{
	public float min, max;

	public RangeFloat(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
