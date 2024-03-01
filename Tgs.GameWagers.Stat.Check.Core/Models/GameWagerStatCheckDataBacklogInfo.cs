using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameWagers.Stat.Check.Core.Models;
public record GameWagerStatCheckDataBacklogInfo(
	string PlayerCode,
	DateTimeOffset StatAt,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	DateTimeOffset TriggerAt,
	int Count);
