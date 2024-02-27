using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameWagers.Stat.Check.Core;
public interface IStatisticalTimeUtnsil
{
	(DateTimeOffset StartOn, DateTimeOffset EndOn) GetInterval(DateTimeOffset startOn, DateTimeOffset endOn);
}
