
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CoreXF
{

    public class INPCspyder: ObservableObject,  IDisposable
    {
        public bool HasChanges { get; set; }

        public bool Enabled { get; set; }

        List<INotifyPropertyChanged> _notif = new List<INotifyPropertyChanged>();
        List<INotifyCollectionChanged> _notifCoolection = new List<INotifyCollectionChanged>();
        INotifyPropertyChanged _root;

        List<object> _objs;

        void AddNotifier(INotifyPropertyChanged obj)
        {
            _objs = new List<object>();
            AddNotifierRecursive(obj);
            _objs.Clear();
            _objs = null;

        }

        void AddNotifierRecursive(INotifyPropertyChanged obj)
        {
            if (obj == null || _objs.Contains(obj))
                return;

            _objs.Add(obj);

            _notif.Add(obj);
            obj.PropertyChanged += Obj_PropertyChanged;

            foreach(var prop in obj.GetType().GetProperties())
            {
                if(prop.Name == "Item")
                {
                    continue;
                }
                var elm = prop.GetValue(obj);
                if (elm == null)
                    continue;

                var elmINPC = elm as INotifyPropertyChanged;
                if(elmINPC != null)
                {
                    AddNotifierRecursive(elmINPC);
                }

                var elmIObColl = elm as INotifyCollectionChanged;
                if(elmIObColl != null)
                {
                    _notifCoolection.Add(elmIObColl);
                    elmIObColl.CollectionChanged += ElmIObColl_CollectionChanged;

                    var coll = elmIObColl as IEnumerable;
                    foreach (var elmOfColl in coll)
                    {
                        if (elmOfColl == null)
                            continue;
                        try
                        {
                            var elmOfCollINPC = elmOfColl as INotifyPropertyChanged;
                            if (elmOfCollINPC != null)
                                AddNotifierRecursive(elmOfCollINPC);
                        }
                        catch
                        {
                        }

                    }
                }

                //var elmEnumerable = elm as IEnumerable;
                //if(elmE)
                //if (elm as INotifyPropertyChanged)
            }
        }


        void SetHasChanges()
        {
            if (Enabled)
            {
                HasChanges = true;
            }
        }

        private void ElmIObColl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetHasChanges();
        }

        void Clear()
        {
            foreach(var obj in _notif)
            {
                obj.PropertyChanged -= Obj_PropertyChanged;
            }
            _notif.Clear();

            foreach (var obj in _notifCoolection)
            {
                obj.CollectionChanged -= ElmIObColl_CollectionChanged;
            }
            _notifCoolection.Clear();

        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetHasChanges();
        }

        public void Rebuild(INotifyPropertyChanged obj = null)
        {
            Clear();
            if (obj != null)
            {
                _root = obj;
            }
            
            AddNotifier(_root);
        }

        public INPCspyder()
        {

        }

        public INPCspyder(INotifyPropertyChanged obj)
        {
            _root = obj;
            Rebuild();
        }

        public void Dispose()
        {
            Clear();
            _root = null;
        }

    }
}
