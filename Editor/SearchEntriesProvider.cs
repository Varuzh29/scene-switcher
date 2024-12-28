using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SearchEntriesProvider : ScriptableObject, ISearchWindowProvider
{
    private SearchTreeEntry[] _entries;
    private Action<string> _callback;
    private string _title;

    public void Initialize(string title, SearchTreeEntry[] entries, Action<string> callback)
    {
        _title = title;
        _entries = entries;
        _callback = callback;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var searchList = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent(_title)),
        };

        searchList.AddRange(_entries);

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _callback?.Invoke((string)searchTreeEntry.userData);
        return true;
    }
}