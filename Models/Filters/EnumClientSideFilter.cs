﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MainBit.Projections.ClientSide.Storage;

namespace MainBit.Projections.ClientSide.Models.Filters
{
    public class EnumClientSideFilter : ClientSideFilter
    {
        private static readonly char[] separator = new [] {'{', '}', ','};

        private EnumClientSideFilterItemEntry[] _items = null;
        public EnumClientSideFilterItemEntry[] Items
        {
            get {
                if (_items == null)
                {
                    _items = DecodeValues(Storage.Get<object>());
                }
                return _items;
            }
            set {
                _items = value;
                Storage.Set(EncodeValues(value));
            }
        }


        private string EncodeValues(ICollection<EnumClientSideFilterItemEntry> items) {
            if (items == null || !items.Any()) {
                return string.Empty;
            }

            // use {1},{2} format so it can be filtered with delimiters
            return "{" + string.Join("},{", items.Select(v => v.DisplayValue).ToArray()) + "}";
        }

        private EnumClientSideFilterItemEntry[] DecodeValues(object items)
        {
            var arItems = new string[0];

            if (items is string[])
            {
                arItems = items as string[];
            }
            else if (items is string && !String.IsNullOrWhiteSpace(items.ToString()))
            {
                arItems = items.ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }

            int index = 0;
            return arItems.Select(item => new EnumClientSideFilterItemEntry
            { 
                DisplayValue = item,
                Id = (++index).ToString(),
                Selected = false
            }).ToArray();
        }
    }

    public class EnumClientSideFilterItemEntry
    {
        public string Id { get; set; }
        public string DisplayValue { get; set; }
        public bool Selected { get; set; }
    }
}