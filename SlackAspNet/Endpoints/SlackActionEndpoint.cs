﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAspNet.Endpoints
{
	public class SlackActionEndpoint : SlackEndpoint<SlackActionContext>
	{
		public SlackActionEndpoint(SlackActionContext context) : base(context)
		{
		}
	}
}
