#region Copyright (C) 2007-2008 Team MediaPortal

/*
    Copyright (C) 2007-2008 Team MediaPortal
    http://www.team-mediaportal.com
 
    This file is part of MediaPortal II

    MediaPortal II is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal II is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal II.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Drawing;
using MediaPortal.Control.InputManager;
using MediaPortal.Presentation.DataObjects;
using MediaPortal.SkinEngine.InputManagement;
using MediaPortal.Utilities.DeepCopy;

namespace MediaPortal.SkinEngine.Controls.Visuals
{
  public class ScrollViewer : ContentControl
  {
    #region Protected fields

    protected const float SCROLLBAR_MINLENGTH = 10f;

    protected Property _scrollBarXKnobPosProperty;
    protected Property _scrollBarXKnobWidthProperty;
    protected Property _scrollBarXVisibleProperty;
    protected Property _scrollBarYKnobPosProperty;
    protected Property _scrollBarYKnobHeightProperty;
    protected Property _scrollBarYVisibleProperty;

    #endregion

    #region Ctor

    public ScrollViewer()
    {
      Init();
      Attach();
      UpdateScrollBars();
    }

    void Init()
    {
      _scrollBarXKnobPosProperty = new Property(typeof(float), 0f);
      _scrollBarXKnobWidthProperty = new Property(typeof(float), 30f);
      _scrollBarXVisibleProperty = new Property(typeof(bool), false);
      _scrollBarYKnobPosProperty = new Property(typeof(float), 0f);
      _scrollBarYKnobHeightProperty = new Property(typeof(float), 30f);
      _scrollBarYVisibleProperty = new Property(typeof(bool), false);
    }

    void Attach()
    {
      ContentProperty.Attach(OnContentChanged);
    }

    void Detach()
    {
      ContentProperty.Detach(OnContentChanged);
    }

    public override void DeepCopy(IDeepCopyable source, ICopyManager copyManager)
    {
      Detach();
      base.DeepCopy(source, copyManager);
      ScrollViewer sv = (ScrollViewer) source;
      ScrollBarXKnobPos = sv.ScrollBarXKnobPos;
      ScrollBarXKnobWidth = sv.ScrollBarXKnobWidth;
      ScrollBarYKnobPos = sv.ScrollBarYKnobPos;
      ScrollBarYKnobHeight = sv.ScrollBarYKnobHeight;
      Attach();
      UpdateScrollBars();
      ConfigureContentScrollFacility();
    }
    #endregion

    void OnContentChanged(Property property, object oldValue)
    {
      UpdateScrollBars();
      ConfigureContentScrollFacility();
    }

    void UpdateScrollBars()
    {
      ScrollContentPresenter scp =
          FindElement_DepthFirst(new SubTypeFinder(typeof(ScrollContentPresenter))) as ScrollContentPresenter;
      if (scp == null)
        return;
      IScrollInfo scrollInfo = scp.TemplateControl as IScrollInfo;
      if (scrollInfo == null)
        return;
      float totalWidth = Math.Max(1, scrollInfo.TotalWidth);
      float totalHeight = Math.Max(1, scrollInfo.TotalHeight);
      float scrollAreaWidth = (float)scp.ActualWidth;
      float scrollAreaHeight = (float) scp.ActualHeight;
      ScrollBarXKnobWidth = Math.Min(scrollAreaWidth, Math.Max(
          scrollInfo.ViewPortWidth / totalWidth * scrollAreaWidth, SCROLLBAR_MINLENGTH));
      ScrollBarXKnobPos = Math.Min(scrollAreaWidth-ScrollBarXKnobWidth,
          scrollInfo.ViewPortStartX / totalWidth * scrollAreaWidth);
      ScrollBarXVisible = totalWidth > scrollInfo.ViewPortWidth;
      ScrollBarYKnobHeight = Math.Min(scrollAreaHeight, Math.Max(
          scrollInfo.ViewPortHeight / totalHeight * scrollAreaHeight, SCROLLBAR_MINLENGTH));
      ScrollBarYKnobPos = Math.Min(scrollAreaHeight - ScrollBarYKnobHeight,
          scrollInfo.ViewPortStartY / totalHeight * scrollAreaHeight);
      ScrollBarYVisible = totalHeight > scrollInfo.ViewPortHeight;
    }

    void ConfigureContentScrollFacility()
    {
      IScrollInfo scrollInfo = Content as IScrollInfo;
      if (scrollInfo == null)
        return;
      scrollInfo.CanScroll = true;
    }

    public Property ScrollBarXKnobPosProperty
    {
      get { return _scrollBarXKnobPosProperty; }
    }

    public float ScrollBarXKnobPos
    {
      get { return (float) _scrollBarXKnobPosProperty.GetValue(); }
      set { _scrollBarXKnobPosProperty.SetValue(value); }
    }

    public Property ScrollBarXKnobWidthProperty
    {
      get { return _scrollBarXKnobWidthProperty; }
    }

    public float ScrollBarXKnobWidth
    {
      get { return (float) _scrollBarXKnobWidthProperty.GetValue(); }
      set { _scrollBarXKnobWidthProperty.SetValue(value); }
    }

    public bool ScrollBarXVisible
    {
      get { return (bool) _scrollBarXVisibleProperty.GetValue(); }
      set { _scrollBarXVisibleProperty.SetValue(value); }
    }

    public Property ScrollBarXVisibleProperty
    {
      get { return _scrollBarXVisibleProperty; }
    }

    public Property ScrollBarYKnobPosProperty
    {
      get { return _scrollBarYKnobPosProperty; }
    }

    public float ScrollBarYKnobPos
    {
      get { return (float) _scrollBarYKnobPosProperty.GetValue(); }
      set { _scrollBarYKnobPosProperty.SetValue(value); }
    }

    public Property ScrollBarYKnobHeightProperty
    {
      get { return _scrollBarYKnobHeightProperty; }
    }

    public float ScrollBarYKnobHeight
    {
      get { return (float) _scrollBarYKnobHeightProperty.GetValue(); }
      set { _scrollBarYKnobHeightProperty.SetValue(value); }
    }

    public Property ScrollBarYVisibleProperty
    {
      get { return _scrollBarYVisibleProperty; }
    }

    public bool ScrollBarYVisible
    {
      get { return (bool) _scrollBarYVisibleProperty.GetValue(); }
      set { _scrollBarYVisibleProperty.SetValue(value); }
    }

    public override void Arrange(RectangleF finalRect)
    {
      base.Arrange(finalRect);
      UpdateScrollBars();
    }

    public override void OnKeyPressed(ref Key key)
    {
      FrameworkElement content = TemplateControl;
      if (content == null)
        return;

      // Let the children handle the key event first.
      content.OnKeyPressed(ref key);

      if (key == Key.None)
        // Key event was handeled by child
        return;

      if (!CheckFocusInScope())
        return;

      if (key == Key.Down && OnDown())
        key = Key.None;
      else if (key == Key.Up && OnUp())
        key = Key.None;
      else if (key == Key.Left && OnLeft())
        key = Key.None;
      else if (key == Key.Right && OnRight())
        key = Key.None;
      else if (key == Key.Home && OnHome())
        key = Key.None;
      else if (key == Key.End && OnEnd())
        key = Key.None;
      else if (key == Key.PageDown && OnPageDown())
        key = Key.None;
      else if (key == Key.PageUp && OnPageUp())
        key = Key.None;
    }

    /// <summary>
    /// Checks if the currently focused control is contained in this scrollviewer and
    /// is not contained in a sub scrollviewer. This is necessary for this scrollviewer to
    /// handle the focus scrolling keys in this scope.
    /// </summary>
    bool CheckFocusInScope()
    {
      Visual focusPath = Screen == null ? null : Screen.FocusedElement;
      while (focusPath != null)
      {
        if (focusPath == this)
          // Focused control is located in our focus scope
          return true;
        if (focusPath is ScrollViewer)
          // Focused control is located in another scrollviewer's focus scope
          return false;
        focusPath = focusPath.VisualParent;
      }
      return false;
    }

    bool OnHome()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusHome();
    }

    bool OnEnd()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusEnd();
    }

    bool OnPageDown()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusPageDown();
    }

    bool OnPageUp()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusPageUp();
    }

    bool OnLeft()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusLeft();
    }

    bool OnRight()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusRight();
    }

    bool OnDown()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusDown();
    }

    bool OnUp()
    {
      IScrollViewerFocusSupport svfs = Content as IScrollViewerFocusSupport;
      return svfs != null && svfs.FocusUp();
    }
  }
}
