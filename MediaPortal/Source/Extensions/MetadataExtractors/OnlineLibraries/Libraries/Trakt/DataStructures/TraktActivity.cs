﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MediaPortal.Extensions.OnlineLibraries.Libraries.Trakt.DataStructures
{
  [DataContract]
  public class TraktActivity
  {
    [DataMember(Name = "timestamps")]
    public TraktTimestamps Timestamps { get; set; }

    [DataContract]
    public class TraktTimestamps
    {
      [DataMember(Name = "start")]
      public long Start { get; set; }

      [DataMember(Name = "current")]
      public long Current { get; set; }
    }

    [DataMember(Name = "activity")]
    public List<Activity> Activities { get; set; }

    [DataContract]
    public class Activity : IEquatable<Activity>
    {
      #region IEquatable

      public bool Equals(Activity other)
      {
        if (other == null)
          return false;

        return (this.Id == other.Id && this.Type == other.Type);
      }

      public override int GetHashCode()
      {
        return (this.Id.ToString() + "-" + this.Type).GetHashCode();
      }

      #endregion

      [DataMember(Name = "id")]
      public long Id { get; set; }

      [DataMember(Name = "timestamp")]
      public string Timestamp { get; set; }

      [DataMember(Name = "when", EmitDefaultValue = false)]
      public TraktWhen When { get; set; }

      [DataContract]
      public class TraktWhen
      {
        [DataMember(Name = "day")]
        public string Day { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
      }

      [DataMember(Name = "elapsed", EmitDefaultValue = false)]
      public TraktElapsed Elapsed { get; set; }

      [DataContract]
      public class TraktElapsed
      {
        [DataMember(Name = "full")]
        public string Full { get; set; }

        [DataMember(Name = "short")]
        public string Short { get; set; }
      }

      [DataMember(Name = "type")]
      public string Type { get; set; }

      [DataMember(Name = "action")]
      public string Action { get; set; }

      [DataMember(Name = "user")]
      public TraktUserSummary User { get; set; }

      [DataMember(Name = "rating", EmitDefaultValue = false)]
      public int Rating { get; set; }

      [DataMember(Name = "progress", EmitDefaultValue = false)]
      public float Progress { get; set; }

      [DataMember(Name = "episode", EmitDefaultValue = false)]
      public TraktEpisodeSummary Episode { get; set; }

      [DataMember(Name = "episodes", EmitDefaultValue = false)]
      public List<TraktEpisodeSummary> Episodes { get; set; }

      [DataMember(Name = "show", EmitDefaultValue = false)]
      public TraktShowSummary Show { get; set; }

      [DataMember(Name = "season", EmitDefaultValue = false)]
      public TraktSeasonSummary Season { get; set; }

      [DataMember(Name = "movie", EmitDefaultValue = false)]
      public TraktMovieSummary Movie { get; set; }

      [DataMember(Name = "shout", EmitDefaultValue = false)]
      public TraktComment Shout { get; set; }

      [DataMember(Name = "list", EmitDefaultValue = false)]
      public TraktListDetail List { get; set; }

      [DataMember(Name = "list_item", EmitDefaultValue = false)]
      public TraktListItem ListItem { get; set; }

      [DataMember(Name = "person", EmitDefaultValue = false)]
      public TraktPersonSummary Person { get; set; }
    }
  }
}
