using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameWagers.Stat.Check.Core;
internal class StatisticalTimeUtnsil : IStatisticalTimeUtnsil
{
	public (DateTimeOffset StartOn, DateTimeOffset EndOn) GetInterval(DateTimeOffset startOn, DateTimeOffset endOn)
	{
		var startMins = startOn.Minute % 5;
		var startOnMins = startOn.AddMinutes(startMins * -1);
		var endMins = 5 - startOn.Minute % 5;
		var endOnMins = endOn.AddMinutes(endMins);
		return (
			new DateTimeOffset(startOnMins.Year, startOnMins.Month, startOnMins.Day, startOnMins.Hour, startOnMins.Minute, 00, startOnMins.Offset),
			new DateTimeOffset(endOnMins.Year, endOnMins.Month, endOnMins.Day, endOnMins.Hour, endOnMins.Minute, 00, endOnMins.Offset));
	}
}
