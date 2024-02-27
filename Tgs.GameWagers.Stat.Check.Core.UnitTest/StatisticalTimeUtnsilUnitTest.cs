namespace Tgs.GameWagers.Stat.Check.Core.UnitTest;
public class StatisticalTimeUtnsilUnitTest
{
	[Fact]
	public void DateTimeOffset_24_02_22_11_49_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 49, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 45, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_50_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 50, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 50, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_54_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 54, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 50, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_55_00_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 55, 00, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 55, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_59_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 59, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 55, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_12_00_00_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 12, 00, 00, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 12, 00, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_Start_On_24_02_22_11_03_59_08_End_On_24_02_22_11_57_59_08()
	{
		var startOn = new DateTimeOffset(2024, 02, 22, 11, 03, 59, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 02, 22, 11, 57, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = utnsil.GetStatInterval(startOn, endOn);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 00, 00, TimeSpan.FromHours(+8)), StartOn);
		Assert.Equal(new DateTimeOffset(2024, 02, 22, 12, 00, 00, TimeSpan.FromHours(+8)), EndOn);
	}

	[Fact]
	public void DateTimeOffset_Start_On_24_02_22_10_57_59_08_End_On_24_02_22_11_57_59_08()
	{
		var startOn = new DateTimeOffset(2024, 02, 22, 10, 57, 59, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 02, 22, 11, 57, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = utnsil.GetStatInterval(startOn, endOn);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 10, 55, 00, TimeSpan.FromHours(+8)), StartOn);
		Assert.Equal(new DateTimeOffset(2024, 02, 22, 12, 00, 00, TimeSpan.FromHours(+8)), EndOn);
	}

	[Fact]
	public void DateTimeOffset_Start_On_24_02_22_10_57_59_08_End_On_24_02_22_12_04_59_08()
	{
		var startOn = new DateTimeOffset(2024, 02, 22, 10, 57, 59, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 02, 22, 12, 04, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = utnsil.GetStatInterval(startOn, endOn);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 10, 55, 00, TimeSpan.FromHours(+8)), StartOn);
		Assert.Equal(new DateTimeOffset(2024, 02, 22, 12, 05, 00, TimeSpan.FromHours(+8)), EndOn);
	}
}
