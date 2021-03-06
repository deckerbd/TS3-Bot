﻿namespace DirkSarodnick.TS3_Bot.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using TS3QueryLib.Core.Server.Entities;
    using TS3QueryLib.Core.Server.Responses;

    /// <summary>
    /// Defines the ChannelData class.
    /// </summary>
    public class ChannelData : DefaultData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelData"/> class.
        /// </summary>
        /// <param name="repository"></param>
        public ChannelData(DataRepository repository) : base(repository)
        {
        }

        /// <summary>
        /// Gets the channel info.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <returns></returns>
        public ChannelInfoResponse GetChannelInfo(uint channelId)
        {
            lock (Container.lockGetChannelInfo)
            {
                if (!Container.ChannelInfoList.ContainsKey(channelId))
                    Container.ChannelInfoList.Add(channelId, QueryRunner.GetChannelInfo(channelId));
            }
            return Container.ChannelInfoList[channelId];
        }

        /// <summary>
        /// Gets the channel list info.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <returns></returns>
        public ChannelListEntry GetChannelListInfo(uint channelId)
        {
            return GetChannelList().FirstOrDefault(c => c.ChannelId == channelId);
        }

        /// <summary>
        /// Gets the channel list.
        /// </summary>
        /// <returns></returns>
        public List<ChannelListEntry> GetChannelList()
        {
            lock (Container.lockGetChannelList)
            {
                if (Container.ChannelList == null)
                    Container.ChannelList = QueryRunner.GetChannelList(true, true, false, false, false).Where(m => !m.IsSpacer).ToList();

                return Container.ChannelList.ToList();
            }
        }

        /// <summary>
        /// Gets the channel list.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns></returns>
        public List<ChannelListEntry> GetChannelList(uint serverId)
        {
            return DynamicQueryRunner(serverId).GetChannelList(true, true, false, false, false).Where(m => !m.IsSpacer).ToList();
        }

        #region Sticky Channel

        /// <summary>
        /// Gets the sticky clients.
        /// </summary>
        /// <returns></returns>
        public List<Sticky> GetStickyClients()
        {
            lock (Container.lockStickyClientList)
            {
                lock (Repository.Container.lockDatabase)
                {
                    return Repository.Container.Database.Sticky.ToList();
                }
            }
        }

        /// <summary>
        /// Adds the sticky clients.
        /// </summary>
        /// <param name="clientDatabaseId">The client database id.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="stickTime">The stick time.</param>
        public void AddStickyClients(uint clientDatabaseId, uint channelId, uint stickTime)
        {
            lock (Container.lockStickyClientList)
            {
                lock (Repository.Container.lockDatabase)
                {
                    Repository.Container.Database.Sticky.Where(c => c.ClientDatabaseId == clientDatabaseId && c.ChannelId == channelId).ToList().ForEach(c => Repository.Container.Database.Sticky.DeleteObject(c));
                    Repository.Container.Database.Sticky.AddObject(new Sticky
                    {
                        Id = Guid.NewGuid(),
                        ClientDatabaseId = (int)clientDatabaseId,
                        ChannelId = (int)channelId,
                        StickTime = (int)stickTime,
                        Creation = Repository.Static.Now
                    });
                    Repository.Container.Database.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Removes the sticky clients.
        /// </summary>
        /// <param name="clientDatabaseId">The client database id.</param>
        /// <param name="channelId">The channel id.</param>
        public void RemoveStickyClients(uint clientDatabaseId, uint? channelId = null)
        {
            lock (Container.lockStickyClientList)
            {
                lock (Repository.Container.lockDatabase)
                {
                    List<Sticky> entities = channelId.HasValue
                                            ? Repository.Container.Database.Sticky.Where(m => m.ClientDatabaseId == clientDatabaseId && m.ChannelId == channelId.Value).ToList()
                                            : Repository.Container.Database.Sticky.Where(m => m.ClientDatabaseId == clientDatabaseId).ToList();
                    entities.ForEach(c => Repository.Container.Database.Sticky.DeleteObject(c));
                    Repository.Container.Database.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets the client sticky.
        /// </summary>
        /// <param name="clientDatabaseId">The client database id.</param>
        /// <returns></returns>
        public uint? GetClientSticky(uint clientDatabaseId)
        {
            lock (Container.lockStickyClientList)
            {
                lock (Repository.Container.lockDatabase)
                {
                    if (Repository.Container.Database.Sticky.Any(m => m.ClientDatabaseId == clientDatabaseId && m.ChannelId == Repository.Settings.Sticky.Channel))
                    {
                        return Repository.Settings.Sticky.Channel;
                    }

                    if (Repository.Container.Database.Sticky.Any(m => m.ClientDatabaseId == clientDatabaseId))
                    {
                        return (uint?)Repository.Container.Database.Sticky.First(m => m.ClientDatabaseId == clientDatabaseId).ChannelId;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}