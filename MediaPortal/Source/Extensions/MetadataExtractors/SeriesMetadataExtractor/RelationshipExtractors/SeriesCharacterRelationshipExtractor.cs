﻿#region Copyright (C) 2007-2015 Team MediaPortal

/*
    Copyright (C) 2007-2015 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.MediaManagement.Helpers;
using MediaPortal.Common.General;
using MediaPortal.Extensions.OnlineLibraries;

namespace MediaPortal.Extensions.MetadataExtractors.SeriesMetadataExtractor
{
  class SeriesCharacterRelationshipExtractor : IRelationshipRoleExtractor, ISeriesRelationshipExtractor
  {
    private static readonly Guid[] ROLE_ASPECTS = { SeriesAspect.ASPECT_ID };
    private static readonly Guid[] LINKED_ROLE_ASPECTS = { CharacterAspect.ASPECT_ID };
    private CheckedItemCache<SeriesInfo> _checkCache = new CheckedItemCache<SeriesInfo>(SeriesMetadataExtractor.MINIMUM_HOUR_AGE_BEFORE_UPDATE);

    public bool BuildRelationship
    {
      get { return true; }
    }

    public Guid Role
    {
      get { return SeriesAspect.ROLE_SERIES; }
    }

    public Guid[] RoleAspects
    {
      get { return ROLE_ASPECTS; }
    }

    public Guid LinkedRole
    {
      get { return CharacterAspect.ROLE_CHARACTER; }
    }

    public Guid[] LinkedRoleAspects
    {
      get { return LINKED_ROLE_ASPECTS; }
    }

    public bool TryExtractRelationships(IDictionary<Guid, IList<MediaItemAspect>> aspects, out ICollection<IDictionary<Guid, IList<MediaItemAspect>>> extractedLinkedAspects, bool forceQuickMode)
    {
      extractedLinkedAspects = null;

      if (forceQuickMode)
        return false;

      SeriesInfo seriesInfo = new SeriesInfo();
      if (!seriesInfo.FromMetadata(aspects))
        return false;

      if (_checkCache.IsItemChecked(seriesInfo))
        return false;

      OnlineMatcherService.UpdateSeriesCharacters(seriesInfo, forceQuickMode);

      if (seriesInfo.Characters.Count == 0)
        return false;

      bool relationshipFound = false;
      IList<MultipleMediaItemAspect> relationships;
      if (MediaItemAspect.TryGetAspects(aspects, RelationshipAspect.Metadata, out relationships))
      {
        foreach (MultipleMediaItemAspect relationship in relationships)
        {
          if (relationship.GetAttributeValue<Guid>(RelationshipAspect.ATTR_LINKED_ROLE) == LinkedRole ||
            relationship.GetAttributeValue<Guid>(RelationshipAspect.ATTR_ROLE) == LinkedRole)
          {
            relationshipFound = true;
            break;
          }
        }
      }

      if (!relationshipFound)
        seriesInfo.HasChanged = true; //Force save if no relationship exists

      if (!seriesInfo.HasChanged && !forceQuickMode)
        return false;

      extractedLinkedAspects = new List<IDictionary<Guid, IList<MediaItemAspect>>>();

      foreach (CharacterInfo character in seriesInfo.Characters)
      {
        IDictionary<Guid, IList<MediaItemAspect>> characterAspects = new Dictionary<Guid, IList<MediaItemAspect>>();
        character.SetMetadata(characterAspects);

        if (characterAspects.ContainsKey(ExternalIdentifierAspect.ASPECT_ID))
          extractedLinkedAspects.Add(characterAspects);
      }
      return extractedLinkedAspects.Count > 0;
    }

    public bool TryMatch(IDictionary<Guid, IList<MediaItemAspect>> extractedAspects, IDictionary<Guid, IList<MediaItemAspect>> existingAspects)
    {
      if (!existingAspects.ContainsKey(CharacterAspect.ASPECT_ID))
        return false;

      CharacterInfo linkedCharacter = new CharacterInfo();
      if (!linkedCharacter.FromMetadata(extractedAspects))
        return false;

      CharacterInfo existingCharacter = new CharacterInfo();
      if (!existingCharacter.FromMetadata(existingAspects))
        return false;

      return linkedCharacter.Equals(existingCharacter);
    }

    public bool TryGetRelationshipIndex(IDictionary<Guid, IList<MediaItemAspect>> aspects, IDictionary<Guid, IList<MediaItemAspect>> linkedAspects, out int index)
    {
      index = -1;

      SingleMediaItemAspect linkedAspect;
      if (!MediaItemAspect.TryGetAspect(linkedAspects, CharacterAspect.Metadata, out linkedAspect))
        return false;

      string name = linkedAspect.GetAttributeValue<string>(CharacterAspect.ATTR_CHARACTER_NAME);

      SingleMediaItemAspect aspect;
      if (!MediaItemAspect.TryGetAspect(aspects, SeriesAspect.Metadata, out aspect))
        return false;

      IEnumerable<object> actors = aspect.GetCollectionAttribute<object>(SeriesAspect.ATTR_CHARACTERS);
      List<string> nameList = new List<string>(actors.Cast<string>());

      index = nameList.IndexOf(name);
      return index >= 0;
    }

    public void ClearCache()
    {
      _checkCache.ClearCache();
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}