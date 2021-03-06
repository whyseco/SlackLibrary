﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using Moq;
using Newtonsoft.Json;
using SlackLibrary.Connections.Clients;
using SlackLibrary.Connections.Clients.Chat;
using SlackLibrary.Connections.Responses;
using SlackLibrary.Models;
using SlackLibrary.Tests.Unit.TestExtensions;
using Xunit;

namespace SlackLibrary.Tests.Unit.Connections.Clients.Flurl
{
    public class FlurlChatClientTests : IDisposable
    {
        private readonly HttpTest _httpTest;
        private readonly Mock<IResponseVerifier> _responseVerifierMock;
        private readonly FlurlChatClient _chatClient;
        
        public FlurlChatClientTests()
        {
            _httpTest = new HttpTest();
            _responseVerifierMock = new Mock<IResponseVerifier>();
            _chatClient = new FlurlChatClient(_responseVerifierMock.Object);
        }
        
        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public async Task should_call_expected_url_with_given_slack_key()
        {
            // given
            const string slackKey = "something-that-looks-like-a-slack-key";
            const string channel = "channel-name";
            const string text = "some-text-for-you";

            var expectedResponse = new DefaultStandardResponse();
            _httpTest.RespondWithJson(expectedResponse);

            // when
            await _chatClient.PostMessage(slackKey, channel, text, null);

            // then
            _responseVerifierMock.Verify(x => x.VerifyResponse(Looks.Like(expectedResponse)));
            _httpTest
                .ShouldHaveCalled(ClientConstants.SlackApiHost.AppendPathSegment(FlurlChatClient.SEND_MESSAGE_PATH))
                .WithRequestUrlEncoded(new
				{
					token = slackKey,
					channel = channel,
					text = text,
					as_user = "false",
					link_names = "true"
				})
                .Times(1);
        }

        [Fact]
        public async Task should_add_attachments_if_given()
        {
            // given
            _httpTest.RespondWithJson(new DefaultStandardResponse());
            var attachments = new List<SlackAttachment>
            {
                new SlackAttachment { Text = "dummy text" },
                new SlackAttachment { AuthorName = "dummy author" },
            };
			const string slackKey = "something-that-looks-like-a-slack-key";
			const string channel = "channel-name";
			const string text = "some-text-for-you";

			// when
			await _chatClient.PostMessage(slackKey, channel, text, attachments);

            // then
            _httpTest
                .ShouldHaveCalled(ClientConstants.SlackApiHost.AppendPathSegment(FlurlChatClient.SEND_MESSAGE_PATH))
				.WithRequestUrlEncoded(new
				{
					token = slackKey,
					channel = channel,
					text = text,
					as_user = "false",
					link_names = "true",
					attachments = JsonConvert.SerializeObject(attachments)
				})
                .Times(1);
        }
    }
}