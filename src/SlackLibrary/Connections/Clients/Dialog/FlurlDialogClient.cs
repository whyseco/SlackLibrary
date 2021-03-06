﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using SlackLibrary.Connections.Responses;

namespace SlackLibrary.Connections.Clients.Dialog
{
	public class FlurlDialogClient : IDialogClient
	{
		const string DIALOG_OPEN_PATH = "/api/dialog.open";
		private readonly IResponseVerifier responseVerifier;

		public FlurlDialogClient(IResponseVerifier responseVerifier)
		{
			this.responseVerifier = responseVerifier;
		}

		public async Task<MessageResponse> Open(string slackKey, Models.Dialog dialog, string triggerId)
		{
			var response = await ClientConstants
					   .SlackApiHost
					   .AppendPathSegment(DIALOG_OPEN_PATH)
					   .SetQueryParam("token", slackKey)
					   .SetQueryParam("trigger_id", triggerId)
					   .SetQueryParam("dialog", JsonConvert.SerializeObject(dialog))
					   .GetJsonAsync<MessageResponse>();

			responseVerifier.VerifyDialogResponse(response);

			return response;
		}
	}
}
