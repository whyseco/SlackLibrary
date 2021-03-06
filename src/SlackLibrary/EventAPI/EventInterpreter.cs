﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackLibrary.Connections.Sockets.Messages.Inbound;
using SlackLibrary.Connections.Sockets.Messages.Inbound.ReactionItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackLibrary.EventAPI
{
	public class EventInterpreter : IEventInterpreter
	{
		private InboundOuterEvent CreateInboundCommonOuterEvent<T>(JObject eventJobject) where T : InboundEvent
		{
			var outerEvent = eventJobject.ToObject<InboundOuterCommonEvent>();
			outerEvent.Event = eventJobject["event"].ToObject<T>();
			return outerEvent;
		}

		private InboundOuterEvent CreateReactionInboundCommonOuterEvent(JObject eventJobject)
		{
			var outerEvent = eventJobject.ToObject<InboundOuterCommonEvent>();
			var reactionEvent = eventJobject["event"].ToObject<ReactionEvent>();
			outerEvent.Event = reactionEvent;
			var item = eventJobject["event"]["item"];
			var itemType = item["type"].Value<string>();
			switch (itemType)
			{
				case "message":
					reactionEvent.ReactingTo = item.ToObject<MessageReaction>();
					break;
				case "file":
					reactionEvent.ReactingTo = item.ToObject<FileReaction>();
					break;
				case "file_comment":
					reactionEvent.ReactingTo = item.ToObject<FileCommentReaction>();
					break;
			}
			return outerEvent;
		}

		public InboundOuterEvent InterpretEvent(string json)
		{
			InboundOuterEvent outerEvent = null;
			var eventJobject = JObject.Parse(json);
			try
			{
				var outerEventType = ParseOuterEventType(eventJobject);
				switch (outerEventType)
				{
					case OuterEventType.event_callback:
						var eventType = ParseEventType(eventJobject);
						switch (eventType)
						{
							case EventType.app_mention:
								outerEvent = this.CreateInboundCommonOuterEvent<AppMentionEvent>(eventJobject);
								break;
							case EventType.app_uninstalled:
								outerEvent = this.CreateInboundCommonOuterEvent<AppUninstalledEvent>(eventJobject);
								break;
							case EventType.channel_archive:
								outerEvent = this.CreateInboundCommonOuterEvent<ChannelArchiveEvent>(eventJobject);
								break;
							case EventType.channel_created:
								outerEvent = this.CreateInboundCommonOuterEvent<ChannelCreatedEvent>(eventJobject);
								break;
							case EventType.channel_deleted:
								outerEvent = this.CreateInboundCommonOuterEvent<ChannelDeletedEvent>(eventJobject);
								break;
							case EventType.channel_history_changed:
								outerEvent = this.CreateInboundCommonOuterEvent<ChannelHistoryChangedEvent>(eventJobject);
								break;
							case EventType.channel_rename:
								outerEvent = this.CreateInboundCommonOuterEvent<ChannelRenameEvent>(eventJobject);
								break;
							case EventType.message:
							case EventType.message_dot_channels:
							case EventType.message_dot_groups:
							case EventType.message_dot_im:
							case EventType.message_dot_mpim:
								var msgSubType = ParseEventSubType<MessageSubType>(eventJobject);
								switch (msgSubType)
								{
									case MessageSubType.message_changed:
										outerEvent = this.CreateInboundCommonOuterEvent<MessageChangedEvent>(eventJobject);
										break;
									case MessageSubType.message_deleted:
										outerEvent = this.CreateInboundCommonOuterEvent<MessageDeletedEvent>(eventJobject);
										break;
									default:
										outerEvent = this.CreateInboundCommonOuterEvent<MessageEvent>(eventJobject);
										break;
								}
								break;
							case EventType.reaction_added:
								outerEvent = this.CreateReactionInboundCommonOuterEvent(eventJobject);
								break;
							case EventType.reaction_removed:
								outerEvent = this.CreateReactionInboundCommonOuterEvent(eventJobject);
								break;
							case EventType.team_domain_change:
								outerEvent = this.CreateInboundCommonOuterEvent<TeamDomainChangeEvent>(eventJobject);
								break;
							case EventType.team_join:
								outerEvent = this.CreateInboundCommonOuterEvent<TeamJoinEvent>(eventJobject);
								break;
							case EventType.team_rename:
								outerEvent = this.CreateInboundCommonOuterEvent<TeamRenameEvent>(eventJobject);
								break;
							case EventType.user_change:
								outerEvent = this.CreateInboundCommonOuterEvent<UserChangeEvent>(eventJobject);
								break;
							case EventType.tokens_revoked:
								outerEvent = this.CreateInboundCommonOuterEvent<TokensRevokedEvent>(eventJobject);
								break;
						}
						break;
					case OuterEventType.url_verification:
						outerEvent = eventJobject.ToObject<UrlVerificationEvent>();
						break;
					case OuterEventType.app_rate_limited:
						outerEvent = eventJobject.ToObject<AppRateLimitedEvent>();
						break;
				}
			}
			catch (Exception)
			{
				throw;
			}

			outerEvent.RawData = json;
			return outerEvent;
		}

		private static OuterEventType ParseOuterEventType(JObject eventJobject)
		{
			var eventType = OuterEventType.Unknown;
			Enum.TryParse(eventJobject["type"].Value<string>(), true, out eventType);

			return eventType;
		}

		private static EventType ParseEventType(JObject eventJobject)
		{
			var eventType = EventType.Unknown;
			Enum.TryParse(eventJobject["event"]["type"].Value<string>(), true, out eventType);

			return eventType;
		}

		private static T ParseEventSubType<T>(JObject eventJobject) where T : struct
		{
			T eventType = default(T);
			if (eventJobject["event"]["subtype"] != null)
				Enum.TryParse<T>(eventJobject["event"]["subtype"].Value<string>(), true, out eventType);

			return eventType;
		}
	}
}
