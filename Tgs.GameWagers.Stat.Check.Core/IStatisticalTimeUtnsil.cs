using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameWagers.Stat.Check.Core;
public interface IStatisticalTimeUtnsil
{
	DateTimeOffset GetStatOn(DateTimeOffset source);
	(DateTimeOffset StartOn, DateTimeOffset EndOn) GetStatInterval(DateTimeOffset startOn, DateTimeOffset endOn);
}
