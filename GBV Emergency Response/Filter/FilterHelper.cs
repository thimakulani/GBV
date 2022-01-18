using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Java.Lang;

namespace GBV_Emergency_Response.Filters
{
    class FilterHelper : Filter
    {
        static JavaList<AppUsers> currentList;
        static AppUsersAdapter adapter;
        FilterResults filterResults = new FilterResults();
        public static Filter newInstance(JavaList<AppUsers> currentList, AppUsersAdapter adapter1)
        {
            FilterHelper.currentList = currentList;
            adapter = adapter1;

            return new FilterHelper();
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
           
            if (constraint != null && constraint.Length() > 0)
            {
                string query = constraint.ToString().ToUpper();
                JavaList<AppUsers> foundFilter = new JavaList<AppUsers>();
                for (int i = 0; i < currentList.Size(); i++ )
                {
                    string name = currentList[i].Name;
                    string email = currentList[i].Email;
                    string contact = currentList[i].PhoneNumber;

                    if ( name.ToUpper().Contains(query.ToString()) 
                        || email.ToUpper().Contains(query.ToString())
                        || contact.ToUpper().Contains(query.ToString()))
                    {
                        foundFilter.Add(new AppUsers{ Name = currentList[i].Name
                                                      , PhoneNumber = currentList[i].PhoneNumber
                                                      , Email = currentList[i].Email });
                    }
                }
                filterResults.Count = foundFilter.Size();
                filterResults.Values = foundFilter;
            }
            else
            {
                filterResults.Count = currentList.Size();
                filterResults.Values = currentList;
            }

            return filterResults;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            //adapter.SetList((JavaList<AppUsers>)results.Values);
            adapter.NotifyDataSetChanged();
        }
    }
}