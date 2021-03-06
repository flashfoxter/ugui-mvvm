﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace uguimvvm
{
    public class ListBox : Primitives.Selector
    {        
        private GameObject _lastSelected;
        private static Func<ListBoxItem, bool> _selectionState = s => s == null ? false : s.IsSelected();

        //because there are no events for when the selected object changes....
        void LateUpdate()
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == _lastSelected) return;
            _lastSelected = selected;
            
            //verify that our 'last' actual control is still selected
            if (SelectedInfo != null && SelectedInfo.Control != null)
            {
                if (_selectionState(SelectedInfo.Control.GetComponent<ListBoxItem>()))
                    return;
            }


            //otherwise we can change it to whatever is really selected now
            SetSelected(GetItemInfo(selected));
        }

        private ItemInfo GetItemInfo(GameObject selected)
        {
            if (selected == null) return null;

            foreach (var info in _items)
            {
                if (info.Control == selected)
                    return info;
            }

            //we only use the first parent, in the case of nested listboxes
            var parentItem = selected.GetComponentInParent<ListBoxItem>();
            var parent = parentItem == null ? null : parentItem.gameObject;
            foreach(var info in _items)
            {
                if (info.Control == parent)
                    return info;
            }

            return null;
        }

        protected override void OnSelectedChanged(bool fromProperty)
        {
            if (!fromProperty)
            {
                if (SelectedInfo != null)
                    EventSystem.current.SetSelectedGameObject(SelectedInfo.Control);
                else
                    EventSystem.current.SetSelectedGameObject(null);
            }

            base.OnSelectedChanged(fromProperty);
        }
    }
}
